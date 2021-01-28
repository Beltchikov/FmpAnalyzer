using FmpAnalyzer.Data;
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
        public delegate void DatabaseActionDelegate(object sender, DatabaseActionEventArgs e);
        public event DatabaseActionDelegate DatabaseAction;

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
        /// ReportProgress
        /// </summary>
        /// <param name="max"></param>
        /// <param name="current"></param>
        /// <param name="message"></param>
        public void ReportProgress(int max, int current, string message)
        {
            DatabaseAction?.Invoke(this, new DatabaseActionEventArgs
            {
                Action = message,
                ProgressValue = current,
                MaxValue = max
            });
        }

        /// <summary>
        /// AddStringListParameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="dates"></param>
        protected void AddStringListParameter(DbCommand command, string name, DbType dbType, List<string> dates)
        {
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
        /// AddStringParameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        protected void AddStringParameter(DbCommand command, string name, DbType dbType, string value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.DbType = dbType;
            param.Value = value;
            command.Parameters.Add(param);
        }

        /// <summary>
        /// AddDoubleParameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        protected void AddDoubleParameter(DbCommand command, string name, DbType dbType, double value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.DbType = dbType;
            param.Value = value;
            command.Parameters.Add(param);
        }
    }
}
