using FmpAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// HistoryQuery
    /// </summary>
    public abstract class HistoryQuery : QueryBase
    {
        public HistoryQuery(DataContext dataContext) : base(dataContext) { }

        public abstract List<double >Run(string symbol, string date, int historyDepth);
    }
}
