using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RacerMateOne.CourseEditorDev
{
    public class PolygonData
    {
        public double StartValue { get; set; }
        public double EndValue { get; set; }
        public static double MaxValue { get; set; }
        public static double MinValue { get; set; }

        public PolygonData(double startValue, double endValue)
        {
            this.StartValue = startValue;
            this.EndValue = endValue;
        }

        
    }
}
