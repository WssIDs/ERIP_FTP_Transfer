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
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        public LoginForm()
        {
            InitializeComponent();


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            FTPClient client = new FTPClient();

           

            MainWindow main_wnd = new MainWindow();

            if (client.ConnectToFTP())
            {
                Close();

                Properties.Settings.Default.server = server_adress_box.Text;
                Properties.Settings.Default.Port = port_box.Text;
                Properties.Settings.Default.username = user_name_box.Text;
                Properties.Settings.Default.password = password_box.Password;
                Properties.Settings.Default.UsePassivemode = passive_check.IsChecked.Value;

                Properties.Settings.Default.Save();


                main_wnd.Show();
            }
            else
            {
                MessageBox.Show("Невозможно подключиться к FTP");
            }

        }

        private void exitbut_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void login_wnd_Loaded(object sender, RoutedEventArgs e)
        {
            server_adress_box.Text = Properties.Settings.Default.server;
            port_box.Text = Properties.Settings.Default.Port;
            user_name_box.Text = Properties.Settings.Default.username;
            password_box.Password = Properties.Settings.Default.password;
            passive_check.IsChecked = Properties.Settings.Default.UsePassivemode;
        }
    }
}
