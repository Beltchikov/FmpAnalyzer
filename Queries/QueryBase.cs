﻿using FmpAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer.Queries
{
    /// <summary>
    /// QueryBase
    /// </summary>
    public class QueryBase
    {
        public delegate void DatabaseActionDelegate(object sender, DatabaseActionEventArgs e);
        public event DatabaseActionDelegate DatabaseAction;

        QueryBase() { }

        /// <summary>
        /// QueryBase
        /// </summary>
        /// <param name="companies"></param>
        protected QueryBase(DataContext companies)
        {
            DataContext = companies;
        }

        /// <summary>
        /// ReportProgress
        /// </summary>
        /// <param name="max"></param>
        /// <param name="current"></param>
        /// <param name="message"></param>
        public void ReportProgress(int max, int current, string message)
        {
            DatabaseAction?.Invoke(this, new DatabaseActionEventArgs
            {
                Action = message,
                ProgressValue = current,
                MaxValue = max
            });
        }

        /// <summary>
        /// DataContext
        /// </summary>
        public DataContext DataContext { get; }
    }
}
