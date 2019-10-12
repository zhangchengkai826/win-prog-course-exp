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
        public ChapterContent3()
        {
            InitializeComponent();
        }

        private void GetMac(object sender, RoutedEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            // 是否使用外壳程序   
            process.StartInfo.UseShellExecute = false;
            // 是否在新窗口中启动该进程的值   
            process.StartInfo.CreateNoWindow = true;
            // 重定向输入流  
            process.StartInfo.RedirectStandardInput = true;
            // 重定向输出流
            process.StartInfo.RedirectStandardOutput = true;
            //使ping命令执行九次 
            string strCmd = "getmac /FO table /NH";
            process.Start();
            process.StandardInput.WriteLine(strCmd);
            process.StandardInput.WriteLine("exit");
            // 获取输出信息   
            var output = process.StandardOutput.ReadToEnd();
            Mac.Text = output;
            process.WaitForExit();
            process.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
