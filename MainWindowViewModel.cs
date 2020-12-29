using FmpAnalyzer.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FmpAnalyzer
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : DependencyObject
    {
        public static readonly DependencyProperty ConnectionStringProperty;
        public static readonly DependencyProperty ResultsProperty;
        public static readonly DependencyProperty RoeFilterProperty;
        public static readonly DependencyProperty CurrentActionProperty;
        public static readonly DependencyProperty StableRowGrowthProperty;
        public static readonly DependencyProperty HistoryDepthProperty;
        public static readonly DependencyProperty GrowthGradProperty;
        public static readonly DependencyProperty ProgressMaxProperty;
        public static readonly DependencyProperty ProgressCurrentProperty;
        public RelayCommand CommandGo { get; set; }

        static MainWindowViewModel()
        {
            ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            ResultsProperty = DependencyProperty.Register("Results", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            RoeFilterProperty = DependencyProperty.Register("RoeFilter", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(Double)));
            CurrentActionProperty = DependencyProperty.Register("CurrentAction", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            StableRowGrowthProperty = DependencyProperty.Register("StableRowGrowth", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(default(Boolean)));
            HistoryDepthProperty = DependencyProperty.Register("HistoryDepth", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            GrowthGradProperty = DependencyProperty.Register("GrowthGrad", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            ProgressMaxProperty = DependencyProperty.Register("ProgressMax", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            ProgressCurrentProperty = DependencyProperty.Register("ProgressCurrent", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
        }

        public MainWindowViewModel()
        {
            ConnectionString = Configuration.Instance["ConnectionString"];
            RoeFilter = 15;
            CurrentAction = "Willkommen!";
            StableRowGrowth = true;
            HistoryDepth = 5;
            GrowthGrad = 3;

            CommandGo = new RelayCommand(async p => { await OnCommandGoAsync(p); });
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
        /// CurrentAction
        /// </summary>
        public string CurrentAction
        {
            get { return (string)GetValue(CurrentActionProperty); }
            set { SetValue(CurrentActionProperty, value); }
        }

        /// <summary>
        /// StableRowGrowth
        /// </summary>
        public bool StableRowGrowth
        {
            get { return (bool)GetValue(StableRowGrowthProperty); }
            set { SetValue(StableRowGrowthProperty, value); }
        }

        /// <summary>
        /// HistoryDepth
        /// </summary>
        public int HistoryDepth
        {
            get { return (int)GetValue(HistoryDepthProperty); }
            set { SetValue(HistoryDepthProperty, value); }
        }

        /// <summary>
        /// GrowthGrad
        /// </summary>
        public int GrowthGrad
        {
            get { return (int)GetValue(GrowthGradProperty); }
            set { SetValue(GrowthGradProperty, value); }
        }

        /// <summary>
        /// ProgressMax
        /// </summary>
        public int ProgressMax
        {
            get { return (int)GetValue(ProgressMaxProperty); }
            set { SetValue(ProgressMaxProperty, value); }
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
        /// OnCommandGo
        /// </summary>
        /// <param name="p"></param>
        private async Task OnCommandGoAsync(object p)
        {
            QueryFactory.CompounderQuery.DatabaseAction += (s, e) => 
            { 
                CurrentAction = e.Action;
                ProgressMax = e.MaxValue;
                ProgressCurrent = e.ProgressValue;
            };

            var historyDepth = StableRowGrowth ? HistoryDepth : 0;
            var growthGrad = StableRowGrowth ? GrowthGrad : 0;
            var symbols = await QueryFactory.CompounderQuery.Run("2019-12-31", RoeFilter, historyDepth, growthGrad);

            Dispatcher.Invoke(() =>
             {
                 Results = $"Found {symbols.Count()} companies:";
                 symbols.ForEach(s => Results += $"\r\n{s}");
             });
        }

    }
}
