using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmpAnalyzer.Model
{
    /// <summary>
    /// Exchange
    /// </summary>
    public class Exchange
    {

        /// <summary>
        /// Exchange
        /// </summary>
        Exchange() 
        {
            ExchangesFmp = new List<string>();
        }
                
        public string Name { get; private set; }
        public bool Selected { get; set; }
        public List<string> ExchangesFmp { get; private set; }
        
        public static Exchange Nyse
        {
            get
            {
                return new Exchange
                {
                    Name = "NYSE",
                    Selected = true,
                    ExchangesFmp = new List<string>
                    { "New York Stock Exchange","NYSE Arca","NYSE","NYSE American","NYSEArca"}
                };
            }
        }

        public static Exchange Nasdaq
        {
            get
            {
                return new Exchange
                {
                    Name = "NASDAQ",
                    Selected = true,
                    ExchangesFmp = new List<string>
                    { "Nasdaq","Nasdaq Global Select","NASDAQ Capital Market","NASDAQ Global Market","NasdaqGS", "NasdaqCM", "NasdaqGM"}
                };
            }
        }

        public static Exchange Lse
        {
            get
            {
                return new Exchange
                {
                    Name = "LSE",
                    Selected = true,
                    ExchangesFmp = new List<string>
                    { "LSE"}
                };
            }
        }

        public static Exchange Hkse
        {
            get
            {
                return new Exchange
                {
                    Name = "HKSE",
                    Selected = true,
                    ExchangesFmp = new List<string>
                    { "HKSE"}
                };
            }
        }

        public static Exchange Asx
        {
            get
            {
                return new Exchange
                {
                    Name = "ASX",
                    Selected = true,
                    ExchangesFmp = new List<string>
                    { "ASX"}
                };
            }
        }

        public static Exchange Nse
        {
            get
            {
                return new Exchange
                {
                    Name = "NSE",
                    Selected = true,
                    ExchangesFmp = new List<string>
                    { "NSE"}
                };
            }
        }

        public static Exchange Canada
        {
            get
            {
                return new Exchange
                {
                    Name = "Canada",
                    Selected = true,
                    ExchangesFmp = new List<string>
                    { "Toronto", "YHD"}
                };
            }
        }

        public static Exchange Europe
        {
            get
            {
                return new Exchange
                {
                    Name = "Europe",
                    Selected = true,
                    ExchangesFmp = new List<string>
                    { "Paris", "XETRA", "BATS Exchange",
                    "MCX","Brussels","BATS",
                    "Amsterdam", "OSE", "SIX",
                    "NMS","Lisbon","NCM"}
                };
            }
        }


    }
}
