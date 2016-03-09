using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace RacerMateOne.CourseEditorDev.Converters
{
    public class WindConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double windSpeed = (double)value;
            string path = windSpeed == 0 ? "pack://application:,,,/RacerMateOne;component/CourseEditorDev/Images/windEmpty.png" : windSpeed < 0 ? "pack://application:,,,/RacerMateOne;component/CourseEditorDev/Images/wind.png" : "pack://application:,,,/RacerMateOne;component/CourseEditorDev/Images/windNagative.png";
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(path);
            logo.EndInit();
            return logo;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
