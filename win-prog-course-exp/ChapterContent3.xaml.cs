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
    /// Interaction logic for ChapterContent3.xaml
    /// </summary>
    public partial class ChapterContent3 : UserControl
    {
        private string mac;
        private int getMacOutputLineId;
        private Process client;
        private Process server;
        public ChapterContent3()
        {
            InitializeComponent();
        }

        private void GetMac(object sender, RoutedEventArgs e)
        {
            mac = "";
            getMacOutputLineId = 0;
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            // 不使用外壳程序
            process.StartInfo.UseShellExecute = false;
            // 不在新窗口中启动该进程
            process.StartInfo.CreateNoWindow = true;
            // 重定向输入流
            process.StartInfo.RedirectStandardInput = true;
            // 重定向输出流
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += new DataReceivedEventHandler(GetMacOutputHandler);
            string strCmd = "getmac /FO table /NH";
            process.Start();
            process.BeginOutputReadLine();
            process.StandardInput.WriteLine(strCmd);
            process.StandardInput.WriteLine("exit");
            process.WaitForExit();
            process.Close();
            Mac.Text = "网卡mac为： " + mac;
        }

        private void GetMacOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if(getMacOutputLineId == 5)
            {
                mac = outLine.Data.Split()[0];
            }
            getMacOutputLineId++;
        }

        private void Shutdown(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("是否关机？", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                // 不使用外壳程序
                process.StartInfo.UseShellExecute = false;
                // 不在新窗口中启动该进程
                process.StartInfo.CreateNoWindow = true;
                // 重定向输入流
                process.StartInfo.RedirectStandardInput = true;
                string strCmd = "shutdown /s /t 0 /f";
                process.Start();
                process.StandardInput.WriteLine(strCmd);
            }
        }

        private void Restart(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("是否重启？", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                // 不使用外壳程序
                process.StartInfo.UseShellExecute = false;
                // 不在新窗口中启动该进程
                process.StartInfo.CreateNoWindow = true;
                // 重定向输入流
                process.StartInfo.RedirectStandardInput = true;
                string strCmd = "shutdown /r /t 0 /f";
                process.Start();
                process.StandardInput.WriteLine(strCmd);
            }
        }

        private void OpenServer(object sender, RoutedEventArgs e)
        {
            server = new Process();
            server.StartInfo.FileName = "../../../zckserver/bin/Debug/zckserver.exe";
            // 不使用外壳程序
            server.StartInfo.UseShellExecute = false;
            // 不在新窗口中启动该进程
            server.StartInfo.CreateNoWindow = true;
            server.Start();
        }

        private void OpenClient(object sender, RoutedEventArgs e)
        {
            client = new Process();
            client.StartInfo.FileName = "../../../zckclient/bin/Debug/zckclient.exe";
            // 不使用外壳程序
            client.StartInfo.UseShellExecute = false;
            // 不在新窗口中启动该进程
            client.StartInfo.CreateNoWindow = true;
            client.Start();
        }

        private void KillAllProcess(object sender, RoutedEventArgs e)
        {
            try
            {
                server.Kill();
                client.Kill();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
