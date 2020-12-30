using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace FmpAnalyzer.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    public class RoeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (String.IsNullOrWhiteSpace((string)value))
            {
                return default(double);
            }

            return System.Convert.ToDouble(value);
        }
    }
}
