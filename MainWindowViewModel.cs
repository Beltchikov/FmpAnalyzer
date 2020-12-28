using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace FmpAnalyzer
{
    public class MainWindowViewModel : DependencyObject
    {
        public static readonly DependencyProperty ConnectionStringProperty;
        public RelayCommand CommandGo { get; set; }

        static MainWindowViewModel()
        {
            ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(String.Empty));
        }

        public MainWindowViewModel()
        {
            ConnectionString = Configuration.Instance["ConnectionString"];

            CommandGo = new RelayCommand(p => { OnCommandGo(p); });
        }

        public string ConnectionString
        {
            get { return (string)GetValue(ConnectionStringProperty); }
            set { SetValue(ConnectionStringProperty, value); }
        }

        private void OnCommandGo(object p)
        {
            //var topStocks = DataContext.Instance.Stocks.All(s => 1== 1).Take(10).Select(s => s.)

        }

    }
}
