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
        public RelayCommand CommandGo { get; set; }
        public RelayCommand CommandCount { get; set; }
        public RelayCommand CommandFirst { get; set; }
        public RelayCommand CommandPrevious { get; set; }
        public RelayCommand CommandNext { get; set; }
        public RelayCommand CommandLast { get; set; }

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
        public static readonly DependencyProperty FirstButtonEnabledProperty;
        public static readonly DependencyProperty PreviousButtonEnabledProperty;
        public static readonly DependencyProperty NextButtonEnabledProperty;
        public static readonly DependencyProperty LastButtonEnabledProperty;
        public static readonly DependencyProperty SortByListProperty;
        public static readonly DependencyProperty SortBySelectedProperty;
        public static readonly DependencyProperty PageSizeListProperty;
        public static readonly DependencyProperty PageSizeSelectedProperty;

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
            FirstButtonEnabledProperty = DependencyProperty.Register("FirstButtonEnabled", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(false));
            PreviousButtonEnabledProperty = DependencyProperty.Register("PreviousButtonEnabled", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(false));
            NextButtonEnabledProperty = DependencyProperty.Register("NextButtonEnabled", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(false));
            LastButtonEnabledProperty = DependencyProperty.Register("LastButtonEnabled", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(false));
            SortByListProperty = DependencyProperty.Register("SortByList", typeof(List<SortBy>), typeof(MainWindowViewModel), new PropertyMetadata(new List<SortBy>()));
            SortBySelectedProperty = DependencyProperty.Register("SortBySelected", typeof(SortBy), typeof(MainWindowViewModel), new PropertyMetadata(default(SortBy)));
            PageSizeListProperty = DependencyProperty.Register("PageSizeList", typeof(List<int>), typeof(MainWindowViewModel), new PropertyMetadata(new List<int>()));
            PageSizeSelectedProperty = DependencyProperty.Register("PageSizeSelected", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
        }

        public MainWindowViewModel()
        {
            ConnectionString = Configuration.Instance["ConnectionString"];
            Roe = 40;
            CurrentAction = "Willkommen!";
            ReinvestmentRate = 50;
            YearFrom = 2019;
            YearTo = 2020;
            GenerateCountMessage();
            InitComboboxes();

            CommandGo = new RelayCommand(p => { OnCommandGo(p); });
            CommandCount = new RelayCommand(p => { OnCommandCount(p); });
            CommandFirst = new RelayCommand(p => { OnCommandFirst(p); });
            CommandPrevious = new RelayCommand(p => { OnCommandPrevious(p); });
            CommandNext = new RelayCommand(p => { OnCommandCommandNext(p); });
            CommandLast = new RelayCommand(p => { OnCommandLast(p); });

            QueryFactory.CompounderQuery.DatabaseAction += CompounderQuery_DatabaseAction;
        }

        #region Properties

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
        /// ResultSetList
        /// </summary>
        public List<ResultSet> ResultSetList
        {
            get { return (List<ResultSet>)GetValue(ResultSetListProperty); }
            set { SetValue(ResultSetListProperty, value); }
        }

        /// <summary>
        /// CountTotal
        /// </summary>
        public int CountTotal { get; private set; }

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
        /// FirstButtonEnabled
        /// </summary>
        public bool FirstButtonEnabled
        {
            get { return (bool)GetValue(FirstButtonEnabledProperty); }
            set { SetValue(FirstButtonEnabledProperty, value); }
        }

        /// <summary>
        /// PreviousButtonEnabled
        /// </summary>
        public bool PreviousButtonEnabled
        {
            get { return (bool)GetValue(PreviousButtonEnabledProperty); }
            set { SetValue(PreviousButtonEnabledProperty, value); }
        }

        /// <summary>
        /// NextButtonEnabled
        /// </summary>
        public bool NextButtonEnabled
        {
            get { return (bool)GetValue(NextButtonEnabledProperty); }
            set { SetValue(NextButtonEnabledProperty, value); }
        }

        /// <summary>
        /// LastButtonEnabled
        /// </summary>
        public bool LastButtonEnabled
        {
            get { return (bool)GetValue(LastButtonEnabledProperty); }
            set { SetValue(LastButtonEnabledProperty, value); }
        }

        /// <summary>
        /// SortByList
        /// </summary>
        public List<SortBy> SortByList
        {
            get { return (List<SortBy>)GetValue(SortByListProperty); }
            set { SetValue(SortByListProperty, value); }
        }

        /// <summary>
        /// SortBySelected
        /// </summary>
        public SortBy SortBySelected
        {
            get { return (SortBy)GetValue(SortBySelectedProperty); }
            set { SetValue(SortBySelectedProperty, value); }
        }


        /// <summary>
        /// PageSizeList
        /// </summary>
        public List<int> PageSizeList
        {
            get { return (List<int>)GetValue(PageSizeListProperty); }
            set { SetValue(PageSizeListProperty, value); }
        }

        /// <summary>
        /// PageSizeSelected
        /// </summary>
        public int PageSizeSelected
        {
            get { return (int)GetValue(PageSizeSelectedProperty); }
            set { SetValue(PageSizeSelectedProperty, value); }
        }

        /// <summary>
        /// CurrentPage
        /// </summary>
        public int CurrentPage { get; private set; }

        #endregion

        #region Commands

        /// <summary>
        /// OnCommandGo
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandGo(object p)
        {
            LockControls();

            var compounderQueryParams = new CompounderQueryParams<object>
            {
                YearFrom = YearFrom,
                YearTo = YearTo,
                Dates = Configuration.Instance["Dates"].Split(",").Select(d => d.Trim()).ToList(),
                Roe = Roe,
                ReinvestmentRate = ReinvestmentRate,
                HistoryDepth = Convert.ToInt32(Configuration.Instance["HistoryDepth"]),
                Symbol = SelectedSymbol,
                OrderFunction = SortBySelected.Function,
                Descending = SortBySelected.Descending,
                PageSize = PageSizeSelected,
                CurrentPage = CurrentPage
            };

            BackgroundWork((s, e) =>
            {
                var symbols = QueryFactory.CompounderQuery.Run(compounderQueryParams);
                (s as BackgroundWorker).ReportProgress(100, symbols);
            }, (s, e) =>
            {
                ResultSetList = ((ResultSetList)e.UserState).ResultSets;
                CountTotal = ((ResultSetList)e.UserState).CountTotal;
            }, (s, e) =>
            {
                CurrentAction += $" {CountTotal} companies found.";
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

            var compounderCountQueryParams = new CompounderCountQueryParams
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
                var count = QueryFactory.CompounderQuery.Count(compounderCountQueryParams);
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
        /// OnCommandLast
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandLast(object p)
        {
            // TODO
        }

        /// <summary>
        /// OnCommandCommandNext
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandCommandNext(object p)
        {
            // TODO
        }

        /// <summary>
        /// OnCommandPrevious
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandPrevious(object p)
        {
            // TODO
        }

        /// <summary>
        /// OnCommandFirst
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandFirst(object p)
        {
            // TODO
        }

        #endregion

        #region Private

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
        /// InitComboboxes
        /// </summary>
        private void InitComboboxes()
        {
            // SortByList
            SortByList = new List<SortBy>
            {
                new SortBy
                {
                    Text = "ROE Desc",
                    Descending = true,
                    Function = (r) => r.Roe
                },
                new SortBy
                {
                    Text = "ROE Asc",
                    Descending = false,
                    Function = (r) => r.Roe
                }

            };

            SortBySelected = SortByList[0];

            // PageSizeList
            PageSizeList = new List<int> { 10, 20 };
            PageSizeSelected = PageSizeList[1];
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
            ResultSetList = new List<ResultSet>();
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

        #endregion
    }
}
