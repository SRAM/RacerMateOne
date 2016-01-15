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

namespace RacerMateOne.CourseEditorDev.Dialogs
{
    /// <summary>
    /// Interaction logic for OkCancelDialog.xaml
    /// </summary>
    public partial class OkCancelDialog : Window
    {
        string strMessage;

        public OkCancelDialog(string strMessage) : this(strMessage, "OK")
        {
        }

        public OkCancelDialog(string strMessage, string strOk)
        {
            this.strMessage = strMessage;
            InitializeComponent();
            OkBtn.Content = strOk;

            Loaded += OkCancelDialog_Loaded;
        }

        void OkCancelDialog_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBoxTB.Text = strMessage;
        }
        
        
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

    }
}
