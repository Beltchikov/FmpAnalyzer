using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// QueryBase
    /// </summary>
    public class QueryBase
    {
        private Companies _companies;

        QueryBase() { }

        /// <summary>
        /// QueryBase
        /// </summary>
        /// <param name="companies"></param>
        protected QueryBase(Companies companies)
        {
            _companies = companies;
        }

        /// <summary>
        /// Companies
        /// </summary>
        public Companies Companies
        {
            get
            {
                return _companies;
            }
        }
    }
}
