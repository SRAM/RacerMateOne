using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RacerMateOne.CourseEditorDev.Converters
{

    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
     //       TextBox tb = (TextBox) values;
            return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();

        }
    }
}
