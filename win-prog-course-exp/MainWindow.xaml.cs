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
using System.IO;
using filezckmanaged;

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

            FileTree.Items.Add(new FileTreeViewItem(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
        }

        [DllImport("regzck.dll", EntryPoint = "regNewKey", CallingConvention = CallingConvention.StdCall)]
        public static extern void regNewKey(IntPtr parentKey, KeyName name, out IntPtr newKey);
        [DllImport("regzck.dll", EntryPoint = "regDelKey", CallingConvention = CallingConvention.StdCall)]
        public static extern void regDelKey(IntPtr parentKey, KeyName name);
        [DllImport("regzck.dll", EntryPoint = "regSetKeyStringValue", CallingConvention = CallingConvention.StdCall)]
        public static extern void regSetKeyStringValue(IntPtr hKey, KeyName name, KeyName data);
        [DllImport("regzck.dll", EntryPoint = "regDelValue", CallingConvention = CallingConvention.StdCall)]
        public static extern void regDelValue(IntPtr hKey, KeyName name);

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
                        if(!context.openKey())
                        {
                            return;
                        }

                        context.queryKey();
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

        private void RegTreeItemCtxMenu_New_Key_Click(object sender, RoutedEventArgs e)
        {
            var newKeyDlg = new NewKeyDlg();
            if(newKeyDlg.ShowDialog() == true)
            {
                var context = RegTree.SelectedItem as RegTreeViewItem;
                IntPtr newKey;
                var name = newKeyDlg.InputName.Text;
                regNewKey(context.hKey, new KeyName { Name = name }, out newKey);
                var newItem = new RegTreeViewItem(RegTreeViewItemType.FOLDER, newKey) { Parent = context, Title = name, hKeyOpened = true };
                context.Items.Add(newItem);
            }
        }
        private void RegTreeItemCtxMenu_Delete_Click(object sender, RoutedEventArgs e)
        {
            var context = RegTree.SelectedItem as RegTreeViewItem;
            switch(MessageBox.Show(string.Format("Delete HKEY: {0}?", context.Title), "Confirmation", MessageBoxButton.YesNo))
            {
                case MessageBoxResult.Yes:
                    {
                        regDelKey(context.Parent.hKey, new KeyName() { Name = context.Title });
                        context.Parent.Items.Remove(context);
                        break;
                    }
            }
        }
        private void RegTreeItemCtxMenu_New_StringValue_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new NewStringValueDlg();
            if (dlg.ShowDialog() == true)
            {
                var context = RegTree.SelectedItem as RegTreeViewItem;
                var name = dlg.InputName.Text;
                var data = dlg.InputData.Text;
                regSetKeyStringValue(context.hKey, new KeyName() { Name = name }, new KeyName() { Name = data });
                context.hKeyQueried = false;
                context.queryKey();
            }
        }
        private void RegValTable_Delete_Click(object sender, RoutedEventArgs e)
        {
            var contextKey = RegTree.SelectedItem as RegTreeViewItem;
            var contextVal = RegValTable.SelectedItem as RegValueItem;
            switch (MessageBox.Show(string.Format("Delete value '{0}' from key '{1}?", contextVal.Name, contextKey.Title), "Confirmation", MessageBoxButton.YesNo))
            {
                case MessageBoxResult.Yes:
                    {
                        regDelValue(contextKey.hKey, new KeyName() { Name = contextVal.Name });
                        contextKey.Values.Remove(contextVal);
                        break;
                    }
            }
        }

        private void FileTree_Expanded(object sender, RoutedEventArgs e)
        {
            var tvi = e.OriginalSource as TreeViewItem;
            if (tvi != null)
            {
                var context = tvi.DataContext as FileTreeViewItem;
                if (context != null)
                {
                    if (context.Type == FileTreeViewItemType.FOLDER)
                    {
                        context.Type = FileTreeViewItemType.FOLDER_OPEN;
                        context.queryFile();
                    }
                }
            }
        }
        private void FileTree_Collapsed(object sender, RoutedEventArgs e)
        {
            var tvi = e.OriginalSource as TreeViewItem;
            if (tvi != null)
            {
                var context = tvi.DataContext as FileTreeViewItem;
                if (context != null)
                {
                    if (context.Type == FileTreeViewItemType.FOLDER_OPEN)
                    {
                        context.Type = FileTreeViewItemType.FOLDER;
                    }
                }
            }
        }
        [DllImport("Kernel32.dll")]
        public static extern int CreateDirectory(string lpPathName, IntPtr lpSecurityAttributes);
        [DllImport("Kernel32.dll")]
        public static extern IntPtr CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);
        [DllImport("Kernel32.dll")]
        public static extern int CloseHandle(IntPtr hObject);
        [DllImport("Kernel32.dll")]
        public static extern int DeleteFile(string lpFileName);
        [DllImport("Kernel32.dll")]
        public static extern int RemoveDirectory(string lpPathName);
        private void FileTreeItemCtxMenu_New_Folder_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new NewKeyDlg() { PromptText = "Please enter the folder name:" };
            if (dlg.ShowDialog() == true)
            {
                var context = FileTree.SelectedItem as  FileTreeViewItem;
                if(context.Type == FileTreeViewItemType.FILE)
                {
                    context = context.Parent;
                }
                var path = System.IO.Path.Combine(context.Path, dlg.InputName.Text);
                CreateDirectory(path, (IntPtr)0);
                var newItem = new FileTreeViewItem(path) { Parent = context };
                int index = 0;
                for(;index < context.Items.Count; index++)
                {
                    if(context.Items[index].Type == FileTreeViewItemType.FILE)
                    {
                        break;
                    }
                }
                context.Items.Insert(index, newItem);
            }
        }
        private void FileTreeItemCtxMenu_New_File_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new NewKeyDlg() { PromptText = "Please enter the file name:" };
            if (dlg.ShowDialog() == true)
            {
                var context = FileTree.SelectedItem as FileTreeViewItem;
                if (context.Type == FileTreeViewItemType.FILE)
                {
                    context = context.Parent;
                }
                var path = System.IO.Path.Combine(context.Path, dlg.InputName.Text);
                if(File.Exists(path))
                {
                    MessageBox.Show("File already exists.");
                    return;
                }
                var hFile = CreateFile(path, 0, 0, (IntPtr)0, 1, 0x80, (IntPtr)0);
                CloseHandle(hFile);
                var newItem = new FileTreeViewItem(path) { Parent = context };
                context.Items.Add(newItem);
            }
        }
        private void FileTreeItemCtxMenu_Delete_Click(object sender, RoutedEventArgs e)
        {
            var context = FileTree.SelectedItem as FileTreeViewItem;
            if(context.Type != FileTreeViewItemType.FILE && context.Items.Count > 0)
            {
                MessageBox.Show("Cannot delete a non-empty folder.");
                return;
            }
            switch (MessageBox.Show(string.Format("Delete {0}?", context.Title), "Confirmation", MessageBoxButton.YesNo))
            {
                case MessageBoxResult.Yes:
                    {
                        if(context.Type == FileTreeViewItemType.FILE)
                        {
                            DeleteFile(context.Path);
                        }
                        else
                        {
                            RemoveDirectory(context.Path);
                        }
                        context.Parent.Items.Remove(context);
                        break;
                    }
            }
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
        public RegTreeViewItem Parent { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
            {
                return;
            }
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [DllImport("regzck.dll", EntryPoint = "regOpen", CallingConvention = CallingConvention.StdCall)]
        public static extern void regOpen(IntPtr parentKey, KeyName name, out IntPtr output);
        [DllImport("regzck.dll", EntryPoint = "regQuery", CallingConvention = CallingConvention.StdCall)]
        public static extern void regQuery(IntPtr hKey, out IntPtr subKeyNames, out int pcSubKeys, out IntPtr regValues, out int pcValues);
        public bool openKey()
        {
            if(hKeyOpened)
            {
                return true;
            }

            var name = new KeyName() { Name = Title };
            IntPtr output;
            regOpen(Parent.hKey, name, out output);
            hKey = output;
            if (hKey != (IntPtr)0)
            {
                hKeyOpened = true;
            }
            else
            {
                MessageBox.Show(string.Format("Cannot open HKEY: {0}.", Title));
                return false;
            }

            return true;
        }

        public void queryKey()
        {
            if (!hKeyQueried)
            {
                IntPtr subKeyNamesUnmanaged;
                int cSubKeys;
                IntPtr regValuesUnmanaged;
                int cValues;
                regQuery(hKey, out subKeyNamesUnmanaged, out cSubKeys, out regValuesUnmanaged, out cValues);

                var subKeyNames = new KeyName[cSubKeys];
                var keyNameStride = Marshal.SizeOf(typeof(KeyName));
                for (int i = 0; i < cSubKeys; i++)
                {
                    var p = new IntPtr(subKeyNamesUnmanaged.ToInt64() + i * keyNameStride);
                    subKeyNames[i] = (KeyName)Marshal.PtrToStructure(p, typeof(KeyName));
                }

                Items.Clear();
                for (int i = 0; i < cSubKeys; i++)
                {
                    Items.Add(new RegTreeViewItem(RegTreeViewItemType.FOLDER) { Title = subKeyNames[i].Name, Parent = this });
                }

                OriginalValues.Clear();
                Values.Clear();
                var regValueStride = Marshal.SizeOf(typeof(RegValue));
                for (int i = 0; i < cValues; i++)
                {
                    var p = new IntPtr(regValuesUnmanaged.ToInt64() + i * regValueStride);
                    var regValue = (RegValue)Marshal.PtrToStructure(p, typeof(RegValue));
                    OriginalValues.Add(regValue);
                    Values.Add(new RegValueItem(regValue));
                }

                hKeyQueried = true;
            }
        }
    }
}
