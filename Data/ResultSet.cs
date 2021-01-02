using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FmpAnalyzer.Data
{
    public class ResultSet
    {
        public ResultSet()
        {
            RoeHistory = new List<double>();
            RevenueHistory = new List<double>();
            ReinvestmentHistory = new List<double>();
            IncrementalRoe = new List<double>();
            OperatingIncome = new List<double>();
            Eps = new List<double>();
        }

        public string Symbol { get; set; }
        public string Name { get; set; }
        public double Roe { get; set; }
        public List<double> RoeHistory { get; set; }
        public List<double> RevenueHistory { get; set; }
        public double ReinvestmentRate { get; set; }
        public List<double> ReinvestmentHistory { get; set; }
        public List<double> IncrementalRoe { get; set; }
        public List<double> OperatingIncome { get; set; }
        public List<double> Eps { get; set; }

        
    }
}
