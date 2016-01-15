using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace RacerMateOne.CourseEditorDev.Converters
{
    public class TimeConverter : IValueConverter
    {
        const int MinutsInHour = 60;
        const int SecondsInMinute = 60;
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double OrgValue = (double)values;

            int Hours = (int)(OrgValue / MinutsInHour);
            OrgValue  -= Hours * MinutsInHour;
            int Minutes = (int)OrgValue;
            int Seconds = (int)((OrgValue - Minutes) * SecondsInMinute);
            string TimeString = string.Format("{0:00}:{1:00}:{2:00}", Hours, Minutes, Seconds);
            return TimeString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();

        }

    }
}
