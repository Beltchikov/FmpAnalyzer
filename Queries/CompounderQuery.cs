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
        public CompounderQuery(DataContext dataContext) : base(dataContext) {}

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="date"></param>
        /// <param name="roe"></param>
        /// <param name="historyDepth"></param>
        /// <param name="growthGrad"></param>
        /// <returns></returns>
        public async Task<List<string>> Run(string date, double roe, int historyDepth, int growthGrad)
        {
            List<string> resultList = CompounderHighRoe(date, roe);
            resultList = CompounderStableRowGrowth(resultList, date, historyDepth, growthGrad);

            return await Task<List<string>>.Run(() => resultList);
        }

        /// <summary>
        /// CompounderHighRoe
        /// </summary>
        /// <param name="date"></param>
        /// <param name="roe"></param>
        /// <returns></returns>
        public List<string> CompounderHighRoe(string date, double roe)
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
        public List<string> CompounderStableRowGrowth(List<string> inputSymbolList, string date, int historyDepth, int growthGrad)
        {
            ReportProgress(100, 20, $"filtering companies without stable ROE growth out...");

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

            return resultList;
        }

    }
}
