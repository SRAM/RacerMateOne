using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace RacerMateOne.CourseEditorDev.Converters
{
   
    public class VisibilityMode : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CourseType NewValue = (CourseType)value;

            string strPar = System.Convert.ToString(parameter);
            if (strPar != string.Empty)
            {
                CourseType currentMode = (CourseType) int.Parse(strPar);

                Visibility returnValue =  NewValue == currentMode ? Visibility.Visible : Visibility.Hidden;
                return returnValue;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();

        }
    }
}
