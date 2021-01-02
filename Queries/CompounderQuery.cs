using FmpAnalyzer.Data;
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

            resultSetList = MainQuery(parameters.Date, parameters.Roe, parameters.ReinvestmentRate);
            resultSetList = AddHistoryData(resultSetList, parameters.Date, parameters.HistoryDepth, QueryFactory.RoeHistoryQuery, a => a.RoeHistory);
            resultSetList = AddHistoryData(resultSetList, parameters.Date, parameters.HistoryDepth, QueryFactory.ReinvestmentHistoryQuery, a => a.ReinvestmentHistory);
            resultSetList = AddHistoryData(resultSetList, parameters.Date, parameters.HistoryDepth, QueryFactory.IncrementalRoeQuery, a => a.IncrementalRoe);
            resultSetList = AddCompanyName(resultSetList);

            ReportProgress(100, 100, $"OK! Finished query.");
            return resultSetList;
        }

        /// <summary>
        /// MainQuery
        /// </summary>
        /// <param name="date"></param>
        /// <param name="roe"></param>
        /// <param name="reinvestmentRate"></param>
        /// <returns></returns>
        private List<ResultSet> MainQuery(string date, double roe, double reinvestmentRate)
        {
            ReportProgress(100, 10, $"Retrieving companies with ROE > {roe}");

            List<ResultSet> roeFiltered = (from income in DataContext.IncomeStatements
                                           join balance in DataContext.BalanceSheets
                                           on new { a = income.Symbol, b = income.Date } equals new { a = balance.Symbol, b = balance.Date }
                                           join cash in DataContext.CashFlowStatements
                                           on new { a = income.Symbol, b = income.Date } equals new { a = cash.Symbol, b = cash.Date }
                                           where income.Date == date
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
        /// <param name="date"></param>
        /// <param name="historyDepth"></param>
        /// <param name="query"></param>
        /// <param name="funcAttributeToSet"></param>
        /// <returns></returns>
        private List<ResultSet> AddHistoryData(List<ResultSet> inputResultSetList, string date, int historyDepth,
            HistoryQuery query, Func<ResultSet, List<double>> funcAttributeToSet)
        {
            for (int i = 0; i < inputResultSetList.Count(); i++)
            {
                var queryResults = query.Run(inputResultSetList[i].Symbol, date, historyDepth);
                queryResults.Reverse();

                for (int ii = 0; ii < queryResults.Count(); ii++)
                {
                    inputResultSetList.Select(funcAttributeToSet).ToList()[i].Add(queryResults[ii]);
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
