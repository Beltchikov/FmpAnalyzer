﻿using System;
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
                        _compounderQuery = new CompounderQuery(Companies.Instance);
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
                        _withBestRoeQuery = new WithBestRoeQuery(Companies.Instance);
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
                        _roeHistoryQuery = new RoeHistoryQuery(Companies.Instance);
                    }
                    return _roeHistoryQuery;
                }
            }
        }
    }
}
