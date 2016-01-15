using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace RacerMateOne.CourseEditorDev.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int ParaInt = int.Parse(System.Convert.ToString(parameter));
            int ActualValue = (int)values;
            return ActualValue >= ParaInt ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();

        }
    }
}
