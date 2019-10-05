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
using comzck;

namespace win_prog_course_exp
{
    /// <summary>
    /// Interaction logic for ChapterContent2.xaml
    /// </summary>
    public partial class ChapterContent2 : UserControl
    {
        private ICOMZck comWord;
        private ICOMZck comExcel;
        public ChapterContent2()
        {
            InitializeComponent();
            var guid = new Guid("9D9B759B-55E4-4FFC-B9DF-D7F2230B3439");
            var comType = Type.GetTypeFromCLSID(guid);
            comWord = Activator.CreateInstance(comType) as ICOMZck;
            guid = new Guid("2FE12FD0-6AF0-4575-9EBB-D6A692FEFF9D");
            comType = Type.GetTypeFromCLSID(guid);
            comExcel = Activator.CreateInstance(comType) as ICOMZck;
        }
        private void Btn_Word1_Click(object sender, RoutedEventArgs e)
        {
            comWord.doTask1(null);
        }
        private void Btn_Word2_Click(object sender, RoutedEventArgs e)
        {
            comWord.doTask2();
        }
        private void Btn_Word3_Click(object sender, RoutedEventArgs e)
        {
            comWord.doTask3();
        }
        private void Btn_Excel1_Click(object sender, RoutedEventArgs e)
        {
            comExcel.doTask1(DataArea);
        }
        private void Btn_Excel2_Click(object sender, RoutedEventArgs e)
        {
            comExcel.doTask2();
        }
        private void Btn_Excel3_Click(object sender, RoutedEventArgs e)
        {
            comExcel.doTask3();
        }
    }
}
