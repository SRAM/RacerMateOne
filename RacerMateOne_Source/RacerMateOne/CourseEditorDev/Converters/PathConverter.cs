using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace RacerMateOne.CourseEditorDev.Converters
{
    public class PathConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strPath = "F1 M 0,178 2,100L 130,100L 130,178 Z";
            
            const double ListPolygonMaxHeight = 90;
            const double ListPolygonMinHeight = 25;
            const double Delta = 0.00000005;

            double denominator = Math.Abs(PolygonData.MaxValue - PolygonData.MinValue);
            if (denominator <= Delta)
                return string.Format("F1 M 0,178L 0,{0}L 130,{1} 130,178 Z", ListPolygonMaxHeight, ListPolygonMaxHeight);


            PolygonData InputValue = (PolygonData)value;

           
            double ratio = (ListPolygonMaxHeight -  ListPolygonMinHeight) / denominator;
            int leftSide = (int)(ratio * (Math.Abs(InputValue.StartValue - PolygonData.MinValue)));
            int rightSide = (int)(ratio * (Math.Abs(InputValue.EndValue - PolygonData.MinValue)));

            strPath = string.Format("F1 M 0,180L 0,{0}L 130,{1} 130,180 Z", ListPolygonMaxHeight - leftSide, ListPolygonMaxHeight - rightSide);
            
            return strPath;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
