using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace _1sServerWidget
{
    public class ElementsFormConverter : MarkupExtension, IValueConverter
    {
        private static ElementsFormConverter _instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = ToDouble(value) - ToDouble(parameter);
            return result < 0 ? value : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ToDouble(value) + ToDouble(parameter);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new ElementsFormConverter());
        }

        private double ToDouble(object value)
        {
            return System.Convert.ToDouble(value);
        }
    }
}
