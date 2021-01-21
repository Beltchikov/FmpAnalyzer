using FmpAnalyzer.Data;
using FmpDataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// CompounderQuery
    /// </summary>
    public class CompounderQuery : QueryBase
    {
        public CompounderQuery(DataContext dataContext) : base(dataContext) { }

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ResultSetList Run<T>(CompounderQueryParams<T> parameters)
        {
            ResultSetList resultSetList = null;

            try
            {
                var p = parameters;
                resultSetList = MainQuery<T>(parameters);
                resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.RoeHistoryQuery, a => a.RoeHistory);
                resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.ReinvestmentHistoryQuery, a => a.ReinvestmentHistory);
                resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.IncrementalRoeQuery, a => a.IncrementalRoe);
                resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.RevenueHistoryQuery, a => a.RevenueHistory);
                resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.OperatingIncomeHistoryQuery, a => a.OperatingIncome);
                resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.EpsHistoryQuery, a => a.Eps);
                resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.CashConversionQuery, a => a.CashConversionHistory);
                resultSetList = AddCompanyName(resultSetList);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());

            }

            ReportProgress(100, 100, $"OK! Finished query.");
            return resultSetList;
        }

        /// <summary>
        /// Count
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int Count(CompounderCountQueryParams parameters)
        {
            return QueryAsEnumerable(parameters).Count();
        }

        /// <summary>
        /// MainQuery
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private ResultSetList MainQuery<TKey>(CompounderQueryParams<TKey> parameters)
        {
            ReportProgress(100, 10, $"Retrieving companies with ROE > {parameters.Roe}");
            
            var queryAsEnumerable = QueryAsEnumerable(parameters).OrderByDescending(parameters.OrderFunction).ToList();
            var p = parameters;
            List<ResultSet> roeFiltered = p.Descending
                ? queryAsEnumerable.OrderByDescending(p.OrderFunction).Skip(p.CurrentPage * p.PageSize).Take(p.PageSize).ToList()
                : queryAsEnumerable.OrderBy(p.OrderFunction).Skip(p.CurrentPage* p.PageSize).Take(p.PageSize).ToList();
            ResultSetList resultSetList  = new ResultSetList(roeFiltered);
            resultSetList.CountTotal = queryAsEnumerable.Count();
            
            ReportProgress(100, 20, $"OK! {resultSetList.CountTotal} companies found.");

            return resultSetList;
        }

        /// <summary>
        /// QueryAsEnumerable
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private IEnumerable<ResultSet> QueryAsEnumerable(CompounderCountQueryParams parameters)
        {
            var dates = FmpHelper.BuildDatesList(parameters.YearFrom, parameters.YearTo, parameters.Dates);

            return from income in DataContext.IncomeStatements
                   join balance in DataContext.BalanceSheets
                   on new { a = income.Symbol, b = income.Date } equals new { a = balance.Symbol, b = balance.Date }
                   join cash in DataContext.CashFlowStatements
                   on new { a = income.Symbol, b = income.Date } equals new { a = cash.Symbol, b = cash.Date }
                   where dates.Contains(income.Date)
                   && String.IsNullOrWhiteSpace(parameters.Symbol) ? 1 == 1 : income.Symbol == parameters.Symbol
                   select new
                   {
                       Symbol = income.Symbol,
                       Equity = balance.TotalStockholdersEquity,
                       Roe = balance.TotalStockholdersEquity == 0
                          ? 0
                          : Math.Round(income.NetIncome * 100 / balance.TotalStockholdersEquity, 0),
                       ReinvestmentRate = income.NetIncome == 0
                          ? 0
                          : Math.Round(cash.CapitalExpenditure * -100 / income.NetIncome, 0)
                   } into selectionFirst
                   where selectionFirst.Roe >= parameters.Roe
                   && selectionFirst.ReinvestmentRate >= parameters.ReinvestmentRate
                   select new
                   {
                       Symbol = selectionFirst.Symbol,
                       Roe = selectionFirst.Roe,
                       ReinvestmentRate = selectionFirst.ReinvestmentRate
                   } into selectionSecond
                   orderby selectionSecond.Roe descending
                   select new ResultSet { Symbol = selectionSecond.Symbol, Roe = selectionSecond.Roe, ReinvestmentRate = selectionSecond.ReinvestmentRate };
        }



        /// <summary>
        /// AddHistoryData
        /// </summary>
        /// <param name="inputResultSetList"></param>
        /// <param name="parameters"></param>
        /// <param name="query"></param>
        /// <param name="funcAttributeToSet"></param>
        /// <returns></returns>
        private ResultSetList AddHistoryData(ResultSetList inputResultSetList, HistoryQueryParams parameters,
            HistoryQuery query, Func<ResultSet, List<double>> funcAttributeToSet)
        {
            for (int i = 0; i < inputResultSetList.ResultSets.Count(); i++)
            {
                foreach (string dateParam in parameters.Dates)
                {
                    string date = parameters.YearFrom.ToString() + dateParam[4..];

                    var queryResults = query.Run(inputResultSetList.ResultSets[i].Symbol, date, parameters.HistoryDepth);
                    if (!queryResults.Any())
                    {
                        continue;
                    }

                    queryResults.Reverse();
                    for (int ii = 0; ii < queryResults.Count(); ii++)
                    {
                        inputResultSetList.ResultSets.Select(funcAttributeToSet).ToList()[i].Add(queryResults[ii]);
                    }
                    break;
                }
            }

            return inputResultSetList;
        }

        /// <summary>
        /// AddCompanyName
        /// </summary>
        /// <param name="inputResultSetList"></param>
        /// <returns></returns>
        private ResultSetList AddCompanyName(ResultSetList inputResultSetList)
        {
            ReportProgress(100, 70, $"Searching for the companies names ...");
            ResultSetList resultSetList = QueryFactory.CompanyNameQuery.Run(inputResultSetList);
            ReportProgress(100, 80, $"Search for the companies names ended ...");
            return resultSetList;
        }
    }
}
