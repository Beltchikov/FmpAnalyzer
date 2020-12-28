using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer
{
    /// <summary>
    /// DatabaseActionEventArgs
    /// </summary>
    public class DatabaseActionEventArgs
    {
        public string Action { get; set; }
        public int MaxValue { get; set; }
        public int ProgressValue { get; set; }
    }
}
