﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace RacerMateOne.CourseEditorDev.Converters
{
   
    public class RoundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int round = int.Parse((string)System.Convert.ToString(parameter));
            double newValue = Ulilities.Round((double)value, round);
            return newValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)  {
			  int round = -9;

			  try {
				  round = int.Parse((string)System.Convert.ToString(parameter));
			  }

			  catch (Exception e) {
				  // tlm20160218
//#if DEBUG
				  System.Console.WriteLine("RoundConverter.cs error: '{0}'\n", e.ToString());
//#endif
				Log.WriteLine(e.ToString());
			  }

            string strValue = (string)value;
            if (string.IsNullOrEmpty(strValue) || (strValue.Length == 1 && strValue[0] == '-') || (strValue.Length == 1 && strValue[0] == '.'))
                return 0;
            double newValue = Ulilities.Round(double.Parse(strValue), round);
            return newValue;

        }
    }
}