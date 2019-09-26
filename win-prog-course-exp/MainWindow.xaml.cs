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

            ChapterBtn.Items = new ObservableCollection<ChapterBtn>();
            ChapterBtn.Items.Add(new ChapterBtn() { Title = "实验一" });
            ChapterBtn.Items.Add(new ChapterBtn() { Title = "实验二" });
            ChapterBtn.Items.Add(new ChapterBtn() { Title = "实验三" });
            chapterBtnsControl.ItemsSource = ChapterBtn.Items;
            ChapterBtn.CurOnId = 1;

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
                    if(context.Type != RegTreeViewItemType.COMPUTER)
                    {
                        context.Icon = new BitmapImage(new Uri("Resources/icon/folder-open.png", UriKind.Relative));
                        if (context.Items.Count == 0)
                        {
                            if (!hKeyPredefined.Contains(context.hKey))
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

                            for (int i = 0; i < cSubKeys; i++)
                            {
                                context.Items.Add(new RegTreeViewItem(RegTreeViewItemType.FOLDER) { Title = subKeyNames[i].achKey });
                            }
                        }
                    }
                }
            }
        }

        private void TreeView_Collapsed(object sender, RoutedEventArgs e)
        {
            var tvi = e.OriginalSource as TreeViewItem;
            if (tvi != null)
            {
                var context = tvi.DataContext as RegTreeViewItem;
                if (context != null)
                {
                    if (context.Type != RegTreeViewItemType.COMPUTER)
                    {
                        context.Icon = new BitmapImage(new Uri("Resources/icon/folder.png", UriKind.Relative));
                    }
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
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }

    public class ChapterBtn : INotifyPropertyChanged
    {
        public ChapterBtn()
        {
            IsOn = false;
            OnClickCmd = new RelayCommand(OnClick);
        }
        public string Title { get; set; }

        private bool isOn;
        public bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;
                OnPropertyChanged("IsOn");
            }
        }

        public static ObservableCollection<ChapterBtn> Items { get; set; }
        private static int curOnId;
        public static int CurOnId
        {
            get { return curOnId; }
            set
            {
                Items[CurOnId].IsOn = false;
                curOnId = value;
                Items[CurOnId].IsOn = true;
            }
        }
        private void OnClick(object obj)
        {
            var chapterBtn = (obj as Button).DataContext as ChapterBtn;
            CurOnId = Items.IndexOf(chapterBtn);
        }
        public ICommand OnClickCmd { get; set; }

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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct KeyName
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string achKey;
    }

    public enum RegTreeViewItemType { COMPUTER, FOLDER, }

    public class RegTreeViewItem : INotifyPropertyChanged
    {
        public RegTreeViewItem(RegTreeViewItemType Type)
        {
            Items = new ObservableCollection<RegTreeViewItem>();
            this.Type = Type;

            switch (Type)
            {
                case RegTreeViewItemType.COMPUTER:
                    {
                        Icon = new BitmapImage(new Uri("Resources/icon/laptop.png", UriKind.Relative));
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

        private ImageSource icon;
        public ImageSource Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                OnPropertyChanged("Icon");
            }
        }

        public RegTreeViewItemType Type { get; set; }

        public IntPtr hKey { get; set; }

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
