using System;
using System.Collections.Generic;
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

namespace win_prog_course_exp
{
    /// <summary>
    /// Interaction logic for ChapterContent6.xaml
    /// </summary>
    public partial class ChapterContent6 : UserControl
    {
        private DataSet data;
        public ChapterContent6()
        {
            InitializeComponent();
            data = new DataSet();
        }

        private void OpenExcel(object sender, RoutedEventArgs e)
        {
            var pathOfFile = @"C:\Users\andys\Desktop\hw6-test\table.xlsx";
            var sheetName = "2017级$";

            var mCon = new OleDbConnection();
            mCon.ConnectionString = @"Provider =Microsoft.ACE.OLEDB.16.0;data source=" + pathOfFile + ";Extended Properties=\"Excel 12.0;HDR=YES\";";
            mCon.Open();
            var strSelectQuery = "SELECT * FROM [" + sheetName + "]";
            var DataAdapter = new OleDbDataAdapter(strSelectQuery, mCon);
            DataAdapter.Fill(data);
            mCon.Close();

            DataPresenter.DataContext = data.Tables[0].DefaultView;
            MessageBox.Show("Finished!");
        }
    }
}
