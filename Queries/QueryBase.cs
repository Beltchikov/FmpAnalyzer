using FmpAnalyzer.ResultSet;
using FmpDataContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// QueryBase
    /// </summary>
    public class QueryBase
    {
        QueryBase() { }

        /// <summary>
        /// DataContext
        /// </summary>
        public DataContext DataContext { get; }

        /// <summary>
        /// QueryBase
        /// </summary>
        /// <param name="dataContext"></param>
        protected QueryBase(DataContext dataContext)
        {
            DataContext = dataContext;
        }

        /// <summary>
        /// QueryAsEnumerable
        /// </summary>
        /// <param name="command"></param>
        /// <param name="resultSetFunction"></param>
        /// <returns></returns>
        protected IEnumerable<ResultSet.ResultSet> QueryAsEnumerable(DbCommand command, Func<DataTable, IEnumerable<ResultSet.ResultSet>> resultSetFunction)
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
    }
}
