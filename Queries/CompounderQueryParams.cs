using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer.Queries
{
    public class CompounderQueryParams
    {
        public string Date { get; set; }
        public double Roe { get; set; }
        public int HistoryDepthRoe { get; set; }
        public int GrowthGradRoe { get; set; }
        public int HistoryDepthReinvestment { get; set; }
        public int GrowthGradReinvestment { get; set; }
        public double ReinvestmentRate { get; set; }
        public double AverageReinvestmentRate { get; set; }

    }
}
