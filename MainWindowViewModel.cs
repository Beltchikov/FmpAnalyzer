using FmpDataContext.Model;
using FmpDataContext.Queries;
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
        QueryFactory _queryFactory;

        public RelayCommand CommandGo { get; set; }
        public RelayCommand CommandCount { get; set; }
        public RelayCommand CommandFirst { get; set; }
        public RelayCommand CommandPrevious { get; set; }
        public RelayCommand CommandNext { get; set; }
        public RelayCommand CommandLast { get; set; }
        public RelayCommand CommandFind { get; set; }
        public RelayCommand CommandEarnings { get; set; }
        public RelayCommand CommandFindByCompany { get; set; }


        public static readonly DependencyProperty ConnectionStringProperty;
        public static readonly DependencyProperty RoeFromProperty;
        public static readonly DependencyProperty CurrentActionProperty;
        public static readonly DependencyProperty ProgressCurrentProperty;
        public static readonly DependencyProperty BackgroundResultsProperty;
        public static readonly DependencyProperty ListOfResultSetsProperty;
        public static readonly DependencyProperty ReinvestmentRateFromProperty;
        public static readonly DependencyProperty SelectedSymbolProperty;
        public static readonly DependencyProperty YearFromProperty;
        public static readonly DependencyProperty YearToProperty;
        public static readonly DependencyProperty CountMessageProperty;
        public static readonly DependencyProperty CountFilteredMessageProperty;
        public static readonly DependencyProperty FirstButtonEnabledProperty;
        public static readonly DependencyProperty PreviousButtonEnabledProperty;
        public static readonly DependencyProperty NextButtonEnabledProperty;
        public static readonly DependencyProperty LastButtonEnabledProperty;
        public static readonly DependencyProperty SortByListProperty;
        public static readonly DependencyProperty SortBySelectedProperty;
        public static readonly DependencyProperty PageSizeListProperty;
        public static readonly DependencyProperty PageSizeSelectedProperty;
        public static readonly DependencyProperty RoeGrowthKoefListProperty;
        public static readonly DependencyProperty RoeGrowthKoefSelectedProperty;
        public static readonly DependencyProperty SymbolProperty;
        public static readonly DependencyProperty SymbolResultSetListProperty;
        public static readonly DependencyProperty RoeToProperty;
        public static readonly DependencyProperty ReinvestmentRateToProperty;
        public static readonly DependencyProperty RevenueGrowthKoefListProperty;
        public static readonly DependencyProperty EpsGrowthKoefListProperty;
        public static readonly DependencyProperty RevenueGrowthKoefSelectedProperty;
        public static readonly DependencyProperty EpsGrowthKoefSelectedProperty;
        public static readonly DependencyProperty DebtToEquityFromProperty;
        public static readonly DependencyProperty DebtToEquityToProperty;
        public static readonly DependencyProperty CompanyProperty;
        public static readonly DependencyProperty SymbolsFoundProperty;
        public static readonly DependencyProperty ExchangesProperty;
        public static readonly DependencyProperty CompaniesEarningsProperty;
        public static readonly DependencyProperty CompaniesEarningsNotProcessedProperty;
        public static readonly DependencyProperty EarningsResultSetListProperty;

        static MainWindowViewModel()
        {
            ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            RoeFromProperty = DependencyProperty.Register("RoeFrom", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(Double)));
            CurrentActionProperty = DependencyProperty.Register("CurrentAction", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            ProgressCurrentProperty = DependencyProperty.Register("ProgressCurrent", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            BackgroundResultsProperty = DependencyProperty.Register("BackgroundResults", typeof(Brush), typeof(MainWindowViewModel), new PropertyMetadata(default(Brush)));
            ListOfResultSetsProperty = DependencyProperty.Register("ListOfResultSets", typeof(List<ResultSetReinvestment>), typeof(MainWindowViewModel), new PropertyMetadata(new List<ResultSetReinvestment>()));
            ReinvestmentRateFromProperty = DependencyProperty.Register("ReinvestmentRateFrom", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(double)));
            SelectedSymbolProperty = DependencyProperty.Register("SelectedSymbol", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            YearFromProperty = DependencyProperty.Register("YearFrom", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0, YearFromChanged));
            YearToProperty = DependencyProperty.Register("YearTo", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0, YearToChanged));
            CountMessageProperty = DependencyProperty.Register("CountMessage", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            CountFilteredMessageProperty = DependencyProperty.Register("CountFilteredMessage", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            FirstButtonEnabledProperty = DependencyProperty.Register("FirstButtonEnabled", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(false));
            PreviousButtonEnabledProperty = DependencyProperty.Register("PreviousButtonEnabled", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(false));
            NextButtonEnabledProperty = DependencyProperty.Register("NextButtonEnabled", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(false));
            LastButtonEnabledProperty = DependencyProperty.Register("LastButtonEnabled", typeof(bool), typeof(MainWindowViewModel), new PropertyMetadata(false));
            SortByListProperty = DependencyProperty.Register("SortByList", typeof(List<SortBy>), typeof(MainWindowViewModel), new PropertyMetadata(new List<SortBy>()));
            SortBySelectedProperty = DependencyProperty.Register("SortBySelected", typeof(SortBy), typeof(MainWindowViewModel), new PropertyMetadata(default(SortBy)));
            PageSizeListProperty = DependencyProperty.Register("PageSizeList", typeof(List<int>), typeof(MainWindowViewModel), new PropertyMetadata(new List<int>()));
            PageSizeSelectedProperty = DependencyProperty.Register("PageSizeSelected", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            RoeGrowthKoefListProperty = DependencyProperty.Register("RoeGrowthKoefList", typeof(List<int>), typeof(MainWindowViewModel), new PropertyMetadata(new List<int>()));
            RoeGrowthKoefSelectedProperty = DependencyProperty.Register("RoeGrowthKoefSelected", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            SymbolProperty = DependencyProperty.Register("Symbol", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            SymbolResultSetListProperty = DependencyProperty.Register("SymbolResultSetList", typeof(List<ResultSetReinvestment>), typeof(MainWindowViewModel), new PropertyMetadata(new List<ResultSetReinvestment>()));
            RoeToProperty = DependencyProperty.Register("RoeTo", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(double)));
            ReinvestmentRateToProperty = DependencyProperty.Register("ReinvestmentRateTo", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(double)));
            RevenueGrowthKoefListProperty = DependencyProperty.Register("RevenueGrowthKoefList", typeof(List<int>), typeof(MainWindowViewModel), new PropertyMetadata(new List<int>()));
            EpsGrowthKoefListProperty = DependencyProperty.Register("EpsGrowthKoefList", typeof(List<int>), typeof(MainWindowViewModel), new PropertyMetadata(new List<int>()));
            RevenueGrowthKoefSelectedProperty = DependencyProperty.Register("RevenueGrowthKoefSelected", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            EpsGrowthKoefSelectedProperty = DependencyProperty.Register("EpsGrowthKoefSelected", typeof(int), typeof(MainWindowViewModel), new PropertyMetadata(0));
            DebtToEquityFromProperty = DependencyProperty.Register("DebtToEquityFrom", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(double)));
            DebtToEquityToProperty = DependencyProperty.Register("DebtToEquityTo", typeof(double), typeof(MainWindowViewModel), new PropertyMetadata(default(double)));
            CompanyProperty = DependencyProperty.Register("Company", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            SymbolsFoundProperty = DependencyProperty.Register("SymbolsFound", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            ExchangesProperty = DependencyProperty.Register("Exchanges", typeof(List<Exchange>), typeof(MainWindowViewModel), new PropertyMetadata(new List<Exchange>()));
            CompaniesEarningsProperty = DependencyProperty.Register("CompaniesEarnings", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            CompaniesEarningsNotProcessedProperty = DependencyProperty.Register("CompaniesEarningsNotProcessed", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
            EarningsResultSetListProperty = DependencyProperty.Register("EarningsResultSetList", typeof(List<ResultSetReinvestment>), typeof(MainWindowViewModel), new PropertyMetadata(new List<ResultSetReinvestment>()));
        }

        public MainWindowViewModel()
        {
            ConnectionString = Configuration.Instance["ConnectionString"];
            _queryFactory = new QueryFactory(ConnectionString);
            RoeFrom = 30;
            CurrentAction = "Willkommen!";
            ReinvestmentRateFrom = 50;
            YearFrom = 2019;
            YearTo = 2020;
            DebtToEquityTo = 2.0;
            GenerateCountMessage();
            InitComboboxes();

            CommandGo = new RelayCommand(p => { OnCommandGo(p); });
            CommandCount = new RelayCommand(p => { OnCommandCount(p); });
            CommandFirst = new RelayCommand(p => { OnCommandFirst(p); });
            CommandPrevious = new RelayCommand(p => { OnCommandPrevious(p); });
            CommandNext = new RelayCommand(p => { OnCommandCommandNext(p); });
            CommandLast = new RelayCommand(p => { OnCommandLast(p); });
            CommandFind = new RelayCommand(p => { OnCommandFindBySymbol(p); });
            CommandEarnings = new RelayCommand(p => { OnCommandEarnings(p); });
            CommandFindByCompany = new RelayCommand(p => { OnCommandFindByCompany(p); });
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
        /// RoeFrom
        /// </summary>
        public double RoeFrom
        {
            get { return (double)GetValue(RoeFromProperty); }
            set { SetValue(RoeFromProperty, value); }
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
        /// ListOfResultSets
        /// </summary>
        public List<ResultSetReinvestment> ListOfResultSets
        {
            get { return (List<ResultSetReinvestment>)GetValue(ListOfResultSetsProperty); }
            set { SetValue(ListOfResultSetsProperty, value); }
        }

        /// <summary>
        /// SymbolResultSetList
        /// </summary>
        public List<ResultSetReinvestment> SymbolResultSetList
        {
            get { return (List<ResultSetReinvestment>)GetValue(SymbolResultSetListProperty); }
            set { SetValue(SymbolResultSetListProperty, value); }
        }

        /// <summary>
        /// EarningsResultSetList
        /// </summary>
        public List<ResultSetReinvestment> EarningsResultSetList
        {
            get { return (List<ResultSetReinvestment>)GetValue(EarningsResultSetListProperty); }
            set { SetValue(EarningsResultSetListProperty, value); }
        }

        /// <summary>
        /// CountTotal
        /// </summary>
        public int CountTotal { get; private set; }

        /// <summary>
        /// ReinvestmentRateFrom
        /// </summary>
        public double ReinvestmentRateFrom
        {
            get { return (double)GetValue(ReinvestmentRateFromProperty); }
            set { SetValue(ReinvestmentRateFromProperty, value); }
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

        /// <summary>
        /// ShowingCompanyFrom
        /// </summary>
        public int ShowingCompanyFrom
        {
            get
            {
                return CurrentPage * PageSizeSelected + 1;
            }
        }

        /// <summary>
        /// ShowingCompanyTo
        /// </summary>
        public int ShowingCompanyTo
        {
            get
            {
                return Math.Min((CurrentPage + 1) * PageSizeSelected, CountTotal);
            }
        }

        /// <summary>
        /// RoeGrowthKoefList
        /// </summary>
        public List<int> RoeGrowthKoefList
        {
            get { return (List<int>)GetValue(RoeGrowthKoefListProperty); }
            set { SetValue(RoeGrowthKoefListProperty, value); }
        }

        /// <summary>
        /// RoeGrowthKoefSelected
        /// </summary>
        public int RoeGrowthKoefSelected
        {
            get { return (int)GetValue(RoeGrowthKoefSelectedProperty); }
            set { SetValue(RoeGrowthKoefSelectedProperty, value); }
        }

        /// <summary>
        /// RevenueGrowthKoefList
        /// </summary>
        public List<int> RevenueGrowthKoefList
        {
            get { return (List<int>)GetValue(RevenueGrowthKoefListProperty); }
            set { SetValue(RevenueGrowthKoefListProperty, value); }
        }

        /// <summary>
        /// EpsGrowthKoefList
        /// </summary>
        public List<int> EpsGrowthKoefList
        {
            get { return (List<int>)GetValue(EpsGrowthKoefListProperty); }
            set { SetValue(EpsGrowthKoefListProperty, value); }
        }

        /// <summary>
        /// RevenueGrowthKoefSelected
        /// </summary>
        public int RevenueGrowthKoefSelected
        {
            get { return (int)GetValue(RevenueGrowthKoefSelectedProperty); }
            set { SetValue(RevenueGrowthKoefSelectedProperty, value); }
        }

        /// <summary>
        /// EpsGrowthKoefSelected
        /// </summary>
        public int EpsGrowthKoefSelected
        {
            get { return (int)GetValue(EpsGrowthKoefSelectedProperty); }
            set { SetValue(EpsGrowthKoefSelectedProperty, value); }
        }

        /// <summary>
        /// Symbol
        /// </summary>
        public string Symbol
        {
            get { return (string)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        /// <summary>
        /// RoeTo
        /// </summary>
        public double RoeTo
        {
            get { return (double)GetValue(RoeToProperty); }
            set { SetValue(RoeToProperty, value); }
        }

        /// <summary>
        /// ReinvestmentRateTo
        /// </summary>
        public double ReinvestmentRateTo
        {
            get { return (double)GetValue(ReinvestmentRateToProperty); }
            set { SetValue(ReinvestmentRateToProperty, value); }
        }

        /// <summary>
        /// DebtToEquityFrom
        /// </summary>
        public double DebtToEquityFrom
        {
            get { return (double)GetValue(DebtToEquityFromProperty); }
            set { SetValue(DebtToEquityFromProperty, value); }
        }

        /// <summary>
        /// DebtToEquityTo
        /// </summary>
        public double DebtToEquityTo
        {
            get { return (double)GetValue(DebtToEquityToProperty); }
            set { SetValue(DebtToEquityToProperty, value); }
        }

        /// <summary>
        /// SymbolsFound
        /// </summary>
        public string SymbolsFound
        {
            get { return (string)GetValue(SymbolsFoundProperty); }
            set { SetValue(SymbolsFoundProperty, value); }
        }

        public string CompaniesEarnings
        {
            get { return (string)GetValue(CompaniesEarningsProperty); }
            set { SetValue(CompaniesEarningsProperty, value); }
        }

        public string CompaniesEarningsNotProcessed
        {
            get { return (string)GetValue(CompaniesEarningsNotProcessedProperty); }
            set { SetValue(CompaniesEarningsNotProcessedProperty, value); }
        }

        /// <summary>
        /// Exchanges
        /// </summary>
        public List<Exchange> Exchanges
        {
            get { return (List<Exchange>)GetValue(ExchangesProperty); }
            set { SetValue(ExchangesProperty, value); }
        }

        /// <summary>
        /// Company
        /// </summary>
        public string Company
        {
            get { return (string)GetValue(CompanyProperty); }
            set { SetValue(CompanyProperty, value); }
        }

        /// <summary>
        /// ResultSetList
        /// </summary>
        public ResultSetListReinvestment ResultSetList { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// OnCommandGo
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandGo(object p)
        {
            LockControls();

            var compounderQueryParams = new CompounderQueryParams
            {
                YearFrom = YearFrom,
                YearTo = YearTo,
                Dates = Configuration.Instance["Dates"].Split(",").Select(d => d.Trim()).ToList(),
                RoeFrom = RoeFrom == 0 ? null : RoeFrom,
                RoeTo = RoeTo == 0 ? null : RoeTo,
                ReinvestmentRateFrom = ReinvestmentRateFrom == 0 ? null : ReinvestmentRateFrom,
                ReinvestmentRateTo = ReinvestmentRateTo == 0 ? null : ReinvestmentRateTo,
                HistoryDepth = Convert.ToInt32(Configuration.Instance["HistoryDepth"]),
                RoeGrowthKoef = RoeGrowthKoefSelected == 0 ? null : RoeGrowthKoefSelected,
                RevenueGrowthKoef = RevenueGrowthKoefSelected == 0 ? null : RevenueGrowthKoefSelected,
                EpsGrowthKoef = EpsGrowthKoefSelected == 0 ? null : EpsGrowthKoefSelected,
                OrderBy = SortBySelected.Value,
                Descending = SortBySelected.Descending,
                PageSize = PageSizeSelected,
                CurrentPage = CurrentPage,
                DebtEquityRatioFrom = DebtToEquityFrom == 0 ? null : DebtToEquityFrom,
                DebtEquityRatioTo = DebtToEquityTo == 0 ? null : DebtToEquityTo,
                Exchanges = Exchanges,
                MaxResultCount = Convert.ToInt32(Configuration.Instance["MaxResultCount"])
            };

            BackgroundWork((s, e) =>
            {
                var symbols = _queryFactory.CompounderQuery.Run(compounderQueryParams);
                (s as BackgroundWorker).ReportProgress(100, symbols);
            }, (s, e) =>
            {
                ResultSetList = (ResultSetListReinvestment)e.UserState;
                ListOfResultSets = ResultSetList.ResultSets;
                CountTotal = ResultSetList.CountTotal;
            }, (s, e) =>
            {
                CurrentAction = GenerateCurrentActionMessage(ResultSetList);
                UnlockControls();
                UpdatePageButtons();
            });
        }

        /// <summary>
        /// GenerateCurrentActionMessage
        /// </summary>
        /// <param name="resultSetList"></param>
        /// <returns></returns>
        private string GenerateCurrentActionMessage(ResultSetListReinvestment resultSetList)
        {
            if(resultSetList == null)
            {
                return string.Empty;
            }    
            
            if(resultSetList.ResultSets.Any())
            {
                return $"{CountTotal} companies found. Showing companies {ShowingCompanyFrom} - {ShowingCompanyTo}";
            }

            return $"Too many companies found ({CountTotal}). Please restrict your query.";
        }

        /// <summary>
        /// OnCommandCount
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandCount(object p)
        {
            LockControls();

            var compounderQueryParams = new CompounderQueryParams
            {
                YearFrom = YearFrom,
                YearTo = YearTo,
                Dates = Configuration.Instance["Dates"].Split(",").Select(d => d.Trim()).ToList(),
                RoeFrom = RoeFrom,
                ReinvestmentRateFrom = ReinvestmentRateFrom,
                HistoryDepth = Convert.ToInt32(Configuration.Instance["HistoryDepth"]),
                RoeGrowthKoef = RoeGrowthKoefSelected,
                RevenueGrowthKoef = RevenueGrowthKoefSelected,
                EpsGrowthKoef = EpsGrowthKoefSelected,
                DebtEquityRatioFrom = DebtToEquityFrom,
                DebtEquityRatioTo = DebtToEquityTo,
                Exchanges = Exchanges
            };

            BackgroundWork((s, e) =>
            {
                var count = _queryFactory.CompounderQuery.Count(compounderQueryParams);
                (s as BackgroundWorker).ReportProgress(100, count);
            }, (s, e) =>
            {
                var cnt = (int)(e.UserState);
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
            CurrentPage = CountTotal / PageSizeSelected;
            OnCommandGo(p);
        }

        /// <summary>
        /// OnCommandCommandNext
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandCommandNext(object p)
        {
            CurrentPage += 1;
            OnCommandGo(p);
        }

        /// <summary>
        /// OnCommandPrevious
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandPrevious(object p)
        {
            CurrentPage -= 1;
            OnCommandGo(p);
        }

        /// <summary>
        /// OnCommandFirst
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandFirst(object p)
        {
            CurrentPage = 0;
            OnCommandGo(p);
        }

        /// <summary>
        /// OnCommandFindBySymbol
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandFindBySymbol(object p)
        {
            var compounderQueryParams = new CompounderQueryParams
            {
                YearFrom = YearFrom,
                YearTo = YearTo,
                Dates = Configuration.Instance["Dates"].Split(",").Select(d => d.Trim()).ToList(),
                HistoryDepth = Convert.ToInt32(Configuration.Instance["HistoryDepth"])
            };

            var symbolList = new List<string> { Symbol };
            SymbolResultSetList = _queryFactory.CompounderQuery.FindBySymbol(compounderQueryParams, symbolList).ResultSets;
        }

        /// <summary>
        /// OnCommandFindByCompany
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandFindByCompany(object p)
        {
            var compounderQueryParams = new CompounderQueryParams
            {
                YearFrom = YearFrom,
                YearTo = YearTo,
                Dates = Configuration.Instance["Dates"].Split(",").Select(d => d.Trim()).ToList(),
                HistoryDepth = Convert.ToInt32(Configuration.Instance["HistoryDepth"])
            };

            var symbolsAsList = _queryFactory.SymbolByCompanyQuery.FindByCompany(Company);
            if (symbolsAsList.Any())
            {
                SymbolsFound = symbolsAsList.Aggregate((r, n) => r + "\r\n" + n);
                SymbolsFound = string.IsNullOrWhiteSpace(SymbolsFound) ? "No matches!" : SymbolsFound;
            }
            else
            {
                SymbolsFound = "No data found.";
            }
        }

        /// <summary>
        /// OnCommandEarnings
        /// </summary>
        /// <param name="p"></param>
        private void OnCommandEarnings(object p)
        {
            List<string> companiesNotResolved = new List<string>();
            List<string> symbolList = new List<string>();

            var listOfCompanies = CompaniesEarnings.Split("\r\n").ToList();
            listOfCompanies = listOfCompanies.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
            listOfCompanies = listOfCompanies.Select(c => c.Trim()).ToList();
            listOfCompanies = listOfCompanies.Select(c => c.Replace("'"," ")).ToList();

            foreach (var company in listOfCompanies)
            {
                var symbolsAsList = _queryFactory.SymbolByCompanyQuery.FindByCompany(company);
                if (!symbolsAsList.Any())
                {
                    companiesNotResolved.Add(company);
                }
                else
                {
                    var currentSymbols = symbolsAsList.Select(s => s.Split("\t")[0]);
                    symbolList.AddRange(currentSymbols);
                }
            }

            if (companiesNotResolved.Any())
            {
                CompaniesEarningsNotProcessed = companiesNotResolved.Aggregate((r, n) => r + "\r\n" + n);
            }

            if(!symbolList.Any())
            {
                return;
            }

            var compounderQueryParams = new CompounderQueryParams
            {
                YearFrom = YearFrom,
                YearTo = YearTo,
                Dates = Configuration.Instance["Dates"].Split(",").Select(d => d.Trim()).ToList(),
                HistoryDepth = Convert.ToInt32(Configuration.Instance["HistoryDepth"])
            };
            
            LockControls();

            BackgroundWork((s, e) =>
            {
                var resultSetList = _queryFactory.CompounderQuery.FindBySymbol(compounderQueryParams, symbolList);
                (s as BackgroundWorker).ReportProgress(100, resultSetList);
            }, (s, e) =>
            {
                EarningsResultSetList = ((ResultSetListReinvestment)e.UserState).ResultSets;
                CountTotal = ((ResultSetListReinvestment)e.UserState).CountTotal;
            }, (s, e) =>
            {
                UnlockControls();
            });
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
                var count = _queryFactory.CountByYearsQuery.Run(yearFrom, yearTo, dates);
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
                    Value = "Roe"
                },
                new SortBy
                {
                    Text = "ROE Asc",
                    Descending = false,
                    Value = "Roe"
                },
                new SortBy
                {
                    Text = "RR Asc",
                    Descending = false,
                    Value = "ReinvestmentRate"
                },
                new SortBy
                {
                    Text = "RR Desc",
                    Descending = true,
                    Value = "ReinvestmentRate"
                }
            };

            SortBySelected = SortByList[0];

            // PageSizeList
            PageSizeList = new List<int> { 10, 20 };
            PageSizeSelected = PageSizeList[1];

            // RoeGrowthKoefList
            RoeGrowthKoefList = new List<int> { 0, 2, 3, 4 };
            RoeGrowthKoefSelected = RoeGrowthKoefList[1];

            // RevenueGrowthKoefList
            RevenueGrowthKoefList = new List<int> { 0, 2, 3, 4 };
            RevenueGrowthKoefSelected = RevenueGrowthKoefList[0];

            // EpsGrowthKoefList
            EpsGrowthKoefList = new List<int> { 0, 2, 3, 4 };
            EpsGrowthKoefSelected = EpsGrowthKoefList[2];

            // Exchages
            Exchanges = new List<Exchange>()
            {
                Exchange.Nyse,
                Exchange.Nasdaq,
                Exchange.Lse,
                Exchange.Hkse,
                Exchange.Asx,
                Exchange.Nse,
                Exchange.Canada,
                Exchange.Europe
            };
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
        /// LockControls
        /// </summary>
        private void LockControls()
        {
            ListOfResultSets = new List<ResultSetReinvestment>();
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

        /// <summary>
        /// UpdatePageButtons
        /// </summary>
        private void UpdatePageButtons()
        {
            if (CountTotal < PageSizeSelected)
            {
                FirstButtonEnabled = false;
                PreviousButtonEnabled = false;
                NextButtonEnabled = false;
                LastButtonEnabled = false;
            }
            else if ((CurrentPage + 1) * PageSizeSelected >= CountTotal)
            {
                FirstButtonEnabled = true;
                PreviousButtonEnabled = true;
                NextButtonEnabled = false;
                LastButtonEnabled = false;
            }
            else if (CurrentPage == 0)
            {
                FirstButtonEnabled = false;
                PreviousButtonEnabled = false;
                NextButtonEnabled = true;
                LastButtonEnabled = true;
            }
            else
            {
                FirstButtonEnabled = true;
                PreviousButtonEnabled = true;
                NextButtonEnabled = true;
                LastButtonEnabled = true;
            }
        }


        #endregion
    }
}
