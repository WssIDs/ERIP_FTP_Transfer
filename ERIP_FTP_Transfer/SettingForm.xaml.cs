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
using System.Windows.Threading;

using System.IO;

namespace ERIP_FTP_Transfer
{
    /// <summary>
    /// Interaction logic for SettingForm.xaml
    /// </summary>
    public partial class SettingForm : Window
    {

        DispatcherTimer dispatcherTimer;

        public SettingForm()
        {
            InitializeComponent();

            host_box.Text = Properties.Settings.Default.server;
            port_box.Text = Properties.Settings.Default.Port;
            username_box.Text = Properties.Settings.Default.username;
            password_box.Password = Properties.Settings.Default.password;

            mode_box.IsChecked = Properties.Settings.Default.UsePassivemode;
            workingpath_box.Text = Properties.Settings.Default.workingpath;
            archivepath_box.Text = Properties.Settings.Default.archivepath;
            delete_box.IsChecked = Properties.Settings.Default.bDeleteFiles;


            //  DispatcherTimer setup
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

        }

        //  System.Windows.Threading.DispatcherTimer.Tick handler
        //
        //  Updates the current seconds display and calls
        //  InvalidateRequerySuggested on the CommandManager to force 
        //  the Command to raise the CanExecuteChanged event.
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!Directory.Exists(workingpath_box.Text))
            {
                workingpath_box.Background = new SolidColorBrush(Colors.LightPink);
            }
            else
            {
                workingpath_box.Background = new SolidColorBrush(Colors.LightGreen);
            }

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }


        private void ok_button_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(workingpath_box.Text))
            {
                MessageBox.Show("Папки по указаному пути не существует \n" + workingpath_box.Text + "\n укажите существую папку", "Системное сообщение");
            }
            else
            {
                SaveConfig();
                Close();
            }

        }

        void SaveConfig()
        {
                if (!Directory.Exists(workingpath_box.Text + @"\" + Properties.Settings.Default.inPath))
                {
                    Directory.CreateDirectory(workingpath_box.Text + @"\" + Properties.Settings.Default.inPath);
                }

                if (!Directory.Exists(workingpath_box.Text + @"\" + Properties.Settings.Default.outPath))
                {
                    Directory.CreateDirectory(workingpath_box.Text + @"\" + Properties.Settings.Default.outPath);
                }


                if (!Directory.Exists(workingpath_box.Text + archivepath_box.Text + Properties.Settings.Default.inPath))
                {
                    Directory.CreateDirectory(workingpath_box.Text + archivepath_box.Text + Properties.Settings.Default.inPath);
                }

                if (!Directory.Exists(workingpath_box.Text + archivepath_box.Text + Properties.Settings.Default.outPath))
                {
                    Directory.CreateDirectory(workingpath_box.Text + archivepath_box.Text + Properties.Settings.Default.outPath);
                }

                if (!Directory.Exists(workingpath_box.Text + archivepath_box.Text + Properties.Settings.Default.outPath + @"\" + Properties.Settings.Default.eripresultpath))
                {
                    Directory.CreateDirectory(workingpath_box.Text + archivepath_box.Text + Properties.Settings.Default.outPath + @"\" + Properties.Settings.Default.eripresultpath);
                }


                Properties.Settings.Default.UsePassivemode = (bool)mode_box.IsChecked;
                Properties.Settings.Default.workingpath = workingpath_box.Text;
                Properties.Settings.Default.archivepath = archivepath_box.Text;
                Properties.Settings.Default.bDeleteFiles = (bool)delete_box.IsChecked;

                Properties.Settings.Default.server = host_box.Text;
                Properties.Settings.Default.Port = port_box.Text;
                Properties.Settings.Default.username = username_box.Text;
                Properties.Settings.Default.password = password_box.Password;

            Properties.Settings.Default.Save();

        }

        private void accept_button_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(workingpath_box.Text))
            {
                MessageBox.Show("Папки по указаному пути не существует \n" + workingpath_box.Text + "\n укажите существую папку", "Системное сообщение");
            }
            else
            {
                SaveConfig();
            }
        }

        private void cancel_button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
