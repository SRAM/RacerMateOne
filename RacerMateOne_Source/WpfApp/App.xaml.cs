using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Process executable = new Process();
            executable.StartInfo.FileName = "RacerMateOne.exe";
            executable.StartInfo.UseShellExecute = true;
            executable.Start();

            System.Environment.Exit(0);
        }
    }
}
