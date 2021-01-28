using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace FmpAnalyzer.Queries
{
    public class DbCommandsBase
    {
        /// <summary>
        /// AddStringListParameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="dates"></param>
        protected static void AddStringListParameter(DbCommand command, string name, DbType dbType, List<string> dates)
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
        protected static void AddStringParameter(DbCommand command, string name, DbType dbType, string value)
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
        protected static void AddDoubleParameter(DbCommand command, string name, DbType dbType, double value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.DbType = dbType;
            param.Value = value;
            command.Parameters.Add(param);
        }
    }
}