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
    /// Interaction logic for SegmentEditDialog.xaml
    /// </summary>
    public partial class SegmentEditDialog : Window
    {
        public SegmentEditDialog()
        {
            InitializeComponent();
        }


        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
