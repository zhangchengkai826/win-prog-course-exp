﻿using System;
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
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace win_prog_course_exp
{
    /// <summary>
    /// Interaction logic for ChapterSideSelector.xaml
    /// </summary>
    public partial class ChapterSideSelector : UserControl
    {
        public ChapterSideSelector()
        {
            InitializeComponent();

            ChapterSideSelectorItem.Items.Add(new ChapterSideSelectorItem() { Title = "实验一" });
            ChapterSideSelectorItem.Items.Add(new ChapterSideSelectorItem() { Title = "实验二" });
            ChapterSideSelectorItem.Items.Add(new ChapterSideSelectorItem() { Title = "实验三" });
            ChapterSideSelectorItem.Items.Add(new ChapterSideSelectorItem() { Title = "实验四" });
            ChapterSideSelectorItem.Items.Add(new ChapterSideSelectorItem() { Title = "实验五" });
            ChapterSideSelectorItem.Items.Add(new ChapterSideSelectorItem() { Title = "实验六" });
            chapterSideSelector.ItemsSource = ChapterSideSelectorItem.Items;
            ChapterSideSelectorController.Instance.CurOnId = 0;
        }
        public sealed class ChapterSideSelectorController : INotifyPropertyChanged
        {
            private ChapterSideSelectorController() { }
            private static readonly Lazy<ChapterSideSelectorController> Lazy = new Lazy<ChapterSideSelectorController>(() => new ChapterSideSelectorController());
            public static ChapterSideSelectorController Instance { get { return Lazy.Value; } }
            private int curOnId;
            public int CurOnId
            {
                get { return curOnId; }
                set
                {
                    ChapterSideSelectorItem.Items[curOnId].IsOn = false;
                    curOnId = value;
                    ChapterSideSelectorItem.Items[curOnId].IsOn = true;
                    OnPropertyChanged("CurOnId");
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
        }
        public class ChapterSideSelectorItem : INotifyPropertyChanged
        {
            static ChapterSideSelectorItem()
            {
                Items = new ObservableCollection<ChapterSideSelectorItem>();
            }
            public ChapterSideSelectorItem()
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

            public static ObservableCollection<ChapterSideSelectorItem> Items { get; set; }
           
            private void OnClick(object obj)
            {
                var chapterBtn = (obj as Button).DataContext as ChapterSideSelectorItem;
                ChapterSideSelectorController.Instance.CurOnId = Items.IndexOf(chapterBtn);
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
    }
}
