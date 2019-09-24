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
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

namespace win_prog_course_exp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var root = new RegTreeViewItem(RegTreeViewItemType.COMPUTER) { Title = "Computer" };
            root.Items.Add(new RegTreeViewItem(RegTreeViewItemType.FOLDER) { Title = "HKEY_CLASSES_ROOT", hKey = (IntPtr)0x80000000 });
            root.Items.Add(new RegTreeViewItem(RegTreeViewItemType.FOLDER) { Title = "HKEY_CURRENT_USER", hKey = (IntPtr)0x80000001 });
            root.Items.Add(new RegTreeViewItem(RegTreeViewItemType.FOLDER) { Title = "HKEY_LOCAL_MACHINE", hKey = (IntPtr)0x80000002 });
            root.Items.Add(new RegTreeViewItem(RegTreeViewItemType.FOLDER) { Title = "HKEY_USERS", hKey = (IntPtr)0x80000003 });
            root.Items.Add(new RegTreeViewItem(RegTreeViewItemType.FOLDER) { Title = "HKEY_CURRENT_CONFIG", hKey = (IntPtr)0x80000005 });
            treeView.Items.Add(root);
        }

        [DllImport("regzck.dll", EntryPoint = "regList", CallingConvention = CallingConvention.StdCall)]
        public static extern void regList(IntPtr hKey, out IntPtr subKeyNames, out int pcSubKeys);

        [DllImport("regzck.dll", EntryPoint = "regOpen", CallingConvention = CallingConvention.StdCall)]
        public static extern void regOpen(IntPtr parentKey, KeyName name, out IntPtr output);

        public static readonly IntPtr[] hKeyPredefined = { (IntPtr)0x80000000, (IntPtr)0x80000001, (IntPtr)0x80000002, (IntPtr)0x80000003, (IntPtr)0x80000005 };

        private static TreeViewItem getParent(TreeViewItem item)
        {
            var parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as TreeViewItem;
        }

        private void TreeView_Expanded(object sender, RoutedEventArgs e)
        {
            var tvi = e.OriginalSource as TreeViewItem;
            if (tvi != null)
            {
                var context = tvi.DataContext as RegTreeViewItem;
                if(context != null)
                {
                    if(context.Title != "Computer" && context.Items.Count == 0)
                    {
                        if(!hKeyPredefined.Contains(context.hKey))
                        {
                            var name = new KeyName() { achKey = context.Title };
                            IntPtr output;
                            regOpen((getParent(tvi).DataContext as RegTreeViewItem).hKey, name, out output);
                            context.hKey = output;
                        }

                        IntPtr subKeyNamesUnmanaged;
                        int cSubKeys;
                        regList(context.hKey, out subKeyNamesUnmanaged, out cSubKeys);

                        var subKeyNames = new KeyName[cSubKeys];
                        var keyNameStride = Marshal.SizeOf(typeof(KeyName));
                        for (int i = 0; i < cSubKeys; i++)
                        {
                            var p = new IntPtr(subKeyNamesUnmanaged.ToInt64() + i * keyNameStride);
                            subKeyNames[i] = (KeyName)Marshal.PtrToStructure(p, typeof(KeyName));
                        }

                        for(int i = 0; i < cSubKeys; i++)
                        {
                            context.Items.Add(new RegTreeViewItem(RegTreeViewItemType.FOLDER) { Title = subKeyNames[i].achKey });
                        }
                    }
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct KeyName
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string achKey;
    }

    public enum RegTreeViewItemType { COMPUTER, FOLDER, FOLDER_OPEN, }

    public class RegTreeViewItem
    {
        public RegTreeViewItem(RegTreeViewItemType Type)
        {
            Items = new ObservableCollection<RegTreeViewItem>();
            this.Type = Type;

            switch (Type)
            {
                case RegTreeViewItemType.COMPUTER:
                    {
                        Icon = new BitmapImage(new Uri("Resources/icon/computer.png", UriKind.Relative));
                        break;
                    }
                case RegTreeViewItemType.FOLDER:
                    {
                        Icon = new BitmapImage(new Uri("Resources/icon/folder.png", UriKind.Relative));
                        break;
                    }
            }
        }

        public ObservableCollection<RegTreeViewItem> Items { get; set; }

        public string Title { get; set; }

        public ImageSource Icon { get; set; }

        public RegTreeViewItemType Type { get; set; }

        public IntPtr hKey { get; set; }
    }
}
