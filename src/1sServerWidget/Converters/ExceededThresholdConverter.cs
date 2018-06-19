using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace _1sServerWidget
{
    public class ExceededThresholdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float floatValue = ToFloat(value);

            if (floatValue >= 15)
                return new SolidColorBrush(Colors.Red);
            else if (floatValue >= 10)
                return new SolidColorBrush(Colors.Yellow);
            else if (floatValue >= 5)
                return new SolidColorBrush(Colors.LimeGreen);
            else
                return new SolidColorBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush();
        }

        private float ToFloat(object value)
        {
            return System.Convert.ToSingle(value);
        }
    }
}
