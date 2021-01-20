using FmpAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer.Queries
{
    public class CompounderQueryParams<TKey> : CompounderCountQueryParams
    {
       public Func<ResultSet, TKey> OrderFunction;
    }
}
