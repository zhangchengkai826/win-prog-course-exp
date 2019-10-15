using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for ChapterContent5.xaml
    /// </summary>
    public partial class ChapterContent5 : UserControl
    {
        public ChapterContent5()
        {
            InitializeComponent();
        }
        private Process client;
        private Process server;

        private void OpenServer(object sender, RoutedEventArgs e)
        {
            server = new Process();
            server.StartInfo.FileName = "../../../zckserver-winform/bin/Debug/zckserver-winform.exe";
            // 不使用外壳程序
            server.StartInfo.UseShellExecute = false;
            // 不在新窗口中启动该进程
            server.StartInfo.CreateNoWindow = true;
            server.Start();
        }

        private void OpenClient(object sender, RoutedEventArgs e)
        {
            client = new Process();
            client.StartInfo.FileName = "../../../zckclient-winform/bin/Debug/zckclient-winform.exe";
            // 不使用外壳程序
            client.StartInfo.UseShellExecute = false;
            // 不在新窗口中启动该进程
            client.StartInfo.CreateNoWindow = true;
            client.Start();
        }

        private void CloseAll(object sender, RoutedEventArgs e)
        {
            try
            {
                server.Kill();
                client.Kill();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
