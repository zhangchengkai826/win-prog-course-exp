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
            var root = new TreeViewItem(TreeViewItemType.COMPUTER) { Title = "Computer" };
            root.Items.Add(new TreeViewItem(TreeViewItemType.FOLDER) { Title = "HKEY_CLASSES_ROOT" });
            root.Items.Add(new TreeViewItem(TreeViewItemType.FOLDER) { Title = "HKEY_CURRENT_USER" });
            root.Items.Add(new TreeViewItem(TreeViewItemType.FOLDER) { Title = "HKEY_LOCAL_MACHINE" });
            root.Items.Add(new TreeViewItem(TreeViewItemType.FOLDER) { Title = "HKEY_USERS" });
            root.Items.Add(new TreeViewItem(TreeViewItemType.FOLDER) { Title = "HKEY_CURRENT_CONFIG" });
            treeView.Items.Add(root);
        }
    }

    public enum TreeViewItemType { COMPUTER, FOLDER, FOLDER_OPEN, }

    public class TreeViewItem
    {
        public TreeViewItem(TreeViewItemType Type)
        {
            Items = new ObservableCollection<TreeViewItem>();
            this.Type = Type;

            switch (Type)
            {
                case TreeViewItemType.COMPUTER:
                    {
                        Icon = new BitmapImage(new Uri("Resources/icon/computer.png", UriKind.Relative));
                        break;
                    }
                case TreeViewItemType.FOLDER:
                    {
                        Icon = new BitmapImage(new Uri("Resources/icon/folder.png", UriKind.Relative));
                        break;
                    }
            }
        }

        public ObservableCollection<TreeViewItem> Items { get; set; }

        public string Title { get; set; }

        public ImageSource Icon { get; set; }

        public TreeViewItemType Type { get; set; }

    }
}
