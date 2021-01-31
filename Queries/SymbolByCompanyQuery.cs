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
        /// <param name="company"></param>
        /// <returns></returns>
        public List<string> FindByCompany(string company)
        {
            var command = DbCommands.FindByCompany(DataContext.Database.GetDbConnection(), Sql.FindByCompany(company), company);
            var queryAsEnumerable = QueryAsEnumerable(command, ResultSetFunctions.FindByCompany).ToList();
            if (!queryAsEnumerable.Any())
            {
                return new List<string>();
            }

            var result = queryAsEnumerable.Select(q => q.Symbol + "\t" + q.Name).Distinct().ToList();
            return result;
        }
    }
}