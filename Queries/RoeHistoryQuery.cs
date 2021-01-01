using FmpAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// RoeHistoryQuery
    /// </summary>
    public class RoeHistoryQuery : QueryBase
    {
        public RoeHistoryQuery(DataContext dataContext) : base(dataContext) { }

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="oldestDate"></param>
        /// <param name="depth"></param>
        /// <returns>If some data is missing, the list is returned shorter as expected.</returns>
        public List<double> Run(string symbol, string oldestDate, int depth)
        {
            List<double> resultList = new List<double>();

            var currentDate = oldestDate;
            for (int i = 0; i < depth; i++)
            {
                var queryResult = (from income in DataContext.IncomeStatements
                                   join balance in DataContext.BalanceSheets
                                   on new { a = income.Symbol, b = income.Date } equals new { a = balance.Symbol, b = balance.Date }
                                   where income.Symbol == symbol
                                   && income.Date == currentDate
                                   && income.NetIncome > 0
                                   select balance.TotalStockholdersEquity == 0 ? 0 : income.NetIncome * 100 / balance.TotalStockholdersEquity)
                                  .ToList();

                if (!queryResult.Any())
                {
                    continue;
                }

                resultList.Add(queryResult.FirstOrDefault());
                currentDate = (Convert.ToInt32(currentDate[..4]) - 1).ToString() + currentDate[4..];
            }

            return resultList;
        }
    }
}
