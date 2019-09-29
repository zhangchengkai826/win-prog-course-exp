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
using System.ComponentModel;

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
            root.Items.Add(new RegTreeViewItem(RegTreeViewItemType.FOLDER, (IntPtr)0x80000000) { Title = "HKEY_CLASSES_ROOT" });
            root.Items.Add(new RegTreeViewItem(RegTreeViewItemType.FOLDER, (IntPtr)0x80000001) { Title = "HKEY_CURRENT_USER" });
            root.Items.Add(new RegTreeViewItem(RegTreeViewItemType.FOLDER, (IntPtr)0x80000002) { Title = "HKEY_LOCAL_MACHINE" });
            root.Items.Add(new RegTreeViewItem(RegTreeViewItemType.FOLDER, (IntPtr)0x80000003) { Title = "HKEY_USERS" });
            root.Items.Add(new RegTreeViewItem(RegTreeViewItemType.FOLDER, (IntPtr)0x80000005) { Title = "HKEY_CURRENT_CONFIG" });
            RegTree.Items.Add(root);

            RegValTable.ItemsSource = RegValueItem.Empty;
        }

        [DllImport("regzck.dll", EntryPoint = "regQuery", CallingConvention = CallingConvention.StdCall)]
        public static extern void regQuery(IntPtr hKey, out IntPtr subKeyNames, out int pcSubKeys, out IntPtr regValues, out int pcValues);

        [DllImport("regzck.dll", EntryPoint = "regOpen", CallingConvention = CallingConvention.StdCall)]
        public static extern void regOpen(IntPtr parentKey, KeyName name, out IntPtr output);

        private static TreeViewItem getParent(TreeViewItem item)
        {
            var parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as TreeViewItem;
        }

        private void RegTree_Expanded(object sender, RoutedEventArgs e)
        {
            var tvi = e.OriginalSource as TreeViewItem;
            if (tvi != null)
            {
                var context = tvi.DataContext as RegTreeViewItem;
                if(context != null)
                {
                    if(context.Type != RegTreeViewItemType.COMPUTER)
                    {
                        context.Type = RegTreeViewItemType.FOLDER_OPEN;
                        if (!context.hKeyOpened)
                        {
                            var name = new KeyName() { Name = context.Title };
                            IntPtr output;
                            regOpen((getParent(tvi).DataContext as RegTreeViewItem).hKey, name, out output);
                            context.hKey = output;
                            if (context.hKey != (IntPtr)0)
                            {
                                context.hKeyOpened = true;
                            }
                            else
                            {
                                MessageBox.Show(string.Format("Cannot open HKEY: {0}.", context.Title));
                                return;
                            }
                        }

                        if(!context.hKeyQueried)
                        {
                            IntPtr subKeyNamesUnmanaged;
                            int cSubKeys;
                            IntPtr regValuesUnmanaged;
                            int cValues;
                            regQuery(context.hKey, out subKeyNamesUnmanaged, out cSubKeys, out regValuesUnmanaged, out cValues);

                            var subKeyNames = new KeyName[cSubKeys];
                            var keyNameStride = Marshal.SizeOf(typeof(KeyName));
                            for (int i = 0; i < cSubKeys; i++)
                            {
                                var p = new IntPtr(subKeyNamesUnmanaged.ToInt64() + i * keyNameStride);
                                subKeyNames[i] = (KeyName)Marshal.PtrToStructure(p, typeof(KeyName));
                            }

                            for (int i = 0; i < cSubKeys; i++)
                            {
                                context.Items.Add(new RegTreeViewItem(RegTreeViewItemType.FOLDER) { Title = subKeyNames[i].Name });
                            }

                            var regValueStride = Marshal.SizeOf(typeof(RegValue));
                            for (int i = 0; i < cValues; i++)
                            {
                                var p = new IntPtr(regValuesUnmanaged.ToInt64() + i * regValueStride);
                                var regValue = (RegValue)Marshal.PtrToStructure(p, typeof(RegValue));
                                context.OriginalValues.Add(regValue);
                                context.Values.Add(new RegValueItem(regValue));
                            }

                            context.hKeyQueried = true;
                        }
                    }
                }
            }
        }

        private void RegTree_Collapsed(object sender, RoutedEventArgs e)
        {
            var tvi = e.OriginalSource as TreeViewItem;
            if (tvi != null)
            {
                var context = tvi.DataContext as RegTreeViewItem;
                if (context != null)
                {
                    if (context.Type != RegTreeViewItemType.COMPUTER)
                    {
                        context.Type = RegTreeViewItemType.FOLDER;
                    }
                }
            }
        }

        private void RegTree_Selected(object sender, RoutedEventArgs e)
        {
            var tvi = e.OriginalSource as TreeViewItem;
            if (tvi != null)
            {
                var context = tvi.DataContext as RegTreeViewItem;
                if (context != null)
                {
                    RegValTable.ItemsSource = context.Values;
                }
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct KeyName
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string Name;
    }

    public enum RegValueType : uint
    {
        REG_NONE = 0, REG_SZ = 1, REG_EXPAND_SZ = 2, REG_BINARY = 3, REG_DWORD = 4, REG_DWORD_BIG_ENDIAN = 5,
        REG_LINK = 6, REG_MULTI_SZ = 7, REG_RESOURCE_LIST = 8, REG_FULL_RESOURCE_DESCRIPTOR = 9,
        REG_RESOURCE_REQUIREMENTS_LIST = 10, REG_QWORD = 11,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct RegValue
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16383)]
        public string Name;
        public RegValueType Type;
        public IntPtr Data;
        public uint CbData;
    }

    public class RegValueItem
    {
        public static ObservableCollection<RegValueItem> Empty { get; set; }

        static RegValueItem()
        {
            Empty = new ObservableCollection<RegValueItem>();
        }

        public RegValueItem(RegValue original)
        {
            Name = original.Name;
            if(Name == "")
            {
                Name = "(Default)";
            }
            Type = original.Type.ToString("G");
            switch(original.Type)
            {
                case RegValueType.REG_SZ:
                    {
                        Data = Marshal.PtrToStringAuto(original.Data);
                        break;
                    }
                default:
                    {
                        Data = "(preview unavailable)";
                        break;
                    }
            }
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
    }

    public enum RegTreeViewItemType { COMPUTER, FOLDER, FOLDER_OPEN, }

    public class RegTreeViewItem : INotifyPropertyChanged
    {
        public RegTreeViewItem(RegTreeViewItemType Type, IntPtr hKey)
        {
            Items = new ObservableCollection<RegTreeViewItem>();
            this.Type = Type;
            this.hKey = hKey;
            hKeyOpened = hKeyPredefined.Contains(hKey) ? true : false;
            hKeyQueried = false;
            OriginalValues = new List<RegValue>();
            Values = new ObservableCollection<RegValueItem>();
        }
        public RegTreeViewItem(RegTreeViewItemType Type) : this(Type, (IntPtr)0) { }

        public ObservableCollection<RegTreeViewItem> Items { get; set; }

        public string Title { get; set; }

        private RegTreeViewItemType type;
        public RegTreeViewItemType Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }

        public static readonly IntPtr[] hKeyPredefined = { (IntPtr)0x80000000, (IntPtr)0x80000001, (IntPtr)0x80000002, (IntPtr)0x80000003, (IntPtr)0x80000005 };

        public IntPtr hKey { get; set; }
        public bool hKeyOpened { get; set; }
        public bool hKeyQueried { get; set; }
        public List<RegValue> OriginalValues { get; set; }
        public ObservableCollection<RegValueItem> Values { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
            {
                return;
            }
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
