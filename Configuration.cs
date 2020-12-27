using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer
{
    public class Configuration
    {
        private static IConfiguration _configuration;
        private static readonly object lockObject = new object();

        Configuration() { }

        public static IConfiguration Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (_configuration == null)
                    {
                        _configuration = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
                    }
                    return _configuration;
                }
            }
        }
    }
}
