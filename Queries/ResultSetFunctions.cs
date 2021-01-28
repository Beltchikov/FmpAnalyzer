using FmpAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmpAnalyzer.Queries
{
    public class ResultSetFunctions
    {
        /// <summary>
        /// ResultsetFunctionCompounder
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static IEnumerable<ResultSet> Compounder(DataTable dataTable)
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
        public static IEnumerable<ResultSet> FindBySymbol(DataTable dataTable)
        {
            return Compounder(dataTable);
        }

        /// <summary>
        /// ResultsetFunctionFindByCompany
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static IEnumerable<ResultSet> FindByCompany(DataTable dataTable)
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
    }
}
