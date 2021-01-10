using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FmpAnalyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// OnStartup
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetupExceptionHandling();
        }

        /// <summary>
        /// SetupExceptionHandling
        /// </summary>
        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                MessageBox.Show(((Exception)e.ExceptionObject).ToString());
            };

            DispatcherUnhandledException += (s, e) =>
            {
                MessageBox.Show(((Exception)e.Exception).ToString());
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                MessageBox.Show(((Exception)e.Exception).ToString());
                e.SetObserved();
            };
        }
    }
}
