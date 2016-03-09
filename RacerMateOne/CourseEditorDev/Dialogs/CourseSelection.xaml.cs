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
    /// Interaction logic for CourseSelection.xaml
    /// </summary>
    public partial class CourseSelection : Window
    {
        public CourseType courseType = CourseType.DISTANCEGRADE;
        public CourseSelection()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (DistanceAndGrade.IsChecked == true)
            {
                courseType = CourseType.DISTANCEGRADE;
            }
            else if (TimeAndGrade.IsChecked == true)
            {
                courseType = CourseType.TIMEGRADE;
            }
            else if (TimeAndWatts.IsChecked == true)
            {
                courseType = CourseType.TIMEWATT;
            }
            else if (DistanceAndWatts.IsChecked == true)
            {
                courseType = CourseType.DISTANCEWATT;
            }
            else if (TimeAndAT.IsChecked == true)
            {
                courseType = CourseType.TIMEPERAT;
            }
            else if (DistanceAndAT.IsChecked == true)
            {
                courseType = CourseType.DISTANCEPERAT;
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
