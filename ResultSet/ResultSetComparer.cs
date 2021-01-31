using FmpAnalyzer.ResultSet;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace FmpAnalyzer.ResultSet
{
    /// <summary>
    /// ResultSetComparer
    /// </summary>
    public class ResultSetComparer : IEqualityComparer<ResultSet>
    {
        public bool Equals(ResultSet x, ResultSet y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Symbol.ToUpper() == y.Symbol.ToUpper();
        }

        public int GetHashCode([DisallowNull] ResultSet obj)
        {
            int hash = Encoding.Unicode.GetBytes(obj.Symbol.ToUpper()).Aggregate((r, n) => (byte)(r + n));
            return hash;
        }
    }
}