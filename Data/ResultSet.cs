using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FmpAnalyzer.Data
{
    public class ResultSet
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public double Roe { get; set; }
        
        //[Display( Name ="Reinvestment Rate")]
        [DisplayName("Reinvestment Rate")]
        public double ReinvestmentRate { get; set; }
    }
}
