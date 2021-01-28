using FmpAnalyzer.Data;
using FmpDataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Common;

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

            var p = parameters;
            ReportProgress(100, 10, $"Retrieving companies with ROE > {parameters.RoeFrom}");

            var dates = FmpHelper.BuildDatesList(parameters.YearFrom, parameters.YearTo, parameters.Dates);
            var command = DbCommands.Compounder(DataContext.Database.GetDbConnection(), Sql.Compounder(parameters, dates), parameters, dates);
            var queryAsEnumerable = QueryAsEnumerable(command, ResultsetFunctionCompounder).OrderByDescending(parameters.OrderFunction).ToList();

            queryAsEnumerable = AddHistoryData(queryAsEnumerable, parameters, QueryFactory.RoeHistoryQuery, a => a.RoeHistory);
            queryAsEnumerable = AddHistoryData(queryAsEnumerable, parameters, QueryFactory.RevenueHistoryQuery, a => a.RevenueHistory);
            queryAsEnumerable = AddHistoryData(queryAsEnumerable, parameters, QueryFactory.EpsHistoryQuery, a => a.EpsHistory);

            queryAsEnumerable = AdjustToGrowthKoef(queryAsEnumerable, parameters.RoeGrowthKoef, r => r.RoeHistory);
            queryAsEnumerable = AdjustToGrowthKoef(queryAsEnumerable, parameters.RevenueGrowthKoef, r => r.RevenueHistory);
            queryAsEnumerable = AdjustToGrowthKoef(queryAsEnumerable, parameters.EpsGrowthKoef, r => r.EpsHistory);

            List<ResultSet> listOfResultSets = p.Descending
                ? queryAsEnumerable.OrderByDescending(p.OrderFunction).Skip(p.CurrentPage * p.PageSize).Take(p.PageSize).ToList()
                : queryAsEnumerable.OrderBy(p.OrderFunction).Skip(p.CurrentPage * p.PageSize).Take(p.PageSize).ToList();
            resultSetList = new ResultSetList(listOfResultSets);
            resultSetList.CountTotal = queryAsEnumerable.Count();

            ReportProgress(100, 20, $"OK! {resultSetList.CountTotal} companies found.");

            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.ReinvestmentHistoryQuery, a => a.ReinvestmentHistory);
            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.IncrementalRoeQuery, a => a.IncrementalRoe);
            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.OperatingIncomeHistoryQuery, a => a.OperatingIncome);
            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.CashConversionQuery, a => a.CashConversionHistory);
            resultSetList = AddCompanyName(resultSetList);
            resultSetList = AddDebtEquityIncome(resultSetList);

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
            var dates = FmpHelper.BuildDatesList(parameters.YearFrom, parameters.YearTo, parameters.Dates);
            var command = DbCommands.Compounder(DataContext.Database.GetDbConnection(), Sql.Compounder(parameters, dates), parameters, dates);
            return QueryAsEnumerable(command, ResultsetFunctionCompounder).Count();
        }

        /// <summary>
        /// Run
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public ResultSetList FindBySymbol<T>(CompounderQueryParams<T> parameters, string symbol)
        {
            ResultSetList resultSetList = null;

            var command = DbCommands.FindBySymbol(DataContext.Database.GetDbConnection(), Sql.FindBySymbol(symbol), symbol);
            var queryAsEnumerable = QueryAsEnumerable(command, ResultsetFunctionFindBySymbol).ToList();

            queryAsEnumerable = AddHistoryData(queryAsEnumerable, parameters, QueryFactory.RoeHistoryQuery, a => a.RoeHistory);
            queryAsEnumerable = AddHistoryData(queryAsEnumerable, parameters, QueryFactory.RevenueHistoryQuery, a => a.RevenueHistory);
            queryAsEnumerable = AddHistoryData(queryAsEnumerable, parameters, QueryFactory.EpsHistoryQuery, a => a.EpsHistory);

            queryAsEnumerable = AdjustToGrowthKoef(queryAsEnumerable, parameters.RoeGrowthKoef, r => r.RoeHistory);
            queryAsEnumerable = AdjustToGrowthKoef(queryAsEnumerable, parameters.RevenueGrowthKoef, r => r.RevenueHistory);
            queryAsEnumerable = AdjustToGrowthKoef(queryAsEnumerable, parameters.EpsGrowthKoef, r => r.EpsHistory);

            List<ResultSet> listOfResultSets = queryAsEnumerable.ToList();
            resultSetList = new ResultSetList(listOfResultSets);
            resultSetList.CountTotal = queryAsEnumerable.Count();

            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.ReinvestmentHistoryQuery, a => a.ReinvestmentHistory);
            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.IncrementalRoeQuery, a => a.IncrementalRoe);
            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.OperatingIncomeHistoryQuery, a => a.OperatingIncome);
            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.CashConversionQuery, a => a.CashConversionHistory);
            resultSetList = AddCompanyName(resultSetList);
            resultSetList = AddDebtEquityIncome(resultSetList);

            return resultSetList;

        }

        /// <summary>
        /// FindByCompany
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        public string FindByCompany<T>(CompounderQueryParams<T> parameters, string company)
        {
            string result = string.Empty;

            var command = DbCommands.FindByCompany(DataContext.Database.GetDbConnection(), Sql.FindByCompany(company), company);
            var queryAsEnumerable = QueryAsEnumerable(command, ResultsetFunctionFindByCompany).ToList();
            if(!queryAsEnumerable.Any())
            {
                return string.Empty;
            }
            
            result = queryAsEnumerable.Select(q => q.Symbol + "\t" + q.Name).Distinct().Aggregate((r, n) => r+ "\r\n" + n);

            return result;
        }

        /// <summary>
        /// QueryAsEnumerable
        /// </summary>
        /// <param name="command"></param>
        /// <param name="resultSetFunction"></param>
        /// <returns></returns>
        private IEnumerable<ResultSet> QueryAsEnumerable(DbCommand command, Func<DataTable, IEnumerable<ResultSet>> resultSetFunction)
        {
            command.Connection.Open();
            DataTable dataTable = null;
            using (var reader = command.ExecuteReader())
            {
                dataTable = new DataTable();
                dataTable.Load(reader);

            }
            command.Connection.Close();
            return resultSetFunction(dataTable);
        }

        /// <summary>
        /// ResultsetFunctionCompounder
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private IEnumerable<ResultSet> ResultsetFunctionCompounder(DataTable dataTable)
        {
            List<ResultSet> listOfResultSets = new List<ResultSet>();

            foreach (DataRow row in dataTable.Rows)
            {
                ResultSet resultSet = new ResultSet();

                resultSet.Symbol = (string)row["Symbol"];
                resultSet.Date = (string)row["Date"];
                resultSet.Equity = row["Equity"] == DBNull.Value ? null : (double)row["Equity"];
                resultSet.Debt = row["Debt"] == DBNull.Value ? null : (double)row["Debt"];
                resultSet.NetIncome = row["NetIncome"] == DBNull.Value ? null : (double)row["NetIncome"];
                resultSet.Roe = row["Roe"] == DBNull.Value ? null : Math.Round((double)row["Roe"], 0);
                resultSet.ReinvestmentRate = row["ReinvestmentRate"] == DBNull.Value ? null : Math.Round((double)row["ReinvestmentRate"], 0);
                resultSet.DebtEquityRatio = row["DebtEquityRatio"] == DBNull.Value ? null : Math.Round((double)row["DebtEquityRatio"], 2);

                listOfResultSets.Add(resultSet);
            }

            return listOfResultSets;
        }

        /// <summary>
        /// ResultsetFunctionFindBySymbol
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private IEnumerable<ResultSet> ResultsetFunctionFindBySymbol(DataTable dataTable)
        {
            return ResultsetFunctionCompounder(dataTable);
        }

        /// <summary>
        /// ResultsetFunctionFindByCompany
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private IEnumerable<ResultSet> ResultsetFunctionFindByCompany(DataTable dataTable)
        {
            List<ResultSet> listOfResultSets = new List<ResultSet>();
            foreach (DataRow row in dataTable.Rows)
            {
                ResultSet resultSet = new ResultSet();

                resultSet.Symbol = (string)row["Symbol"];
                resultSet.Name = (string)row["Name"];
                
                listOfResultSets.Add(resultSet);
            }

            return listOfResultSets;
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
        /// AddHistoryData
        /// </summary>
        /// <param name="inputResultSetList"></param>
        /// <param name="parameters"></param>
        /// <param name="query"></param>
        /// <param name="funcAttributeToSet"></param>
        /// <returns></returns>
        private List<ResultSet> AddHistoryData(List<ResultSet> inputResultSetList, HistoryQueryParams parameters,
            HistoryQuery query, Func<ResultSet, List<double>> funcAttributeToSet)
        {
            for (int i = 0; i < inputResultSetList.Count(); i++)
            {
                foreach (string dateParam in parameters.Dates)
                {
                    string date = parameters.YearFrom.ToString() + dateParam[4..];

                    var queryResults = query.Run(inputResultSetList[i].Symbol, date, parameters.HistoryDepth);
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
        /// AdjustToRoeGrowthKoef
        /// </summary>
        /// <param name="inputResultSetList"></param>
        /// <param name="growthKoef"></param>
        /// <param name="funcResultSetParam"></param>
        /// <returns></returns>
        private List<ResultSet> AdjustToGrowthKoef(List<ResultSet> inputResultSetList, int growthKoef, Func<ResultSet, List<double>> funcResultSetParam)
        {
            if (growthKoef == 0)
            {
                return inputResultSetList;
            }

            List<ResultSet> resultSetList = new List<ResultSet>();

            foreach (ResultSet resultSet in inputResultSetList)
            {
                if (funcResultSetParam(resultSet).Grows() >= growthKoef)
                {
                    resultSetList.Add(resultSet);
                }
            }

            return resultSetList;
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

        /// <summary>
        /// AddDebtEquityIncome
        /// </summary>
        /// <param name="inputResultSetList"></param>
        /// <returns></returns>
        private ResultSetList AddDebtEquityIncome(ResultSetList inputResultSetList)
        {
            for (int i = 0; i < inputResultSetList.ResultSets.Count(); i++)
            {
                ResultSet resultSet = inputResultSetList.ResultSets[i];
                resultSet.DebtEquityIncome.Add(resultSet.Debt.Value);
                resultSet.DebtEquityIncome.Add(resultSet.Equity.Value);
                resultSet.DebtEquityIncome.Add(resultSet.NetIncome.Value);
            }
            return inputResultSetList;
        }

        /// <summary>
        /// AddDebtEquityIncome
        /// </summary>
        /// <param name="inputResultSetList"></param>
        /// <returns></returns>
        private List<ResultSet> AddDebtEquityIncome(List<ResultSet> inputResultSetList)
        {
            for (int i = 0; i < inputResultSetList.Count(); i++)
            {
                ResultSet resultSet = inputResultSetList[i];
                resultSet.DebtEquityIncome.Add(resultSet.Debt.Value);
                resultSet.DebtEquityIncome.Add(resultSet.Equity.Value);
                resultSet.DebtEquityIncome.Add(resultSet.NetIncome.Value);
            }
            return inputResultSetList;
        }
    }
}
