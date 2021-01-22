using FmpAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer.Queries
{
    public class CompounderCountQueryParams : HistoryQueryParams
    {
        public int YearTo { get; internal set; }
        public double Roe { get; set; }
        public double ReinvestmentRate { get; set; }
        public int RoeGrowthKoef { get; internal set; }
    }
}
