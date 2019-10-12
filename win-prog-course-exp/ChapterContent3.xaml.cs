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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
