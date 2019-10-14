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
    static class OleDbTypeMap
    {
        private static readonly Dictionary<Type, OleDbType> TypeMap = new Dictionary<Type, OleDbType> {
            {typeof(string), OleDbType.VarChar },
            {typeof(long), OleDbType.BigInt },
            {typeof(byte[]), OleDbType.Binary },
            {typeof(bool), OleDbType.Boolean },
            {typeof(decimal), OleDbType.Decimal },
            {typeof(DateTime), OleDbType.Date },
            {typeof(TimeSpan), OleDbType.DBTime },
            {typeof(double), OleDbType.Double },
            {typeof(Exception),OleDbType.Error },
            {typeof(Guid), OleDbType.Guid },
            {typeof(int), OleDbType.Integer },
            {typeof(float), OleDbType.Single },
            {typeof(short), OleDbType.SmallInt },
            {typeof(sbyte), OleDbType.TinyInt },
            {typeof(ulong), OleDbType.UnsignedBigInt },
            {typeof(uint), OleDbType.UnsignedInt },
            {typeof(ushort), OleDbType.UnsignedSmallInt },
            {typeof(byte), OleDbType.UnsignedTinyInt }
        };
        public static OleDbType GetType(Type type) => TypeMap[type];
    }
    /// <summary>
    /// Interaction logic for ChapterContent6.xaml
    /// </summary>
    public partial class ChapterContent6 : UserControl
    {
        private OleDbDataAdapter dataAdapter;
        private DataSet dataSet;
        private DataTable dataTable;
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
            mCon.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.16.0;data source=" + pathOfFile + ";Extended Properties=\"Excel 12.0;HDR=YES\";";
            mCon.Open();
            var strSelectQuery = "SELECT * FROM [" + sheetName + "]";
            dataAdapter = new OleDbDataAdapter(strSelectQuery, mCon);
            mCon.Close();

            dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            dataTable = dataSet.Tables[0];

            var updateCmdText = "UPDATE [" + sheetName + "] SET";
            int colId = 0;
            foreach(DataColumn col in dataTable.Columns)
            {
                updateCmdText += " " + col.ColumnName + "=?";
                if(colId < dataTable.Columns.Count - 1)
                {
                    updateCmdText += ",";
                }
                colId++;
            }
            updateCmdText += "WHERE " + dataTable.Columns[0].ColumnName + " IS NOT NULL AND " + dataTable.Columns[0].ColumnName + "=?";
            var updateCommand = new OleDbCommand(updateCmdText, mCon);
            foreach(DataColumn col in dataTable.Columns)
            {
                updateCommand.Parameters.Add("@" + col.ColumnName, OleDbTypeMap.GetType(col.DataType), col.MaxLength, col.ColumnName);
            }
            var whereParam = updateCommand.Parameters.Add("@" + dataTable.Columns[0].ColumnName, OleDbTypeMap.GetType(dataTable.Columns[0].DataType), dataTable.Columns[0].MaxLength, dataTable.Columns[0].ColumnName);
            whereParam.SourceVersion = DataRowVersion.Original;
            dataAdapter.UpdateCommand = updateCommand;

            View = dataTable.DefaultView;
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
                foreach (var cell in row.ItemArray)
                {
                    dataSet.Tables[0].Rows[rowId].ItemArray[colId] = cell;
                    colId++;
                }
                rowId++;
            }
            dataAdapter.Update(dataSet);
            MessageBox.Show("Finished!");
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            var rowId = DataPresenter.SelectedIndex;
            if(rowId != -1)
            {
                for (int i = 0; i < View.Table.Rows[rowId].ItemArray.Count(); i++)
                {
                    View.Table.Rows[rowId][i] = DBNull.Value;
                    dataSet.Tables[0].Rows[rowId][i] = DBNull.Value;
                }
                dataAdapter.Update(dataSet);
                MessageBox.Show("Finished!");
            }
        }
    }
}
