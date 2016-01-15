using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace RacerMateOne.CourseEditorDev.Converters
{
    public class DecimalPlacesConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strExtra = parameter == null ? "3" : System.Convert.ToString(parameter);
            int RoundTo = int.Parse(strExtra);
            double OrgValue = (double) values;
           

            string strValue = System.Convert.ToString(Math.Round((double)OrgValue, RoundTo));
            double dValue = System.Convert.ToDouble(strValue);
            string strMewValue = string.Format("{0:0.###}", dValue);
            return strMewValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();

        }
  
    }
}
