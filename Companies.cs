using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmpAnalyzer
{
    /// <summary>
    /// Companies
    /// </summary>
    public class Companies : DataContext
    {
        private static Companies _companies;
        private static readonly object lockObject = new object();

        Companies() { }

        /// <summary>
        /// Instance
        /// </summary>
        public new static Companies Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (_companies == null)
                    {
                        _companies = new Companies();
                    }
                    return _companies;
                }
            }
        }

        public delegate void DatabaseActionDelegate(object sendet, DatabaseActionEventArgs e);

        public event DatabaseActionDelegate DatabaseAction;

        /// <summary>
        /// WithBestRoe
        /// </summary>
        /// <param name="top"></param>
        /// <param name="date">Date should be in format '2019-12-31' </param>
        /// <returns></returns>
        /*
        SELECT TOP 10 I.symbol, I.Date, I.NetIncome, B.TotalStockholdersEquity,
        (CASE WHEN B.TotalStockholdersEquity = 0 THEN 0 ELSE I.NetIncome * 100 / B.TotalStockholdersEquity END) ROE
        FROM IncomeStatements I
        INNER JOIN BalanceSheets B ON I.Symbol = B.Symbol AND I.Date = B.Date
        WHERE I.Date = '2019-12-31' 
        AND I.NetIncome > 0
        ORDER BY ROE DESC
        */
        public List<string> WithBestRoe(int top, string date)
        {
            return (from income in IncomeStatements
                    join balance in BalanceSheets
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
                    } into unordered
                    orderby unordered.Roe descending
                    select unordered.Symbol)
                         .Take(top)
                         .ToList();
        }

        /// <summary>
        /// HistoryRoe
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="oldestDate"></param>
        /// <param name="depth"></param>
        /// <returns>If some data is missing, the list is returned shorter as expected.</returns>
        public List<double> HistoryRoe(string symbol, string oldestDate, int depth)
        {
            List<double> resultList = new List<double>();

            var currentDate = oldestDate;
            for (int i = 0; i < depth; i++)
            {
                var queryResult = (from income in IncomeStatements
                                   join balance in BalanceSheets
                                   on new { a = income.Symbol, b = income.Date } equals new { a = balance.Symbol, b = balance.Date }
                                   where income.Symbol == symbol
                                   && income.Date == currentDate
                                   && income.NetIncome > 0
                                   select balance.TotalStockholdersEquity == 0 ? 0 : income.NetIncome * 100 / balance.TotalStockholdersEquity)
                                  .ToList();

                if (!queryResult.Any())
                {
                    return resultList;
                }

                resultList.Add(queryResult.FirstOrDefault());
                currentDate = (Convert.ToInt32(currentDate[..4]) - 1).ToString() + currentDate[4..];
            }

            return resultList;
        }

        /// <summary>
        /// ReportProgress
        /// </summary>
        /// <param name="max"></param>
        /// <param name="current"></param>
        /// <param name="message"></param>
        public void ReportProgress(int max, int current, string message)
        {
            DatabaseAction?.Invoke(this, new DatabaseActionEventArgs
            {
                Action = $"filtering companies without stable ROE growth out...",
                ProgressValue = current,
                MaxValue = max
            });
        }
    }
}


