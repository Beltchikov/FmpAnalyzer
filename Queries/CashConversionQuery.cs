using FmpAnalyzer.Data;
using FmpDataContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FmpAnalyzer.Queries
{
    public class CashConversionQuery: HistoryQuery
    {
        public CashConversionQuery(DataContext dataContext) : base(dataContext) { }

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="oldestDate"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public override List<double> Run(string symbol, string oldestDate, int depth)
        {
            List<double> resultList = new List<double>();

            var currentDate = oldestDate;
            for (int i = 0; i < depth; i++)
            {
                var queryResult = (from cash in DataContext.CashFlowStatements
                                   where cash.Symbol == symbol
                                   && cash.Date == currentDate
                                   select cash.NetIncome == 0 ? 0 : cash.OperatingCashFlow * 100 / cash.NetIncome)
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