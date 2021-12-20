using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BSCRM.InstallerAPI;

namespace BSCRM.InstallerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Bigscreen.exe";
            dialog.Filter = "Executable (*.exe) | *.exe | All files(*.*) | *.*";
            if (dialog.ShowDialog() == true)
            {
                var loc = dialog.FileName;
                WebUtils.completionUpdate += WebUtils_completionUpdate;
                WebUtils.DownloadAndInstall("v0.5.2", loc);
            }
        }

        private void WebUtils_completionUpdate(object sender, int e)
        {
            WPB.Value = e;
        }
    }
}
