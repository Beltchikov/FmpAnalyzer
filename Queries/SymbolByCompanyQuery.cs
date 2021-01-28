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
    /// SymbolByCompanyQuery
    /// </summary>
    public class SymbolByCompanyQuery : QueryBase
    {
        public SymbolByCompanyQuery(DataContext dataContext) : base(dataContext) { }

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
            var queryAsEnumerable = QueryAsEnumerable(command, ResultSetFunctions.FindByCompany).ToList();
            if (!queryAsEnumerable.Any())
            {
                return string.Empty;
            }

            result = queryAsEnumerable.Select(q => q.Symbol + "\t" + q.Name).Distinct().Aggregate((r, n) => r + "\r\n" + n);

            return result;
        }
    }
}