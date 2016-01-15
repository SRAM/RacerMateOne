using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace RacerMateOne.CourseEditorDev.Converters
{
    public class CombindConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strExtra = parameter == null ? "3" : System.Convert.ToString(parameter);
            int RoundTo = int.Parse(strExtra);
            double OrgValue = (double) values[0];
            string unit = (string)values[1];

            string strValue = System.Convert.ToString(Math.Round((double)OrgValue, RoundTo));
            double dValue = System.Convert.ToDouble(strValue);
            string strMewValue = string.Format("{0:0.###} {1}", dValue, unit);
            return strMewValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
  
    }
}
