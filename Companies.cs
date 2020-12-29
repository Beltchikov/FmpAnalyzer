using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmpAnalyzer
{
    /// <summary>
    /// Companies
    /// </summary>
    public class Companies : DataContext
    {
        private static Companies _companies;
        private static readonly object lockObject = new object();

        Companies() { }

        /// <summary>
        /// Instance
        /// </summary>
        public new static Companies Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (_companies == null)
                    {
                        _companies = new Companies();
                    }
                    return _companies;
                }
            }
        }

        

        

        
    }
}


