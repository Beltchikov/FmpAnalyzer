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

        /// <summary>
        /// ItemsSource
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
    }
    
    /// <summary>
    /// RectangleHeightConverter
    /// </summary>
    public class RectangleHeightConverter : IMultiValueConverter
    {
        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!values.Any())
            {
                return values;
            }

            // Get history array and height of control
            List<double> inputValuesList = (List<double>)values[0];
            double currentHistoryValue = (double)values[1];
            double height = (double)values[2];

            // Are values there?
            if (Double.IsNaN(currentHistoryValue) || Double.IsNaN(height) || !inputValuesList.Any())
            {
                return 0;
            }

            // Convert and return
            return ConvertToBarHeight(inputValuesList, currentHistoryValue, height);
        }

        /// <summary>
        /// ConvertBack
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ConvertToBarHeight
        /// </summary>
        /// <param name="inputList"></param>
        /// <param name="currentHistoryValue"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        double ConvertToBarHeight(List<double> inputList, double currentHistoryValue, double height)
        {
            var max = inputList.Max();
            var min = .0;
            if (inputList.Where(v => v < 0).Any())
            {
                min = inputList.Min();
            }
            var range = max - min;
            if (range == 0)
            {
                return Math.Abs(currentHistoryValue);
            }
            var koef = 0.8 * height / range;

            return Math.Abs(currentHistoryValue * koef);
        }
    }

    /// <summary>
    /// NegativeValuesConverter
    /// </summary>
    public class NegativeValuesConverter : IMultiValueConverter
    {
        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!values.Any())
            {
                return values;
            }

            // Get history array and height of control
            List<double> inputValuesList = (List<double>)values[0];
            double currentHistoryValue = (double)values[1];
            double height = (double)values[2];

            // Are values there?
            if (Double.IsNaN(currentHistoryValue) || Double.IsNaN(height) || !inputValuesList.Any())
            {
                return 0;
            }

            // Convert and return
            return ConvertToPositivBarShift(inputValuesList, currentHistoryValue, height);
        }

        /// <summary>
        /// ConvertBack
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ConvertToPositivBarShift
        /// </summary>
        /// <param name="inputList"></param>
        /// <param name="currentHistoryValue"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        double ConvertToPositivBarShift(List<double> inputList, double currentHistoryValue, double height)
        {
            if (!inputList.Where(v => v < 0).Any())
            {
                return 0;
            }

            // Find koef.
            var max = inputList.Max();
            var min = inputList.Min();
            var range = max - min;
            if (range == 0)
            {
                return 0;
            }
            var koef = 0.8 * height / range;

            if (inputList.All(v => v < 0))
            {
                return Math.Abs(currentHistoryValue - min) * koef;
            }
            else
            {
                if (currentHistoryValue > 0)
                {
                    return Math.Abs(min * koef);
                }

                return 0;
            }
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
