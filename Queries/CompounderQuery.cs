using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// CompounderQuery
    /// </summary>
    public class CompounderQuery : QueryBase
    {
        public CompounderQuery(DataContext dataContext) : base(dataContext) { }

        public async Task<List<string>> Run(CompounderQueryParams parameters)
        {
            List<string> resultList = CompounderHighRoe(parameters.Date, parameters.Roe);

            if (parameters.HistoryDepthRoe > 0 && parameters.GrowthGradRoe > 0)
            {
                resultList = CompounderStableRowGrowth(resultList, parameters.Date, parameters.HistoryDepthRoe, parameters.GrowthGradRoe);
            }

            if (parameters.HistoryDepthReinvestment > 0 && parameters.GrowthGradReinvestment > 0)
            {
                resultList = CompounderStableReinvestmentGrowth(resultList, parameters.Date, parameters.HistoryDepthReinvestment, parameters.GrowthGradReinvestment);
            }

            ReportProgress(100, 100, $"OK! Finished query.");
            return await Task<List<string>>.Run(() => resultList);
        }

        /// <summary>
        /// CompounderHighRoe
        /// </summary>
        /// <param name="date"></param>
        /// <param name="roe"></param>
        /// <returns></returns>
        private List<string> CompounderHighRoe(string date, double roe)
        {
            ReportProgress(100, 10, $"Retrieving companies with ROE > {roe}");

            var roeFiltered = (from income in DataContext.IncomeStatements
                               join balance in DataContext.BalanceSheets
                               on new { a = income.Symbol, b = income.Date } equals new { a = balance.Symbol, b = balance.Date }
                               where income.Date == date
                               && income.NetIncome > 0
                               select new
                               {
                                   Symbol = income.Symbol,
                                   Equity = balance.TotalStockholdersEquity,
                                   Roe = balance.TotalStockholdersEquity == 0
                                      ? 0
                                      : income.NetIncome * 100 / balance.TotalStockholdersEquity
                               } into selectionFirst
                               where selectionFirst.Roe >= roe
                               select new
                               {
                                   Symbol = selectionFirst.Symbol,
                                   Roe = selectionFirst.Roe
                               } into selectionSecond
                               orderby selectionSecond.Roe descending
                               select selectionSecond.Symbol)
                         .ToList();

            ReportProgress(100, 20, $"OK! {roeFiltered.Count()} companies found.");
            return roeFiltered;
        }

        /// <summary>
        /// CompounderStableRowGrowth
        /// </summary>
        /// <param name="inputSymbolList"></param>
        /// <param name="date"></param>
        /// <param name="historyDepth"></param>
        /// <param name="growthGrad"></param>
        /// <returns></returns>
        private List<string> CompounderStableRowGrowth(List<string> inputSymbolList, string date, int historyDepth, int growthGrad)
        {
            ReportProgress(100, 30, $"filtering companies without stable ROE growth out...");
            List<string> resultList = new List<string>();
            
            foreach (var symbol in inputSymbolList)
            {
                var historyRoe = QueryFactory.RoeHistoryQuery.Run(symbol, date, historyDepth);
                if (historyRoe.Count() < historyDepth || !historyRoe.AllPositive())
                {
                    continue;
                }

                // Decline is used for growth determination because of reverse order
                if (historyRoe.Declines() < growthGrad)
                {
                    continue;
                }

                resultList.Add(symbol);
            }

            ReportProgress(100, 40, $"OK! {resultList.Count()} companies found.");
            return resultList;
        }

        /// <summary>
        /// CompounderStableReinvestmentGrowth
        /// </summary>
        /// <param name="inputSymbolList"></param>
        /// <param name="date"></param>
        /// <param name="historyDepth"></param>
        /// <param name="growthGrad"></param>
        /// <returns></returns>
        private List<string> CompounderStableReinvestmentGrowth(List<string> inputSymbolList, string date, int historyDepth, int growthGrad)
        {
            ReportProgress(100, 50, $"filtering companies without stable reinvestment growth out...");
            List<string> resultList = new List<string>();

            foreach (var symbol in inputSymbolList)
            {
                var historyReinvestment = QueryFactory.ReinvestmentHistoryQuery.Run(symbol, date, historyDepth);
                if (historyReinvestment.Count() < historyDepth || !historyReinvestment.AllPositive())
                {
                    continue;
                }

                // Decline is used for growth determination because of reverse order
                if (historyReinvestment.Declines() < growthGrad)
                {
                    continue;
                }

                resultList.Add(symbol);
            }

            ReportProgress(100, 60, $"OK! {resultList.Count()} companies found.");
            return resultList;
        }

    }
}
