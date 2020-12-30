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
        public static readonly DependencyProperty RoeYearProperty;
        public static readonly DependencyProperty ResultSetListProperty;
        public static readonly DependencyProperty ReinvestmentRateProperty;

        public RelayCommand CommandGo { get; set; }

        static MainWindowViewModel()
        {
            ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            RoeProperty = DependencyProperty.Register("Roe", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(Double)));
            CurrentActionProperty = DependencyProperty.Register("CurrentAction", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            ProgressCurrentProperty = DependencyProperty.Register("ProgressCurrent", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            BackgroundResultsProperty = DependencyProperty.Register("BackgroundResults", typeof(Brush), typeof(MainWindowViewModel), new PropertyMetadata(default(Brush)));
            RoeYearProperty = DependencyProperty.Register("RoeYear", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(string.Empty));
            ResultSetListProperty = DependencyProperty.Register("ResultSetList", typeof(List<ResultSet>), typeof(MainWindowViewModel), new PropertyMetadata(new List<ResultSet>()));
            ReinvestmentRateProperty = DependencyProperty.Register("ReinvestmentRate", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(double)));
        }

        public MainWindowViewModel()
        {
            ConnectionString = Configuration.Instance["ConnectionString"];
            Roe = 15;
            CurrentAction = "Willkommen!";
            RoeYear = "2019";
            ReinvestmentRate = 50;

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
                ReinvestmentRate = ReinvestmentRate
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
