using FmpAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// CompounderQueryParams
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class CompounderQueryParams<TKey> : CompounderCountQueryParams
    {
        public Func<ResultSet, TKey> OrderFunction;
        public bool Descending { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
    }
}
