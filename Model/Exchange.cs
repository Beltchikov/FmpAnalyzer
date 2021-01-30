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
                    Selected = true
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
                    Selected = true
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
                    Selected = true
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
                    Selected = true
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
                    Selected = true
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
                    Selected = true
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
                    Selected = true
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
                    Selected = true
                };
            }
        }


    }
}
