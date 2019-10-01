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
using System.Windows.Shapes;

namespace win_prog_course_exp
{
    /// <summary>
    /// Interaction logic for NewKeyDlg.xaml
    /// </summary>
    public partial class NewKeyDlg : Window
    {
        public NewKeyDlg()
        {
            InitializeComponent();
        }
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        public string PromptText
        {
            get
            {
                return Prompt.Text;
            }
            set
            {
                Prompt.Text = value;
            }
        }
    }
}
