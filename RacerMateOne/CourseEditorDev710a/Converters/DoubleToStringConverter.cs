using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace RacerMateOne.CourseEditorDev.Converters
{
    public class DoubleToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            string strExtra = parameter == null ? "3" : System.Convert.ToString(parameter);
            double doubleValue = (double)value;
            int RoundTo = int.Parse(strExtra);

            string strValue = System.Convert.ToString(Math.Round((double)value, RoundTo));
            if (string.IsNullOrEmpty(strValue) == false)
            {
                double dValue = System.Convert.ToDouble(strValue);
                string strMewValue = string.Format("{0:0.###}", dValue);

                return strMewValue;
            }

            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
