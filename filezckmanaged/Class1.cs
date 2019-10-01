using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filezckmanaged
{
    public enum FileTreeViewItemType { FILE, FOLDER, FOLDER_OPEN, }

    public class FileTreeViewItem : INotifyPropertyChanged
    {
        public FileTreeViewItem(string Path)
        {
            Items = new ObservableCollection<FileTreeViewItem>();
            this.Path = Path;
            var fileInfo = new FileInfo(Path);
            Title = fileInfo.Name;
            if (fileInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                Type = FileTreeViewItemType.FOLDER;
            }
            else
            {
                Type = FileTreeViewItemType.FILE;
            }
            FileQueried = false;
        }
        public ObservableCollection<FileTreeViewItem> Items { get; set; }
        public string Title { get; set; }
        private FileTreeViewItemType type;
        public FileTreeViewItemType Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
            {
                return;
            }
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Path { get; set; }
        public bool FileQueried { get; set; }
        public FileTreeViewItem Parent { get; set; }
        public void queryFile()
        {
            if (Type == FileTreeViewItemType.FILE || FileQueried)
            {
                return;
            }

            Items.Clear();
            var subDirs = Directory.EnumerateDirectories(Path);
            foreach (var d in subDirs)
            {
                Items.Add(new FileTreeViewItem(d) { Parent = this });
            }
            var files = Directory.EnumerateFiles(Path);
            foreach (var f in files)
            {
                Items.Add(new FileTreeViewItem(f) { Parent = this });
            }

            FileQueried = true;
        }
    }
}
