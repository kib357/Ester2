using System;
using System.Diagnostics;
using System.Windows;
using System.IO;

namespace EsterUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            var iu = new InstallUpdates();
            iu.Install();
            var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var enMon = dir + @"/Ester.exe";
            if (File.Exists(enMon))
            {
                Process.Start(enMon);
            }
            Application.Current.Shutdown();
        }
    }
}
