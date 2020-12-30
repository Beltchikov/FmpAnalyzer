using FmpAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// Returns companies with best ROE.
    /// </summary>
    public class WithBestRoeQuery : QueryBase
    {
        public WithBestRoeQuery(DataContext dataContext) : base(dataContext) { }

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
        public List<string> Run(int top, string date)
        {
            return (from income in DataContext.IncomeStatements
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
                    } into unordered
                    orderby unordered.Roe descending
                    select unordered.Symbol)
                         .Take(top)
                         .ToList();
        }
    }
}
