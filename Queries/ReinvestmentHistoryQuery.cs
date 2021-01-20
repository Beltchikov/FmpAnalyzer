using FmpAnalyzer.Data;
using FmpDataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// RoeHistoryQuery
    /// </summary>
    public class ReinvestmentHistoryQuery : HistoryQuery
    {
        public ReinvestmentHistoryQuery(DataContext dataContext) : base(dataContext) { }

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="oldestDate"></param>
        /// <param name="depth"></param>
        /// <returns>If some data is missing, the list is returned shorter as expected.</returns>
        public override List<double> Run(string symbol, string oldestDate, int depth)
        {
            List<double> resultList = new List<double>();

            var currentDate = oldestDate;
            for (int i = 0; i < depth; i++)
            {
                var queryResult = (from cash in DataContext.CashFlowStatements
                                   where cash.Symbol == symbol
                                   && cash.Date == currentDate
                                   select cash.NetIncome == 0 ? 0 : cash.CapitalExpenditure * -100 / cash.NetIncome)
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
