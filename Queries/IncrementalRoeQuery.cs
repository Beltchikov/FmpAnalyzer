using FmpAnalyzer.ResultSet;
using FmpDataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// IncrementalRoeQuery
    /// </summary>
    public class IncrementalRoeQuery : HistoryQuery
    {
        public IncrementalRoeQuery(DataContext dataContext) : base(dataContext) { }

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="youngestDate"></param>
        /// <param name="depth"></param>
        /// <returns>If some data is missing, the list is returned shorter as expected.</returns>
        public override List<double> Run(string symbol, string youngestDate, int depth)
        {
            List<double> resultList = new List<double>();

            var date1YearAgo = (Convert.ToInt32(youngestDate[..4]) - 1).ToString() + youngestDate[4..];
            var date3YearsAgo = (Convert.ToInt32(youngestDate[..4]) - 3).ToString() + youngestDate[4..];
            var date5YearsAgo = (Convert.ToInt32(youngestDate[..4]) - 5).ToString() + youngestDate[4..];

            var queryResultYoungest = RunQuery(symbol, youngestDate);
            if (queryResultYoungest == null)
            {
                return resultList;
            }

            var queryResult2YearsAgo = RunQuery(symbol, date1YearAgo);
            resultList.Add(queryResult2YearsAgo == null ? 0 : CalculateIncrementalRow(queryResultYoungest, queryResult2YearsAgo));
            
            var queryResult4YearsAgo = RunQuery(symbol, date3YearsAgo);
            resultList.Add(queryResult4YearsAgo == null ? 0 : CalculateIncrementalRow(queryResultYoungest, queryResult4YearsAgo));

            var queryResult6YearsAgo = RunQuery(symbol, date5YearsAgo);
            resultList.Add(queryResult6YearsAgo == null ? 0 : CalculateIncrementalRow(queryResultYoungest, queryResult6YearsAgo));

            return resultList;
        }

        /// <summary>
        /// CalculateIncrementalRow
        /// </summary>
        /// <param name="youngestData"></param>
        /// <param name="oldestData"></param>
        /// <returns></returns>
        double CalculateIncrementalRow(NetIncomeAndEquity youngestData, NetIncomeAndEquity oldestData)
        {
            if(youngestData.Equity - oldestData.Equity == 0)
            {
                return 0;
            }

            return (youngestData.NetIncome - oldestData.NetIncome)*100 / (youngestData.Equity - oldestData.Equity);
        }

        /// <summary>
        /// RunQuery
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        private NetIncomeAndEquity RunQuery(string symbol, string date)
        {
            return (from income in DataContext.IncomeStatements
                join balance in DataContext.BalanceSheets
                on new { a = income.Symbol, b = income.Date } equals new { a = balance.Symbol, b = balance.Date }
                where income.Symbol == symbol
                && income.Date == date
                select new NetIncomeAndEquity
                {
                    NetIncome = income.NetIncome,
                    Equity = balance.TotalStockholdersEquity
                }).FirstOrDefault();
        }

    }
}
