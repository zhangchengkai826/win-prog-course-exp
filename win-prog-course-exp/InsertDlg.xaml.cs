using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using System.Windows.Shapes;

namespace win_prog_course_exp
{
    /// <summary>
    /// Interaction logic for ExcelInsertDlg.xaml
    /// </summary>
    public partial class InsertDlg : Window
    {
        public ObservableCollection<Item> Items;
        public InsertDlg(DataColumnCollection cols)
        {
            InitializeComponent();
            Items = new ObservableCollection<Item>();

            foreach(DataColumn c in cols)
            {
                var itm = new Item() { ColumnName = c.ColumnName };
                Items.Add(itm);
            }

            ItemsPresenter.ItemsSource = Items;
        }
        public InsertDlg(string[] cols)
        {
            InitializeComponent();
            Items = new ObservableCollection<Item>();

            foreach (var colname in cols)
            {
                var itm = new Item() { ColumnName = colname };
                Items.Add(itm);
            }

            ItemsPresenter.ItemsSource = Items;
        }
        public class Item
        {
            public string ColumnName { get; set; }
            public object Value { get; set; }
        }

        private void BtnOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
