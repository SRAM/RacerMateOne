using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RacerMateOne
{
    /// <summary>
    /// Interaction logic for Window_CustomLoadBackupFile.xaml
    /// </summary>
    public partial class Window_CustomLoadBackupFile : Window
    {
        private string pvCaptionText;
        public string CaptionText
        {
            set
            {
                pvCaptionText = value;
                txtblkCaption.Text = pvCaptionText;
            }
        }
        private int pvUserChoice = 0; //default is to quit app
        public int UserChoice
        { get { return pvUserChoice; } }

        public Window_CustomLoadBackupFile()
        {
            InitializeComponent();
        }

        private void buttonBackup_Click(object sender, RoutedEventArgs e)
        {
            pvUserChoice = 1;
            this.Close();
        }

        private void buttonDefault_Click(object sender, RoutedEventArgs e)
        {
            pvUserChoice = 2;
            this.Close();
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            pvUserChoice = 0;
            this.Close();
        }
    }
}
