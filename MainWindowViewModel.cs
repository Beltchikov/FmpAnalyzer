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
        public static readonly DependencyProperty ProgressCurrentProperty;
        public static readonly DependencyProperty BackgroundResultsProperty;
        public static readonly DependencyProperty ResultSetListProperty;
        public static readonly DependencyProperty ReinvestmentRateProperty;
        public static readonly DependencyProperty SelectedSymbolProperty;
        public static readonly DependencyProperty YearFromProperty;
        public static readonly DependencyProperty YearToProperty;
        public static readonly DependencyProperty CountMessageProperty;
        public static readonly DependencyProperty CountFilteredMessageProperty;
        public static readonly DependencyProperty ShowButtonEnabledProperty;

        public RelayCommand CommandGo { get; set; }
        public RelayCommand CommandCount { get; set; }

        static MainWindowViewModel()
        {
            ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            RoeProperty = DependencyProperty.Register("Roe", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(Double)));
            CurrentActionProperty = DependencyProperty.Register("CurrentAction", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            ProgressCurrentProperty = DependencyProperty.Register("ProgressCurrent", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            BackgroundResultsProperty = DependencyProperty.Register("BackgroundResults", typeof(Brush), typeof(MainWindowViewModel), new PropertyMetadata(default(Brush)));
            ResultSetListProperty = DependencyProperty.Register("ResultSetList", typeof(List<ResultSet>), typeof(MainWindowViewModel), new PropertyMetadata(new List<ResultSet>()));
            ReinvestmentRateProperty = DependencyProperty.Register("ReinvestmentRate", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(double)));
            SelectedSymbolProperty = DependencyProperty.Register("SelectedSymbol", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            YearFromProperty = DependencyProperty.Register("YearFrom", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0, YearFromChanged));
            YearToProperty = DependencyProperty.Register("YearTo", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0, YearToChanged));
            CountMessageProperty = DependencyProperty.Register("CountMessage", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            CountFilteredMessageProperty = DependencyProperty.Register("CountFilteredMessage", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            ShowButtonEnabledProperty = DependencyProperty.Register("ShowButtonEnabled", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(false));
        }

        public MainWindowViewModel()
        {
            ConnectionString = Configuration.Instance["ConnectionString"];
            Roe = 15;
            CurrentAction = "Willkommen!";
            ReinvestmentRate = 50;
            YearFrom = 2019;
            YearTo = 2020;
            GenerateCountMessage();

            CommandGo = new RelayCommand(p => { OnCommandGo(p); });
            CommandCount = new RelayCommand(p => { OnCommandCount(p); });
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
        /// ProgressCurrent
        /// </summary>
        public int ProgressCurrent
        {
            get { return (int)GetValue(ProgressCurrentProperty); }
            set { SetValue(ProgressCurrentProperty, value); }
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
        /// ResultSetList
        /// </summary>
        public List<ResultSet> ResultSetList
        {
            get { return (List<ResultSet>)GetValue(ResultSetListProperty); }
            set { SetValue(ResultSetListProperty, value); }
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
        /// SelectedSymbol
        /// </summary>
        public string SelectedSymbol
        {
            get { return (string)GetValue(SelectedSymbolProperty); }
            set { SetValue(SelectedSymbolProperty, value); }
        }

        /// <summary>
        /// YearFrom
        /// </summary>
        public int YearFrom
        {
            get { return (int)GetValue(YearFromProperty); }
            set { SetValue(YearFromProperty, value); }
        }

        /// <summary>
        /// YearTo
        /// </summary>
        public int YearTo
        {
            get { return (int)GetValue(YearToProperty); }
            set { SetValue(YearToProperty, value); }
        }

        /// <summary>
        /// CountMessage
        /// </summary>
        public string CountMessage
        {
            get { return (string)GetValue(CountMessageProperty); }
            set { SetValue(CountMessageProperty, value); }
        }

        /// <summary>
        /// CountFilteredMessage
        /// </summary>
        public string CountFilteredMessage
        {
            get { return (string)GetValue(CountFilteredMessageProperty); }
            set { SetValue(CountFilteredMessageProperty, value); }
        }

        /// <summary>
        /// ShowButtonEnabled
        /// </summary>
        public bool ShowButtonEnabled
        {
            get { return (bool)GetValue(ShowButtonEnabledProperty); }
            set { SetValue(ShowButtonEnabledProperty, value); }
        }

        /// <summary>
        /// GenerateCountMessage
        /// </summary>
        /// <returns></returns>
        private void GenerateCountMessage()
        {
            LockControls();
            var dates = Configuration.Instance["Dates"].Split(",").Select(d => d.Trim()).ToList();
            int yearFrom = YearFrom;
            int yearTo = YearTo;

            int count = 0;
            BackgroundWork((s, e) =>
            {
                var count = QueryFactory.CountByYearsQuery.Run(yearFrom, yearTo, dates);
                (s as BackgroundWorker).ReportProgress(100, count);
            }, (s, e) =>
            {
                count = (int)e.UserState;
            }, (s, e) =>
            {
                CountMessage = $"{count} companies in database for the period {yearFrom} - {yearTo}.";
                UnlockControls();
            });
        }

        /// <summary>
        /// BackgroundWork
        /// </summary>
        /// <param name="doWorkEventHandler"></param>
        /// <param name="progressChangedEventHandler"></param>
        /// <param name="runWorkerCompletedEventHandler"></param>
        private void BackgroundWork(DoWorkEventHandler doWorkEventHandler, ProgressChangedEventHandler progressChangedEventHandler, RunWorkerCompletedEventHandler runWorkerCompletedEventHandler)
        {
            var worker = new BackgroundWorker() { WorkerReportsProgress = true };
            worker.DoWork += doWorkEventHandler;
            worker.ProgressChanged += progressChangedEventHandler;
            worker.RunWorkerCompleted += runWorkerCompletedEventHandler;
            worker.RunWorkerAsync();
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
                YearFrom = YearFrom,
                YearTo = YearTo,
                Dates = Configuration.Instance["Dates"].Split(",").Select(d => d.Trim()).ToList(),
                Roe = Roe,
                ReinvestmentRate = ReinvestmentRate,
                HistoryDepth = Convert.ToInt32(Configuration.Instance["HistoryDepth"]),
                Symbol = SelectedSymbol
            };

            BackgroundWork((s, e) =>
            {
                var symbols = QueryFactory.CompounderQuery.Run(CompounderQueryParams);
                (s as BackgroundWorker).ReportProgress(100, symbols);
            }, (s, e) =>
            {
                ResultSetList = (List<ResultSet>)e.UserState;
            }, (s, e) =>
            {
                CurrentAction += $" {ResultSetList.Count()} companies found.";
                UnlockControls();
            });
        }

        /// <summary>
        /// OnCommandCount
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandCount(object p)
        {
            LockControls();

            CompounderQueryParams = new CompounderQueryParams
            {
                YearFrom = YearFrom,
                YearTo = YearTo,
                Dates = Configuration.Instance["Dates"].Split(",").Select(d => d.Trim()).ToList(),
                Roe = Roe,
                ReinvestmentRate = ReinvestmentRate,
                HistoryDepth = Convert.ToInt32(Configuration.Instance["HistoryDepth"]),
                Symbol = SelectedSymbol
            };

            BackgroundWork((s, e) =>
            {
                var count = QueryFactory.CompounderQuery.Count(CompounderQueryParams);
                (s as BackgroundWorker).ReportProgress(100, count);
            }, (s, e) =>
            {
                var cnt = (int)(e.UserState);
                ShowButtonEnabled = cnt <= Convert.ToInt32(Configuration.Instance["MaxCountToShow"]);
                CountFilteredMessage = $"{e.UserState} companies filtered";
            }, (s, e) =>
            {
                UnlockControls();
            });
        }

        /// <summary>
        /// YearFromChanged
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void YearFromChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MainWindowViewModel instance = d as MainWindowViewModel;
            if (instance == null)
            {
                return;
            }

            if (instance.YearFrom.ToString().Length != 4)
            {
                return;
            }

            instance.GenerateCountMessage();
        }

        /// <summary>
        /// YearToChanged
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void YearToChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MainWindowViewModel instance = d as MainWindowViewModel;
            if (instance == null)
            {
                return;
            }

            if (instance.YearTo.ToString().Length != 4)
            {
                return;
            }

            instance.GenerateCountMessage();
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
