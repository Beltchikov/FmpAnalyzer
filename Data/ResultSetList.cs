using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmpAnalyzer.Data
{
    /// <summary>
    /// ResultSetList
    /// </summary>
    public class ResultSetList
    {
        private List<ResultSet> _resultSetList;

        ResultSetList() { }

        /// <summary>
        /// ResultSetList
        /// </summary>
        /// <param name="resultSetList"></param>
        public ResultSetList(List<ResultSet> resultSetList)
        {
            _resultSetList = resultSetList;
        }

        /// <summary>
        /// CountTotal
        /// </summary>
        public int CountTotal { get; set; }

        /// <summary>
        /// ResultSets
        /// </summary>
        public List<ResultSet> ResultSets
        {
            get
            {
                return _resultSetList;
            }
        }
    }
}
