﻿using FmpAnalyzer.Data;
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

            try
            {
                var p = parameters;
                resultSetList = MainQuery<T>(parameters);

                resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.ReinvestmentHistoryQuery, a => a.ReinvestmentHistory);
                resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.IncrementalRoeQuery, a => a.IncrementalRoe);
                resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.RevenueHistoryQuery, a => a.RevenueHistory);
                resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.OperatingIncomeHistoryQuery, a => a.OperatingIncome);
                resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.EpsHistoryQuery, a => a.Eps);
                resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.CashConversionQuery, a => a.CashConversionHistory);
                resultSetList = AddCompanyName(resultSetList);
                resultSetList = AddDebtEquityIncome(resultSetList);
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
        /// Run
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public List<ResultSet> Run<T>(CompounderQueryParams<T> parameters, string symbol)
        {
            var dates = FmpHelper.BuildDatesList(parameters.YearFrom, parameters.YearTo, parameters.Dates);

            List<ResultSet> resultSetList =
                   (from income in DataContext.IncomeStatements
                    join balance in DataContext.BalanceSheets
                    on new { a = income.Symbol, b = income.Date } equals new { a = balance.Symbol, b = balance.Date }
                    join cash in DataContext.CashFlowStatements
                    on new { a = income.Symbol, b = income.Date } equals new { a = cash.Symbol, b = cash.Date }
                    where dates.Contains(income.Date)
                    && income.Symbol == symbol
                    select new ResultSet
                    {
                        Symbol = income.Symbol,
                        Equity = balance.TotalStockholdersEquity,
                        Debt = balance.TotalLiabilities,
                        NetIncome = income.NetIncome,
                        Roe = balance.TotalStockholdersEquity == 0
                           ? 0
                           : Math.Round(income.NetIncome * 100 / balance.TotalStockholdersEquity, 0),
                        ReinvestmentRate = income.NetIncome == 0
                           ? 0
                           : Math.Round(cash.CapitalExpenditure * -100 / income.NetIncome, 0)
                    }).ToList();

            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.RoeHistoryQuery, a => a.RoeHistory);
            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.ReinvestmentHistoryQuery, a => a.ReinvestmentHistory);
            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.IncrementalRoeQuery, a => a.IncrementalRoe);
            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.RevenueHistoryQuery, a => a.RevenueHistory);
            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.OperatingIncomeHistoryQuery, a => a.OperatingIncome);
            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.EpsHistoryQuery, a => a.Eps);
            resultSetList = AddHistoryData(resultSetList, parameters, QueryFactory.CashConversionQuery, a => a.CashConversionHistory);
            resultSetList = AddCompanyName(resultSetList);
            resultSetList = AddDebtEquityIncome(resultSetList);

            return resultSetList;
        }

        /// <summary>
        /// MainQuery
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private ResultSetList MainQuery<TKey>(CompounderQueryParams<TKey> parameters)
        {
            ReportProgress(100, 10, $"Retrieving companies with ROE > {parameters.RoeFrom}");

            var queryAsEnumerable = QueryAsEnumerable(parameters).OrderByDescending(parameters.OrderFunction).ToList();
            queryAsEnumerable = AddHistoryData(queryAsEnumerable, parameters, QueryFactory.RoeHistoryQuery, a => a.RoeHistory);
            queryAsEnumerable = AdjustToRoeGrowthKoef(queryAsEnumerable, parameters);

            var p = parameters;
            List<ResultSet> roeFiltered = p.Descending
                ? queryAsEnumerable.OrderByDescending(p.OrderFunction).Skip(p.CurrentPage * p.PageSize).Take(p.PageSize).ToList()
                : queryAsEnumerable.OrderBy(p.OrderFunction).Skip(p.CurrentPage * p.PageSize).Take(p.PageSize).ToList();
            ResultSetList resultSetList = new ResultSetList(roeFiltered);
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

            string sql = $@"select Symbol, Equity, Debt, NetIncome, Roe, ReinvestmentRate
                from ViewCompounder 
                where 1 = 1
                and Date in (@Dates)
                and Roe >= @RoeFrom
                and ReinvestmentRate >= @ReinvestmentRateFrom";

            string datesAsParam = CreateCommaSeparatedParams("@Dates", dates.Count);
            sql = sql.Replace("@Dates", datesAsParam);

            DataTable dataTable = null;

            var connection = DataContext.Database.GetDbConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            AddDoubleParameter(command, "@RoeFrom", DbType.Double, parameters.RoeFrom);
            AddDoubleParameter(command, "@ReinvestmentRateFrom", DbType.Double, parameters.ReinvestmentRateFrom);
            AddStringListParameter(command, "@Dates", DbType.String, dates);

            using (var reader = command.ExecuteReader())
            {
                dataTable = new DataTable();
                dataTable.Load(reader);

            }
            connection.Close();

            return DataTableToResultSetList(dataTable);
        }

        private string CreateCommaSeparatedParams(string paramBase, int count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sb.Append(paramBase + i.ToString());
                if (i < count - 1)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// AddStringListParameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="dates"></param>
        private void AddStringListParameter(DbCommand command, string name, DbType dbType, List<string> dates)
        {
            //var datesAsSql = "'" + dates.Aggregate((r, n) => r + "','" + n) + "'";


            for (int i = 0; i < dates.Count; i++)
            {
                string date = dates[i];
                var param = command.CreateParameter();
                param.ParameterName = name + i.ToString();
                param.DbType = dbType;
                param.Value = date;
                command.Parameters.Add(param);
            }

        }

        /// <summary>
        /// AddDoubleParameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        private void AddDoubleParameter(DbCommand command, string name, DbType dbType, double value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.DbType = dbType;
            param.Value = value;
            command.Parameters.Add(param);
        }

        /// <summary>
        /// DataTableToResultSetList
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private IEnumerable<ResultSet> DataTableToResultSetList(DataTable dataTable)
        {
            List<ResultSet> listOfResultSets = new List<ResultSet>();

            foreach (DataRow row in dataTable.Rows)
            {
                ResultSet resultSet = new ResultSet();

                resultSet.Symbol = (string)row["Symbol"];
                resultSet.Equity = row["Equity"] == DBNull.Value ? null : (double)row["Equity"];
                resultSet.Debt = row["Debt"] == DBNull.Value ? null : (double)row["Debt"];
                resultSet.NetIncome = row["NetIncome"] == DBNull.Value ? null : (double)row["NetIncome"];
                resultSet.Roe = row["Roe"] == DBNull.Value ? null : Math.Round((double)row["Roe"], 0);
                resultSet.ReinvestmentRate = row["ReinvestmentRate"] == DBNull.Value ? null : Math.Round((double)row["ReinvestmentRate"], 0);

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
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="inputResultSetList"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private List<ResultSet> AdjustToRoeGrowthKoef<TKey>(List<ResultSet> inputResultSetList, CompounderQueryParams<TKey> parameters)
        {
            if (parameters.RoeGrowthKoef == 0)
            {
                return inputResultSetList;
            }

            List<ResultSet> resultSetList = new List<ResultSet>();

            foreach (ResultSet resultSet in inputResultSetList)
            {
                if (resultSet.RoeHistory.Grows() >= parameters.RoeGrowthKoef)
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
