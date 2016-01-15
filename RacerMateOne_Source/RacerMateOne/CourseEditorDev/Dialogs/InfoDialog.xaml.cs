using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace RacerMateOne.CourseEditorDev.Dialogs
{
    /// <summary>
    /// Interaction logic for InfoDialog.xaml
    /// </summary>
    public partial class InfoDialog : Window
    {
        string strMessage;
        ShowIcon showIcon;
        string strFileName = string.Empty;

        public InfoDialog(string strMessage, ShowIcon showIcon, string FileName)
        {
            strFileName = FileName;
            this.strMessage = strMessage;
            this.showIcon = showIcon;
            InitializeComponent();
            Loaded += InfoDialog_Loaded;
        }

        public InfoDialog(string strMessage, ShowIcon showIcon)
        {
            strFileName = string.Empty;
            this.strMessage = strMessage;
            this.showIcon = showIcon;
            InitializeComponent();
            Loaded += InfoDialog_Loaded;
        }

        void InfoDialog_Loaded(object sender, RoutedEventArgs e)
        {
            MessageTB.Text = strMessage;
            if (showIcon == ShowIcon.NONE)
            {
                imageStop.Visibility = Visibility.Hidden;
                imageOk.Visibility = Visibility.Hidden;
            }
            else if (showIcon == ShowIcon.STOP)
            {
                imageStop.Visibility = Visibility.Visible;
                imageOk.Visibility = Visibility.Hidden;
            }
            else
            {
                imageStop.Visibility = Visibility.Hidden;
                imageOk.Visibility = Visibility.Visible;
            }

        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(strFileName) == true)
            {
                Process.Start("explorer.exe", strFileName);
            }
        }
    }
}
