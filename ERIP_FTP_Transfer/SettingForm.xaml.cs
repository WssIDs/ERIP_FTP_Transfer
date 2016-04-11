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

using System.IO;

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

            host_box.Text = Properties.Settings.Default.server;
            port_box.Text = Properties.Settings.Default.Port;
        }

        private void settingbut_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void workingpath_button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ok_button_Click(object sender, RoutedEventArgs e)
        {

            if (!Directory.Exists(workingpath_box.Text))
            {
                MessageBox.Show("Папки по указаному пути не существует \n" + workingpath_box.Text+"\n укажите существую папку", "Системное сообщение");
            }

            else
            {
                if (!Directory.Exists(workingpath_box.Text + Properties.Settings.Default.inPath))
                {
                    Directory.CreateDirectory(workingpath_box.Text + Properties.Settings.Default.inPath);
                }

                if (!Directory.Exists(workingpath_box.Text + Properties.Settings.Default.outPath))
                {
                    Directory.CreateDirectory(workingpath_box.Text + Properties.Settings.Default.outPath);
                }


                if (!Directory.Exists(workingpath_box.Text + archivepath_box.Text + Properties.Settings.Default.inPath))
                {
                    Directory.CreateDirectory(workingpath_box.Text + archivepath_box.Text + Properties.Settings.Default.inPath);
                }

                if (!Directory.Exists(workingpath_box.Text + archivepath_box.Text + Properties.Settings.Default.outPath))
                {
                    Directory.CreateDirectory(workingpath_box.Text + archivepath_box.Text + Properties.Settings.Default.outPath);
                }
            }
        }
    }
}
