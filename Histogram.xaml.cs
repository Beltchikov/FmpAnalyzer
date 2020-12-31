using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FmpAnalyzer
{
    /// <summary>
    /// Interaction logic for Histogram.xaml
    /// </summary>
    public partial class Histogram : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty;

        static Histogram()
        {
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(Histogram), new PropertyMetadata(default(IEnumerable)));
        }

        public Histogram()
        {
            InitializeComponent();
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
    }

    /// <summary>
    /// HistogramConverter
    /// </summary>
    public class HistogramConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!values.Any())
            {
                return values;
            }

            // Get history array and height of control
            List<double> inputValuesList = new List<double>();
            double height = 0;
            foreach (var inputValue in values)
            {
                if (inputValue is List<double>)
                {
                    inputValuesList = (List<double>)inputValue;
                }
                if(inputValue is double)
                {
                    height = (double)inputValue;
                }
            }

            // Are values there?
            if(Double.IsNaN(height) || !inputValuesList.Any())
            {
                return inputValuesList;
            }

            // Convert ant return
            return ConvertHistory(inputValuesList, height);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private IEnumerable ConvertHistory(List<double> inputList, double height)
        {
            List<double> outputList = new List<double>();
            
            var max = inputList.Max();
            var min = .0;
            if (inputList.Where(v => v < 0).Any())
            {
                min = inputList.Min();
            }
            var range = max - min;
            var koef = 0.8 * height / range;
            inputList.ForEach(v => outputList.Add(v * koef));

            return outputList;
        }
    }

    /// <summary>
    /// ColorConverter
    /// </summary>
    [ValueConversion(typeof(double), typeof(Brush))]
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) > 0 ? Brushes.Green : Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
