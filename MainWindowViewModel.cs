using FmpAnalyzer.Data;
using FmpAnalyzer.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FmpAnalyzer
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : DependencyObject
    {
        public static readonly DependencyProperty ConnectionStringProperty;
        public static readonly DependencyProperty RoeProperty;
        public static readonly DependencyProperty CurrentActionProperty;
        public static readonly DependencyProperty StableRoeGrowthProperty;
        public static readonly DependencyProperty HistoryDepthRoeProperty;
        public static readonly DependencyProperty GrowthGradRoeProperty;
        public static readonly DependencyProperty ProgressCurrentProperty;
        public static readonly DependencyProperty StableReinvestmentGrowthProperty;
        public static readonly DependencyProperty HistoryDepthReinvestmentProperty;
        public static readonly DependencyProperty GrowthGradReinvestmentProperty;
        public static readonly DependencyProperty BackgroundResultsProperty;
        public static readonly DependencyProperty AverageIncrementalRoeProperty;
        public static readonly DependencyProperty StableIncrementalRoeGrowthProperty;
        public static readonly DependencyProperty HistoryDepthIncrementalRoeProperty;
        public static readonly DependencyProperty GrowthGradIncrementalRoeProperty;
        public static readonly DependencyProperty RoeYearProperty;
        public static readonly DependencyProperty SymbolsProperty;
        public static readonly DependencyProperty ResultSetListProperty;
        public static readonly DependencyProperty ReinvestmentRateProperty;
        public static readonly DependencyProperty AverageReinvestmentRateProperty;
        public RelayCommand CommandGo { get; set; }

        static MainWindowViewModel()
        {
            ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            RoeProperty = DependencyProperty.Register("Roe", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(Double)));
            CurrentActionProperty = DependencyProperty.Register("CurrentAction", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            StableRoeGrowthProperty = DependencyProperty.Register("StableRoeGrowth", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(default(Boolean)));
            HistoryDepthRoeProperty = DependencyProperty.Register("HistoryDepthRoe", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            GrowthGradRoeProperty = DependencyProperty.Register("GrowthGradRoe", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            ProgressCurrentProperty = DependencyProperty.Register("ProgressCurrent", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            StableReinvestmentGrowthProperty = DependencyProperty.Register("StableReinvestmentGrowth", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(default(Boolean)));
            HistoryDepthReinvestmentProperty = DependencyProperty.Register("HistoryDepthReinvestment", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            GrowthGradReinvestmentProperty = DependencyProperty.Register("GrowthGradReinvestment", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            BackgroundResultsProperty = DependencyProperty.Register("BackgroundResults", typeof(Brush), typeof(MainWindowViewModel), new PropertyMetadata(default(Brush)));
            AverageIncrementalRoeProperty = DependencyProperty.Register("AverageIncrementalRoe", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            StableIncrementalRoeGrowthProperty = DependencyProperty.Register("StableIncrementalRoeGrowth", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(default(Boolean)));
            HistoryDepthIncrementalRoeProperty = DependencyProperty.Register("HistoryDepthIncrementalRoe", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            GrowthGradIncrementalRoeProperty = DependencyProperty.Register("GrowthGradIncrementalRoe", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            RoeYearProperty = DependencyProperty.Register("RoeYear", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            SymbolsProperty = DependencyProperty.Register("Symbols", typeof(List<string>), typeof(MainWindowViewModel), new PropertyMetadata(new List<string>()));
            ResultSetListProperty = DependencyProperty.Register("ResultSetList", typeof(List<ResultSet>), typeof(MainWindowViewModel), new PropertyMetadata(new List<ResultSet>()));
            ReinvestmentRateProperty = DependencyProperty.Register("ReinvestmentRate", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(double)));
            AverageReinvestmentRateProperty = DependencyProperty.Register("AverageReinvestmentRate", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(double)));
        }

        public MainWindowViewModel()
        {
            ConnectionString = Configuration.Instance["ConnectionString"];
            Roe = 15;
            CurrentAction = "Willkommen!";
            StableRoeGrowth = true;
            HistoryDepthRoe = 5;
            GrowthGradRoe = 3;
            StableReinvestmentGrowth = true;
            HistoryDepthReinvestment = 5;
            GrowthGradReinvestment = 3;
            AverageIncrementalRoe = 15;
            StableIncrementalRoeGrowth = true;
            HistoryDepthIncrementalRoe = 5;
            GrowthGradIncrementalRoe = 2;
            RoeYear = "2019";
            ReinvestmentRate = 50;
            AverageReinvestmentRate = 50;

            CommandGo = new RelayCommand(p => { OnCommandGo(p); });

            QueryFactory.CompounderQuery.DatabaseAction += CompounderQuery_DatabaseAction;
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
        /// Roe
        /// </summary>
        public double Roe
        {
            get { return (double)GetValue(RoeProperty); }
            set { SetValue(RoeProperty, value); }
        }

        /// <summary>
        /// CurrentAction
        /// </summary>
        public string CurrentAction
        {
            get { return (string)GetValue(CurrentActionProperty); }
            set { SetValue(CurrentActionProperty, value); }
        }

        /// <summary>
        /// StableRoeGrowth
        /// </summary>
        public bool StableRoeGrowth
        {
            get { return (bool)GetValue(StableRoeGrowthProperty); }
            set { SetValue(StableRoeGrowthProperty, value); }
        }

        /// <summary>
        /// HistoryDepthRoe
        /// </summary>
        public int HistoryDepthRoe
        {
            get { return (int)GetValue(HistoryDepthRoeProperty); }
            set { SetValue(HistoryDepthRoeProperty, value); }
        }

        /// <summary>
        /// GrowthGradRoe
        /// </summary>
        public int GrowthGradRoe
        {
            get { return (int)GetValue(GrowthGradRoeProperty); }
            set { SetValue(GrowthGradRoeProperty, value); }
        }

        /// <summary>
        /// ProgressCurrent
        /// </summary>
        public int ProgressCurrent
        {
            get { return (int)GetValue(ProgressCurrentProperty); }
            set { SetValue(ProgressCurrentProperty, value); }
        }

        /// <summary>
        /// StableReinvestmentGrowth
        /// </summary>
        public bool StableReinvestmentGrowth
        {
            get { return (bool)GetValue(StableReinvestmentGrowthProperty); }
            set { SetValue(StableReinvestmentGrowthProperty, value); }
        }

        /// <summary>
        /// HistoryDepthReinvestment
        /// </summary>
        public int HistoryDepthReinvestment
        {
            get { return (int)GetValue(HistoryDepthReinvestmentProperty); }
            set { SetValue(HistoryDepthReinvestmentProperty, value); }
        }

        /// <summary>
        /// GrowthGradReinvestment
        /// </summary>
        public int GrowthGradReinvestment
        {
            get { return (int)GetValue(GrowthGradReinvestmentProperty); }
            set { SetValue(GrowthGradReinvestmentProperty, value); }
        }

        /// <summary>
        /// BackgroundResults
        /// </summary>
        public Brush BackgroundResults
        {
            get { return (Brush)GetValue(BackgroundResultsProperty); }
            set { SetValue(BackgroundResultsProperty, value); }
        }

        /// <summary>
        /// CompounderQueryParams
        /// </summary>
        public CompounderQueryParams CompounderQueryParams { get; set; }


        /// <summary>
        /// Symbols
        /// </summary>
        public List<string> Symbols
        {
            get { return (List<string>)GetValue(SymbolsProperty); }
            set { SetValue(SymbolsProperty, value); }
        }

        /// <summary>
        /// ResultSetList
        /// </summary>
        public List<ResultSet> ResultSetList
        {
            get { return (List<ResultSet>)GetValue(ResultSetListProperty); }
            set { SetValue(ResultSetListProperty, value); }
        }

        /// <summary>
        /// AverageIncrementalRoe
        /// </summary>
        public int AverageIncrementalRoe
        {
            get { return (int)GetValue(AverageIncrementalRoeProperty); }
            set { SetValue(AverageIncrementalRoeProperty, value); }
        }

        /// <summary>
        ///  StableIncrementalRoeGrowth
        /// </summary>
        public bool StableIncrementalRoeGrowth
        {
            get { return (bool)GetValue(StableIncrementalRoeGrowthProperty); }
            set { SetValue(StableIncrementalRoeGrowthProperty, value); }
        }

        /// <summary>
        /// HistoryDepthIncrementalRoe
        /// </summary>
        public int HistoryDepthIncrementalRoe
        {
            get { return (int)GetValue(HistoryDepthIncrementalRoeProperty); }
            set { SetValue(HistoryDepthIncrementalRoeProperty, value); }
        }

        /// <summary>
        /// GrowthGradIncrementalRoe
        /// </summary>
        public int GrowthGradIncrementalRoe
        {
            get { return (int)GetValue(GrowthGradIncrementalRoeProperty); }
            set { SetValue(GrowthGradIncrementalRoeProperty, value); }
        }

        /// <summary>
        /// RoeYear
        /// </summary>
        public string RoeYear
        {
            get { return (string)GetValue(RoeYearProperty); }
            set { SetValue(RoeYearProperty, value); }
        }

        /// <summary>
        /// ReinvestmentRate
        /// </summary>
        public double ReinvestmentRate
        {
            get { return (double)GetValue(ReinvestmentRateProperty); }
            set { SetValue(ReinvestmentRateProperty, value); }
        }

        /// <summary>
        /// AverageReinvestmentRate
        /// </summary>
        public double AverageReinvestmentRate
        {
            get { return (double)GetValue(AverageReinvestmentRateProperty); }
            set { SetValue(AverageReinvestmentRateProperty, value); }
        }

        /// <summary>
        /// OnCommandGo
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandGo(object p)
        {
            LockControls();

            CompounderQueryParams = new CompounderQueryParams
            {
                Date = RoeYear + Configuration.Instance["DateSuffix"],
                Roe = Roe,
                ReinvestmentRate = ReinvestmentRate,
                HistoryDepthRoe = StableRoeGrowth ? HistoryDepthRoe : 0,
                GrowthGradRoe = StableRoeGrowth ? GrowthGradRoe : 0,
                HistoryDepthReinvestment = StableReinvestmentGrowth ? HistoryDepthReinvestment : 0,
                GrowthGradReinvestment = StableReinvestmentGrowth ? GrowthGradReinvestment : 0
            };

            var worker = new BackgroundWorker() { WorkerReportsProgress = true };
            worker.DoWork += (s, e) =>
            {
                var symbols = QueryFactory.CompounderQuery.Run(CompounderQueryParams);
                (s as BackgroundWorker).ReportProgress(100, symbols);
            };
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += (s, e) =>
            {
                CurrentAction += $" {ResultSetList.Count()} companies found.";
                UnlockControls();
            };
            worker.RunWorkerAsync();

        }

        /// <summary>
        /// Worker_ProgressChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ResultSetList = (List<ResultSet>)e.UserState;

        }

        /// <summary>
        /// CompounderQuery_DatabaseAction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompounderQuery_DatabaseAction(object sender, DatabaseActionEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressCurrent = e.ProgressValue;
                CurrentAction = e.Action;
            });
        }

        /// <summary>
        /// LockControls
        /// </summary>
        private void LockControls()
        {
            ProgressCurrent = 0;
            Symbols = new List<string>();
            BackgroundResults = Brushes.DarkGray;
        }

        /// <summary>
        /// UnlockControls
        /// </summary>
        private void UnlockControls()
        {
            BackgroundResults = Brushes.White;
        }
    }
}
