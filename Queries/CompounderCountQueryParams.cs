using FmpAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer.Queries
{
    public class CompounderCountQueryParams : HistoryQueryParams
    {
        public int YearTo { get; internal set; }
        public double RoeFrom { get; set; }
        public double ReinvestmentRateFrom { get; set; }
        public double RoeTo { get; internal set; }
        public double ReinvestmentRateTo { get; internal set; }
        public int RoeGrowthKoef { get; internal set; }
        public int RevenueGrowthKoef { get; internal set; }
        public int EpsGrowthKoef { get; internal set; }
        public double DebtEquityRatioFrom { get; internal set; }
        public double DebtEquityRatioTo { get; internal set; }
    }
}
