using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer.Queries
{
    public class CompounderQueryParams
    {
        public string Date { get; set; }
        public double Roe { get; set; }
        public double ReinvestmentRate { get; set; }
        public int HistoryDepth { get; internal set; }
        public string Symbol { get; internal set; }
    }
}
