using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;

namespace BSCRM.InstallerAPI
{
    public static class WebUtils
    {
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        public static event EventHandler<int> completionUpdate;

        public static async void DownloadAndInstall(string versionTag, string gameExecPath)
        {
            string url = $"https://github.com/BigscreenModded/MelonLoader/releases/download/%e%/MelonLoader.x64.zip";
            url = url.Replace("%e%", versionTag);
            WebClient webClient = new WebClient();
            Directory.CreateDirectory("./temp");
            string bee = "";
            completionUpdate.Invoke(null, 5);
            webClient.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
            {
                string ee = "Downloading Melonloader." + versionTag + $" - {e.ProgressPercentage}%";
                if (ee != bee)
                {
                    completionUpdate.Invoke(null, 5 + e.ProgressPercentage / 20);
                    Console.WriteLine(ee);
                    bee = ee;
                }
            };
            await webClient.DownloadFileTaskAsync(url, @"./temp/archive.zip");
            webClient.Dispose();
            ZipFile.ExtractToDirectory("./temp/archive.zip", "./temp/extracted");
            completionUpdate.Invoke(null, 50);
            string gameDir = Path.GetDirectoryName(gameExecPath);

            if (Directory.Exists($"{gameDir}\\MelonLoader"))
            {
                Console.WriteLine("Already found modloader installation - uninstalling first.");
                Directory.Delete($"{gameDir}/MelonLoader", true);
                Directory.Delete($"{gameDir}/Mods", true);
                File.Delete($"{gameDir}/version.dll");
            }
            completionUpdate.Invoke(null, 75);
            Console.WriteLine("Copying extracted/MelonLoader -> " + gameDir + "/MelonLoader");
            DirectoryCopy("./temp/extracted/MelonLoader", gameDir + "/MelonLoader", true);
            Console.WriteLine("Copying extracted/version.dll -> " + gameDir + "/version.dll");
            File.Copy("./temp/extracted/version.dll", gameDir + "/version.dll");
            completionUpdate.Invoke(null, 90);
            Console.WriteLine("Creating Mods folder and sub-folders.");
            Directory.CreateDirectory(gameDir + "/Mods/");
            Directory.CreateDirectory(gameDir + "/Mods/Dependencies");
            File.WriteAllText(gameDir + "/Mods/Dependencies/readme.txt", "This folder is for any external libraries that mods require.");
            Directory.Delete("./temp", true);

            completionUpdate.Invoke(null, 100);
            MessageBox.Show("Installed Modded", "Info");
        }
    }
}
