using FmpAnalyzer.Data;
using FmpDataContext;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<ResultSet> Run(CompounderQueryParams parameters)
        {
            List<ResultSet> resultSetList = new List<ResultSet>();

            resultSetList = MainQuery(parameters.YearFrom, parameters.YearTo, parameters.Dates, parameters.Roe, parameters.ReinvestmentRate, parameters.Symbol);
            resultSetList = AddHistoryData(resultSetList, parameters.Dates, parameters.YearFrom, parameters.HistoryDepth, QueryFactory.RoeHistoryQuery, a => a.RoeHistory);
            resultSetList = AddHistoryData(resultSetList, parameters.Dates, parameters.YearFrom, parameters.HistoryDepth, QueryFactory.ReinvestmentHistoryQuery, a => a.ReinvestmentHistory);
            resultSetList = AddHistoryData(resultSetList, parameters.Dates, parameters.YearFrom, parameters.HistoryDepth, QueryFactory.IncrementalRoeQuery, a => a.IncrementalRoe);
            resultSetList = AddHistoryData(resultSetList, parameters.Dates, parameters.YearFrom, parameters.HistoryDepth, QueryFactory.RevenueHistoryQuery, a => a.RevenueHistory);
            resultSetList = AddHistoryData(resultSetList, parameters.Dates, parameters.YearFrom, parameters.HistoryDepth, QueryFactory.OperatingIncomeHistoryQuery, a => a.OperatingIncome);
            resultSetList = AddHistoryData(resultSetList, parameters.Dates, parameters.YearFrom, parameters.HistoryDepth, QueryFactory.EpsHistoryQuery, a => a.Eps);
            resultSetList = AddHistoryData(resultSetList, parameters.Dates, parameters.YearFrom, parameters.HistoryDepth, QueryFactory.CashConversionQuery, a => a.CashConversionHistory);
            resultSetList = AddCompanyName(resultSetList);

            ReportProgress(100, 100, $"OK! Finished query.");
            return resultSetList;
        }

        /// <summary>
        /// MainQuery
        /// </summary>
        /// <param name="yearFrom"></param>
        /// <param name="yearTo"></param>
        /// <param name="datesTemplates"></param>
        /// <param name="roe"></param>
        /// <param name="reinvestmentRate"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private List<ResultSet> MainQuery(int yearFrom, int yearTo, List<string> datesTemplates, double roe, double reinvestmentRate, string symbol)
        {
            ReportProgress(100, 10, $"Retrieving companies with ROE > {roe}");

            var dates = FmpHelper.BuildDatesList(yearFrom, yearTo, datesTemplates);

            List<ResultSet> roeFiltered = (from income in DataContext.IncomeStatements
                                           join balance in DataContext.BalanceSheets
                                           on new { a = income.Symbol, b = income.Date } equals new { a = balance.Symbol, b = balance.Date }
                                           join cash in DataContext.CashFlowStatements
                                           on new { a = income.Symbol, b = income.Date } equals new { a = cash.Symbol, b = cash.Date }
                                           where dates.Contains(income.Date)
                                           && String.IsNullOrWhiteSpace(symbol) ? 1 == 1 : income.Symbol == symbol
                                           select new
                                           {
                                               Symbol = income.Symbol,
                                               Equity = balance.TotalStockholdersEquity,
                                               Roe = balance.TotalStockholdersEquity == 0
                                                  ? 0
                                                  : Math.Round(income.NetIncome * 100 / balance.TotalStockholdersEquity, 0),
                                               ReinvestmentRate = income.NetIncome == 0
                                                  ? 0
                                                  : Math.Round(cash.InvestmentsInPropertyPlantAndEquipment * -100 / income.NetIncome, 0)
                                           } into selectionFirst
                                           where selectionFirst.Roe >= roe
                                           && selectionFirst.ReinvestmentRate >= reinvestmentRate
                                           select new
                                           {
                                               Symbol = selectionFirst.Symbol,
                                               Roe = selectionFirst.Roe,
                                               ReinvestmentRate = selectionFirst.ReinvestmentRate
                                           } into selectionSecond
                                           orderby selectionSecond.Roe descending
                                           select new ResultSet { Symbol = selectionSecond.Symbol, Roe = selectionSecond.Roe, ReinvestmentRate = selectionSecond.ReinvestmentRate })
                                           .ToList();

            ReportProgress(100, 20, $"OK! {roeFiltered.Count()} companies found.");
            return roeFiltered;
        }

        /// <summary>
        /// AddHistoryData
        /// </summary>
        /// <param name="inputResultSetList"></param>
        /// <param name="dates"></param>
        /// <param name="year"></param>
        /// <param name="historyDepth"></param>
        /// <param name="query"></param>
        /// <param name="funcAttributeToSet"></param>
        /// <returns></returns>
        private List<ResultSet> AddHistoryData(List<ResultSet> inputResultSetList, List<string> dates, int year, int historyDepth,
            HistoryQuery query, Func<ResultSet, List<double>> funcAttributeToSet)
        {
            for (int i = 0; i < inputResultSetList.Count(); i++)
            {
                foreach (string dateParam in dates)
                {
                    string date = year.ToString() + dateParam[4..];

                    var queryResults = query.Run(inputResultSetList[i].Symbol, date, historyDepth);
                    if (!queryResults.Any())
                    {
                        continue;
                    }

                    queryResults.Reverse();
                    for (int ii = 0; ii < queryResults.Count(); ii++)
                    {
                        inputResultSetList.Select(funcAttributeToSet).ToList()[i].Add(queryResults[ii]);
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
        private List<ResultSet> AddCompanyName(List<ResultSet> inputResultSetList)
        {
            ReportProgress(100, 70, $"Searching for the companies names ...");
            List<ResultSet> resultSetList = QueryFactory.CompanyNameQuery.Run(inputResultSetList);
            ReportProgress(100, 80, $"Search for the companies names ended ...");
            return resultSetList;
        }

    }
}
