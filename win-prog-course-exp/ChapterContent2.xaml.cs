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
        public ChapterContent2()
        {
            InitializeComponent();
        }

        private void Btn_Word1_Click(object sender, RoutedEventArgs e)
        {
            var guid = new Guid("9D9B759B-55E4-4FFC-B9DF-D7F2230B3439");
            var comType = Type.GetTypeFromCLSID(guid);
            var com = Activator.CreateInstance(comType) as ICOMZck;
            com.doTask1();
        }
    }
}
