using FmpAnalyzer.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using FmpDataContext;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// CompanyNameQuery
    /// </summary>
    public class CompanyNameQuery : QueryBase
    {
        public CompanyNameQuery(DataContext dataContext) : base(dataContext) { }

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="inputResultSetList"></param>
        /// <returns></returns>
        public ResultSetList Run(ResultSetList inputResultSetList)
        {
            var symbols = inputResultSetList.ResultSets.Select(i => i.Symbol).ToList();
            var listSymbolNameQuery = DataContext.Stocks.Where(s => symbols.Contains(s.Symbol))
                .Select(a => new KeyValuePair<string, string>(a.Symbol, a.Name)).ToList();
            var dictSymbolName = new Dictionary<string, string>(listSymbolNameQuery);

            for (int i = 0; i < inputResultSetList.ResultSets.Count(); i++)
            {
                if (dictSymbolName.ContainsKey(inputResultSetList.ResultSets[i].Symbol))
                {
                    inputResultSetList.ResultSets[i].Name = dictSymbolName[inputResultSetList.ResultSets[i].Symbol];
                }
            }

            return inputResultSetList;
        }
    }
}
