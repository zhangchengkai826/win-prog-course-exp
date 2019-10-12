using System;
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
using System.IO.Pipes;
using System.IO;

namespace zckserver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Task.Factory.StartNew(() =>
            {
                var pipe = new NamedPipeServerStream("pipeZck", PipeDirection.In);
                while (true)
                {
                    pipe.WaitForConnection();
                    var reader = new StreamReader(pipe);
                    while (true)
                    {
                        try
                        {
                            var line = reader.ReadLine();
                            if(line.Length > 0)
                            {
                                Log.Text += "收到来自客户端的数据： " + line + "\n";
                            }
                        }
                        catch(IOException e)
                        {
                            Log.Text += e.Message + "\n";
                            break;
                        }
                    }
                    pipe.Close();
                }
            });
        }
    }
}
