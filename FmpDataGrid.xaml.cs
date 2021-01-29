using FmpAnalyzer.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FmpAnalyzer
{
    /// <summary>
    /// Interaction logic for FmpDataGrid.xaml
    /// </summary>
    public partial class FmpDataGrid : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty;

        static FmpDataGrid()
        {
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(FmpDataGrid), new PropertyMetadata(default(IEnumerable)));
        }

        public FmpDataGrid()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ItemsSource
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// gridCompounder_CopyingRowClipboardContent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridCompounder_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            int columnIndex = gridCompounder.CurrentCell.Column.DisplayIndex;
            var column = gridCompounder.Columns[columnIndex];

            var companyName = ((ResultSet)e.Item).Name;
            var cellContent = new DataGridClipboardCellContent(e.Item, column, companyName);
            
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(cellContent);
        }
    }
}
