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
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace zckserver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, StringBuilder lParam);
        private const int DATA_RECV = 0x500;
        private HwndSource source;
        private IntPtr hWnd;
        public class OnDataRecvArgs: EventArgs
        {
            public string text;
        }
        public delegate void OnDataRecvHandler(object sender, OnDataRecvArgs args);
        public event OnDataRecvHandler OnDataRecv;
        public MainWindow()
        {
            InitializeComponent();
            OnDataRecv += new OnDataRecvHandler(ProcessRecvedData);
            hWnd = new WindowInteropHelper(this).EnsureHandle();
            source = HwndSource.FromHwnd(hWnd);
            source.AddHook(new HwndSourceHook(WndProc));
            Task.Factory.StartNew(() =>
            {
                var pipe = new NamedPipeServerStream("pipeZck", PipeDirection.In);
                while (true)
                {
                    pipe.WaitForConnection();
                    var reader = new StreamReader(pipe);
                    while (true)
                    {
                        var line = reader.ReadLine();
                        if(line == null)
                        {
                            break;
                        }
                        if(line.Length > 0)
                        {
                            var data = "收到来自客户端的数据： " + line + "\n";
                            SendMessage(hWnd, DATA_RECV, (IntPtr)0, new StringBuilder(data));
                        }
                    }
                    pipe.Disconnect();
                }
            });
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;
            switch (msg)
            {
                case DATA_RECV:
                    {
                        OnDataRecv(this, new OnDataRecvArgs() { text = Marshal.PtrToStringAuto(lParam) });
                        handled = true;
                        break;
                    }
            }
            return IntPtr.Zero;
        }
        private void ProcessRecvedData(object sender, OnDataRecvArgs args)
        {
            Log.Text += args.text;
        }
    }
}
