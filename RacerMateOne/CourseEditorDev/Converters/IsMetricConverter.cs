using System;
using System.Windows;
using System.Windows.Data;

namespace RacerMateOne.CourseEditorDev.Converters
{

    public class IsMetricConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Boolean TrueOrFalse = System.Convert.ToBoolean(values);
            return TrueOrFalse == true ? "Metric" : "English";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();

        }
    }
}
