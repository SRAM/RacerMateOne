using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RacerMateOne.CourseEditorDev
{
    static class Ulilities
    {
        static public double Round(double OrgValue, int RoundTo)
        {
            double newValue = Math.Round(OrgValue, RoundTo);
            string format = "{0:F" + RoundTo + "}";
            string strValue = string.Format(format, newValue);
            double dValue = System.Convert.ToDouble(strValue);
            return dValue;
        }


        static public string ConvertToTime(double OrgValue)
        {
            // OrgValue is in secnds
            const int MinutsInHour = 60;
            const int SecondsInMinute = 60;

            int Hours = (int)(OrgValue / MinutsInHour / SecondsInMinute);
            OrgValue -= Hours * MinutsInHour * SecondsInMinute;
            int Minutes = (int)(OrgValue / SecondsInMinute);
            OrgValue -= Minutes * SecondsInMinute;

            int Seconds = (int)(OrgValue);
            string TimeString = string.Format("{0:00}:{1:00}:{2:00}", Hours, Minutes, Seconds);
            return TimeString;

        }

    }
}
