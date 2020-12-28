using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace FmpAnalyzer
{
    public class MainWindowViewModel : DependencyObject
    {
        public static readonly DependencyProperty ConnectionStringProperty;
        public static readonly DependencyProperty ResultsProperty;
        public static readonly DependencyProperty RoeFilterProperty;
        public RelayCommand CommandGo { get; set; }

        static MainWindowViewModel()
        {
            ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            ResultsProperty = DependencyProperty.Register("Results", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            RoeFilterProperty = DependencyProperty.Register("RoeFilter", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(Double)));
        }

        public MainWindowViewModel()
        {
            ConnectionString = Configuration.Instance["ConnectionString"];
            RoeFilter= 15;

            CommandGo = new RelayCommand(p => { OnCommandGo(p); });
        }

        /// <summary>
        /// ConnectionString
        /// </summary>
        public string ConnectionString
        {
            get { return (string)GetValue(ConnectionStringProperty); }
            set { SetValue(ConnectionStringProperty, value); }
        }

        /// <summary>
        /// Results
        /// </summary>
        public string Results
        {
            get { return (string)GetValue(ResultsProperty); }
            set { SetValue(ResultsProperty, value); }
        }

        /// <summary>
        /// RoeFilter
        /// </summary>
        public double RoeFilter
        {
            get { return (double)GetValue(RoeFilterProperty); }
            set { SetValue(RoeFilterProperty, value); }
        }

        /// <summary>
        /// OnCommandGo
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandGo(object p)
        {
            var symbols = Companies.Instance.Compounder("2019-12-31", RoeFilter);
            Results = $"Found {symbols.Count()} companies:";
            symbols.ForEach(s => Results += $"\r\n{s}");
        }

        /// <summary>
        /// Diverse
        /// </summary>
        private void Diverse()
        {
            //Dictionary<string, List<double>> dictResults = new Dictionary<string, List<double>>();

            //// ROE
            //var topRoeList = Companies.Instance.WithBestRoe(10, "2019-12-31");
            //foreach (var symbol in topRoeList)
            //{
            //    dictResults[symbol] = null;
            //}

            //// ROE History
            //foreach (var symbol in topRoeList)
            //{
            //    dictResults[symbol] = Companies.Instance.HistoryRoe(symbol, "2019-12-31", 5);
            //}

            //// Output
            //foreach (var symbol in dictResults.Keys)
            //{
            //    Results += symbol;
            //    foreach (var roe in dictResults[symbol])
            //    {
            //        Results += $"\t{roe}";
            //    }
            //    Results += Environment.NewLine;
            //}

        }

    }
}
