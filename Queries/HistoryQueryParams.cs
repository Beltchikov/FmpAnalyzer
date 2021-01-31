using FmpAnalyzer.ResultSet;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer.Queries
{
    public class HistoryQueryParams
    {
        public int YearFrom { get; internal set; }
        public List<string> Dates { get; set; }
        public int HistoryDepth { get; internal set; }
    }
}
