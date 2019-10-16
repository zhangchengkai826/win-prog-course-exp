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
using MySql.Data;
using MySql.Data.MySqlClient;

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
        private OleDbDataAdapter excelAdapter;
        private DataSet excelDataSet;
        private DataTable excelDataTable;
        public DataView excelView;
        private string pathOfExcelFile;
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
                pathOfExcelFile = openFileDialog.FileName;

                var mCon = new OleDbConnection();
                mCon.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.16.0;data source=" + pathOfExcelFile + ";Extended Properties=\"Excel 12.0;HDR=YES\";";
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
            mCon.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.16.0;data source=" + pathOfExcelFile + ";Extended Properties=\"Excel 12.0;HDR=YES\";";
            mCon.Open();
            var strSelectQuery = "SELECT * FROM [" + sheetName + "]";
            excelAdapter = new OleDbDataAdapter(strSelectQuery, mCon);
            mCon.Close();

            excelDataSet = new DataSet();
            excelAdapter.Fill(excelDataSet);
            excelDataTable = excelDataSet.Tables[0];

            var cmdBuilder = new OleDbCommandBuilder(excelAdapter);
            var insertCmdText = cmdBuilder.GetInsertCommand().CommandText;
            excelAdapter.InsertCommand = new OleDbCommand(insertCmdText.Replace(sheetName, "[" + sheetName + "]"), mCon);
            foreach (DataColumn col in excelDataTable.Columns)
            {
                excelAdapter.InsertCommand.Parameters.Add("@" + col.ColumnName, OleDbTypeMap.GetType(col.DataType), col.MaxLength, col.ColumnName);
            }

            var updateCmdText = "UPDATE [" + sheetName + "] SET";
            int colId = 0;
            foreach(DataColumn col in excelDataTable.Columns)
            {
                updateCmdText += " " + col.ColumnName + "=?";
                if(colId < excelDataTable.Columns.Count - 1)
                {
                    updateCmdText += ",";
                }
                colId++;
            }
            updateCmdText += "WHERE " + excelDataTable.Columns[0].ColumnName + " IS NOT NULL AND " + excelDataTable.Columns[0].ColumnName + "=?";
            var updateCommand = new OleDbCommand(updateCmdText, mCon);
            foreach(DataColumn col in excelDataTable.Columns)
            {
                updateCommand.Parameters.Add("@" + col.ColumnName, OleDbTypeMap.GetType(col.DataType), col.MaxLength, col.ColumnName);
            }
            var whereParam = updateCommand.Parameters.Add("@" + excelDataTable.Columns[0].ColumnName, OleDbTypeMap.GetType(excelDataTable.Columns[0].DataType), excelDataTable.Columns[0].MaxLength, excelDataTable.Columns[0].ColumnName);
            whereParam.SourceVersion = DataRowVersion.Original;
            excelAdapter.UpdateCommand = updateCommand;

            excelView = excelDataTable.DefaultView;
            DataPresenter.ItemsSource = excelView;
            MessageBox.Show(string.Format("数据表\"{0}\"已打开", sheetName));
        }

        private void ExcelAddNew(object sender, RoutedEventArgs e)
        {
            if(excelView == null)
            {
                return;
            }
            var dlg = new InsertDlg(excelView.Table.Columns);
            if (dlg.ShowDialog() == true)
            {
                var row = excelView.Table.NewRow();
                int colId = 0;
                foreach (var itm in dlg.Items)
                {
                    if(itm.Value == null)
                    {
                        if(excelView.Table.Columns[colId].DataType == typeof(string))
                        {
                            row[colId] = "";
                        }
                        else
                        {
                            row[colId] = Activator.CreateInstance(excelView.Table.Columns[colId].DataType);
                        }
                    }
                    else
                    {
                        row[colId] = itm.Value;
                    }
                    colId++;
                }
                excelView.Table.Rows.Add(row);
                excelAdapter.Update(excelDataSet);
                MessageBox.Show("Finished!");
            }
        }

        private void ExcelUpdate(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show(string.Format("{0} rows successfully updated!", excelAdapter.Update(excelDataSet)));
            }
            catch(DBConcurrencyException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExcelDelete(object sender, RoutedEventArgs e)
        {
            var rowId = DataPresenter.SelectedIndex;
            if(rowId != -1)
            {
                for (int i = 0; i < excelView.Table.Rows[rowId].ItemArray.Count(); i++)
                {
                    excelView.Table.Rows[rowId][i] = DBNull.Value;
                }
                excelAdapter.Update(excelDataSet);
                MessageBox.Show("Finished!");
            }
        }
        private string mysqlServer = "localhost";
        private string mysqlDefaultDB = "sys";
        private string mysqlUser;
        private string mysqlPw;
        private bool mysqlIsDBInstanceSelected = false;
        private string mysqlCurDB;
        private string mysqlCurConnStr;
        private MySqlConnection mysqlCurCon;
        private string mysqlCurTbl;
        private MySqlDataAdapter mysqlAdapter;
        private DataSet mysqlDataSet;
        private void FindDBInstances(object sender, RoutedEventArgs e)
        {
            var dlg = new PasswdDlg();
            if (dlg.ShowDialog() == true)
            {
                mysqlUser = dlg.InputUser.Text;
                mysqlPw = dlg.InputPw.Password;
                mysqlCurConnStr = "SERVER=" + mysqlServer + ";" + "DATABASE=" + mysqlDefaultDB + ";" + "UID=" + mysqlUser + ";" + "PASSWORD=" + mysqlPw + ";";
                try
                {
                    var connection = new MySqlConnection(mysqlCurConnStr);
                    connection.Open();
                    var schema = connection.GetSchema("Databases");
                    connection.Close();
                    var dbNames = (new DataView(schema)).ToTable(false, new string[] { "database_name" });
                    dbNames.Columns[0].ColumnName = "数据库实例名";
                    Selector.ItemsSource = dbNames.DefaultView;
                    SelectBtnLbl.Text = "选择数据库实例";
                    mysqlIsDBInstanceSelected = false;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SelectDBInstanceOrTable(object sender, RoutedEventArgs e)
        {
            if (!mysqlIsDBInstanceSelected)
            {
                var dataRow = Selector.SelectedItem as DataRowView;
                if (dataRow == null)
                {
                    return;
                }
                mysqlCurDB = dataRow.Row.ItemArray[0].ToString();

                mysqlCurConnStr = "SERVER=" + mysqlServer + ";" + "DATABASE=" + mysqlCurDB + ";" + "UID=" + mysqlUser + ";" + "PASSWORD=" + mysqlPw + ";";
                mysqlCurCon = new MySqlConnection(mysqlCurConnStr);
                mysqlCurCon.Open();
                var cmdStr = "show tables";
                var cmd = new MySqlCommand(cmdStr, mysqlCurCon);
                var reader = cmd.ExecuteReader();
                var curView = Selector.ItemsSource as DataView;
                curView.Table.Columns[0].ColumnName = "数据表名";
                curView.Table.Rows.Clear();
                while (reader.Read())
                {
                    var row = curView.Table.NewRow();
                    row[0] = reader.GetString(0);
                    curView.Table.Rows.Add(row);
                }
                reader.Close();
                mysqlCurCon.Close();
                Selector.ItemsSource = null;
                Selector.ItemsSource = curView;
                SelectBtnLbl.Text = "打开数据表";
                mysqlIsDBInstanceSelected = true;
                MessageBox.Show(string.Format("连接至数据库实例\"{0}\"", mysqlCurDB));
            }
            else
            {
                var dataRow = Selector.SelectedItem as DataRowView;
                if (dataRow == null)
                {
                    return;
                }
                mysqlCurTbl = dataRow.Row.ItemArray[0].ToString();

                mysqlCurCon.Open();
                var strSelectQuery = "SELECT * FROM " + mysqlCurTbl;
                mysqlAdapter = new MySqlDataAdapter(strSelectQuery, mysqlCurCon);
                mysqlCurCon.Close();

                mysqlDataSet = new DataSet();
                mysqlAdapter.Fill(mysqlDataSet);
                var cmdBuilder = new MySqlCommandBuilder(mysqlAdapter);
                try
                {
                    mysqlAdapter.UpdateCommand = cmdBuilder.GetUpdateCommand();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                DBDataPresenter.ItemsSource = mysqlDataSet.Tables[0].DefaultView;
            }
        }

        private void DBInsert(object sender, RoutedEventArgs e)
        {
            if(mysqlDataSet == null)
            {
                return;
            }
            var dlg = new InsertDlg(mysqlDataSet.Tables[0].Columns);
            if (dlg.ShowDialog() == true)
            {
                var row = mysqlDataSet.Tables[0].NewRow();
                int colId = 0;
                foreach (var itm in dlg.Items)
                {
                    if (itm.Value == null)
                    {
                        row[colId] = DBNull.Value;
                    }
                    else
                    {
                        row[colId] = itm.Value;
                    }
                    colId++;
                }
                mysqlDataSet.Tables[0].Rows.Add(row);
                mysqlAdapter.Update(mysqlDataSet);
                MessageBox.Show("Inserted!");
            }
        }

        private void DBUpdate(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format("{0} rows successfully updated!", mysqlAdapter.Update(mysqlDataSet)));
        }

        private void DBDelete(object sender, RoutedEventArgs e)
        {
            var rowId = DBDataPresenter.SelectedIndex;
            if (rowId != -1)
            {
                mysqlDataSet.Tables[0].Rows[rowId].Delete();
                mysqlAdapter.Update(mysqlDataSet);
                MessageBox.Show("Deleted!");
            }
        }
    }
}
