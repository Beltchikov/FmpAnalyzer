using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// QueryFactory
    /// </summary>
    public class QueryFactory
    {
        private static readonly object lockObject = new object();

        private static CompounderQuery _compounderQuery;

        /// <summary>
        /// CompounderQuery
        /// </summary>
        public static CompounderQuery CompounderQuery
        {
            get
            {
                lock (lockObject)
                {
                    if (_compounderQuery == null)
                    {
                        _compounderQuery = new CompounderQuery(Companies.Instance);
                    }
                    return _compounderQuery;
                }
            }
        }
    }
}
