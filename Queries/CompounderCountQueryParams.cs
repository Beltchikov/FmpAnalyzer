using FmpAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer.Queries
{
    public class CompounderCountQueryParams
    {
        public int YearFrom { get; internal set; }
        public int YearTo { get; internal set; }
        public List<string> Dates { get; set; }
        public double Roe { get; set; }
        public double ReinvestmentRate { get; set; }
        public int HistoryDepth { get; internal set; }
        public string Symbol { get; internal set; }
    }
}
