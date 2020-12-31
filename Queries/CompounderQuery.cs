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

        public List<ResultSet> Run(CompounderQueryParams parameters)
        {
            List<ResultSet> resultSetList = new List<ResultSet>();

            resultSetList = MainQuery(parameters.Date, parameters.Roe, parameters.ReinvestmentRate);
            resultSetList = AddRoeHistory(resultSetList, parameters.Date, parameters.HistoryDepth);
            resultSetList = AddCompanyName(resultSetList);

            ReportProgress(100, 100, $"OK! Finished query.");
            return resultSetList;
        }


        private List<ResultSet> MainQuery(string date, double roe, double reinvestmentRate)
        {
            ReportProgress(100, 10, $"Retrieving companies with ROE > {roe}");

            List<ResultSet> roeFiltered = (from income in DataContext.IncomeStatements
                                           join balance in DataContext.BalanceSheets
                                           on new { a = income.Symbol, b = income.Date } equals new { a = balance.Symbol, b = balance.Date }
                                           join cash in DataContext.CashFlowStatements
                                           on new { a = income.Symbol, b = income.Date } equals new { a = cash.Symbol, b = cash.Date }
                                           where income.Date == date
                                           && income.NetIncome > 0
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
        /// AddRoeHistory
        /// </summary>
        /// <param name="inputResultSetList"></param>
        /// <param name="date"></param>
        /// <param name="historyDepth"></param>
        /// <returns></returns>
        private List<ResultSet> AddRoeHistory(List<ResultSet> inputResultSetList, string date, int historyDepth)
        {
            for(int ii = 0; ii < inputResultSetList.Count(); ii++)
            {
                var historyRoe = QueryFactory.RoeHistoryQuery.Run(inputResultSetList[ii].Symbol, date, historyDepth);
                
                for(int i = 0; i < historyRoe.Count(); i++)
                {
                    inputResultSetList[ii].RoeHistory.Add(historyRoe[i]);
                }
            }

            return inputResultSetList;
        }

        /// <summary>
        /// CompounderStableRowGrowth
        /// </summary>
        /// <param name="inputResultSetList"></param>
        /// <param name="date"></param>
        /// <param name="historyDepth"></param>
        /// <param name="growthGrad"></param>
        /// <returns></returns>
        private List<ResultSet> CompounderStableRowGrowth(List<ResultSet> inputResultSetList, string date, int historyDepth, int growthGrad)
        {
            ReportProgress(100, 30, $"Filtering companies without stable ROE growth out...");
            List<ResultSet> resultSetList = new List<ResultSet>();

            foreach (var resultSet in inputResultSetList)
            {
                var historyRoe = QueryFactory.RoeHistoryQuery.Run(resultSet.Symbol, date, historyDepth);
                if (historyRoe.Count() < historyDepth || !historyRoe.AllPositive())
                {
                    continue;
                }

                // Decline is used for growth determination because of reverse order
                if (historyRoe.Declines() < growthGrad)
                {
                    continue;
                }

                resultSetList.Add(resultSet);
            }

            ReportProgress(100, 40, $"OK! {resultSetList.Count()} companies found.");
            return resultSetList;
        }

        /// <summary>
        /// CompounderStableReinvestmentGrowth
        /// </summary>
        /// <param name="inputResultSetList"></param>
        /// <param name="date"></param>
        /// <param name="historyDepth"></param>
        /// <param name="growthGrad"></param>
        /// <returns></returns>
        private List<ResultSet> CompounderStableReinvestmentGrowth(List<ResultSet> inputResultSetList, string date, int historyDepth, int growthGrad)
        {
            ReportProgress(100, 50, $"Filtering companies without stable reinvestment growth out...");
            List<ResultSet> resultSetList = new List<ResultSet>();

            foreach (var resultSet in inputResultSetList)
            {
                var historyReinvestment = QueryFactory.ReinvestmentHistoryQuery.Run(resultSet.Symbol, date, historyDepth);
                if (historyReinvestment.Count() < historyDepth || !historyReinvestment.AllPositive())
                {
                    continue;
                }

                // Decline is used for growth determination because of reverse order
                if (historyReinvestment.Declines() < growthGrad)
                {
                    continue;
                }

                resultSetList.Add(resultSet);
            }

            ReportProgress(100, 60, $"OK! {resultSetList.Count()} companies found.");
            return resultSetList;
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
