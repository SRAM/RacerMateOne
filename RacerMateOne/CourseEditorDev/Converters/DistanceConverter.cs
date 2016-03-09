using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace RacerMateOne.CourseEditorDev.Converters
{
    public class DistanceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            const double KiloMeter = 1000.0;
            const double OneMileInFeet = 5280.0;

            string strExtra = parameter == null ? "2" : System.Convert.ToString(parameter);
            int RoundTo = int.Parse(strExtra);
            
            double OrgValue = (double) values[0];
            bool Metric = (bool) values[1];

            string unit;
            if (Metric == true)
            {
                if (OrgValue > 1000)
                {
                    OrgValue /= KiloMeter;
                    unit = "Km";
                }
                else
                {
                    unit = "Meter";
 
                }
            }
            else
            {
                if (OrgValue > OneMileInFeet)
                {
                    OrgValue /= OneMileInFeet;
                    unit = "Miles";
                }
                else
                {
                    unit = "Feet";
                }
            }
            

            string strValue = System.Convert.ToString(Math.Round((double)OrgValue, RoundTo));
            double dValue = System.Convert.ToDouble(strValue);
            string strMewValue = string.Format("{0:0.##} {1}", dValue, unit);
            return strMewValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
  
    }
}
