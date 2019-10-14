using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
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
using Microsoft.Win32;

namespace win_prog_course_exp
{
    /// <summary>
    /// Interaction logic for ChapterContent6.xaml
    /// </summary>
    public partial class ChapterContent6 : UserControl
    {
        private OleDbDataAdapter dataAdapter;
        private DataSet dataSet;
        public DataView View;
        private string pathOfFile;
        public ChapterContent6()
        {
            InitializeComponent();
        }

        private void OpenExcel(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Documents | *.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                pathOfFile = openFileDialog.FileName;

                var mCon = new OleDbConnection();
                mCon.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.16.0;data source=" + pathOfFile + ";Extended Properties=\"Excel 12.0;HDR=YES\";";
                mCon.Open();
                var sheetsTable = mCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                mCon.Close();

                var sheetNamesTable = (new DataView(sheetsTable)).ToTable(false, new string[] { "TABLE_NAME" });
                TableNamePresenter.DataContext = sheetNamesTable;
                
                MessageBox.Show("Finished!");
            }
        }

        private void OpenSheet(object sender, RoutedEventArgs e)
        {
            var dataRow = TableNamePresenter.SelectedItem as DataRowView;
            if(dataRow == null){
                return;
            }
            var sheetName = dataRow.Row.ItemArray[0].ToString();

            var mCon = new OleDbConnection();
            mCon.ConnectionString = @"Provider =Microsoft.ACE.OLEDB.16.0;data source=" + pathOfFile + ";Extended Properties=\"Excel 12.0;HDR=YES\";";
            mCon.Open();
            var strSelectQuery = "SELECT * FROM [" + sheetName + "]";
            dataAdapter = new OleDbDataAdapter(strSelectQuery, mCon);
            mCon.Close();

            dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            View = dataSet.Tables[0].DefaultView;
            DataPresenter.ItemsSource = View;
            MessageBox.Show(string.Format("数据表\"{0}\"已打开", sheetName));
        }

        private void AddNew(object sender, RoutedEventArgs e)
        {

        }

        private void Update(object sender, RoutedEventArgs e)
        {
            int rowId = 0;
            foreach (DataRow row in View.Table.Rows)
            {
                int colId = 0;
                foreach(var cell in row.ItemArray)
                {
                    dataSet.Tables[0].Rows[rowId].ItemArray[colId] = cell;
                    colId++;
                }
                rowId++;
            }
            dataAdapter.Update(dataSet);
        }

        private void Delete(object sender, RoutedEventArgs e)
        {

        }
    }
}
