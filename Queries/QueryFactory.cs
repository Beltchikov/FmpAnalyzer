using FmpAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// QueryFactory
    /// </summary>
    public class QueryFactory
    {
        private static readonly object lockObject = new object();

        private static CompounderQuery _compounderQuery;
        private static WithBestRoeQuery _withBestRoeQuery;
        private static RoeHistoryQuery _roeHistoryQuery;
        private static ReinvestmentHistoryQuery _reinvestmentHistoryQuery;
        private static CompanyNameQuery _companyNameQuery;

        /// <summary>
        /// CompounderQuery
        /// </summary>
        public static CompounderQuery CompounderQuery
        {
            get
            {
                lock (lockObject)
                {
                    if (_compounderQuery == null)
                    {
                        _compounderQuery = new CompounderQuery(DataContext.Instance);
                    }
                    return _compounderQuery;
                }
            }
        }

        /// <summary>
        /// WithBestRoeQuery
        /// </summary>
        public static WithBestRoeQuery WithBestRoeQuery
        {
            get
            {
                lock (lockObject)
                {
                    if (_withBestRoeQuery == null)
                    {
                        _withBestRoeQuery = new WithBestRoeQuery(DataContext.Instance);
                    }
                    return _withBestRoeQuery;
                }
            }
        }

        /// <summary>
        /// RoeHistoryQuery
        /// </summary>
        public static RoeHistoryQuery RoeHistoryQuery
        {
            get
            {
                lock (lockObject)
                {
                    if (_roeHistoryQuery == null)
                    {
                        _roeHistoryQuery = new RoeHistoryQuery(DataContext.Instance);
                    }
                    return _roeHistoryQuery;
                }
            }
        }

        /// <summary>
        /// ReinvestmentHistoryQuery
        /// </summary>
        public static ReinvestmentHistoryQuery ReinvestmentHistoryQuery
        {
            get
            {
                lock (lockObject)
                {
                    if (_reinvestmentHistoryQuery == null)
                    {
                        _reinvestmentHistoryQuery = new ReinvestmentHistoryQuery(DataContext.Instance);
                    }
                    return _reinvestmentHistoryQuery;
                }
            }
        }

        /// <summary>
        /// CompanyNameQuery
        /// </summary>
        public static CompanyNameQuery CompanyNameQuery
        {
            get
            {
                lock (lockObject)
                {
                    if (_companyNameQuery == null)
                    {
                        _companyNameQuery = new CompanyNameQuery(DataContext.Instance);
                    }
                    return _companyNameQuery;
                }
            }
        }
    }
}
