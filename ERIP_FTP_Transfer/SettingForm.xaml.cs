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

namespace ERIP_FTP_Transfer
{
    /// <summary>
    /// Interaction logic for SettingForm.xaml
    /// </summary>
    public partial class SettingForm : Window
    {
        public SettingForm()
        {
            InitializeComponent();
        }

        private void settingbut_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
