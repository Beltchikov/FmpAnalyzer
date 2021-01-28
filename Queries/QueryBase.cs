using FmpAnalyzer.Data;
using FmpDataContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
        /// DataContext
        /// </summary>
        public DataContext DataContext { get; }

        /// <summary>
        /// QueryBase
        /// </summary>
        /// <param name="dataContext"></param>
        protected QueryBase(DataContext dataContext)
        {
            DataContext = dataContext;
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
    }
}
