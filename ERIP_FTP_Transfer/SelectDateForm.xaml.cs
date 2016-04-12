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
    /// Interaction logic for SelectDateForm.xaml
    /// </summary>
    public partial class SelectDateForm : Window
    {
        public SelectDateForm()
        {
            InitializeComponent();
        }

        private void ok_button_Click(object sender, RoutedEventArgs e)
        {

            SelectedDatesCollection dates = calendar_box.SelectedDates;

            MainWindow.main.Status = "";

            foreach (DateTime date in dates)
            {
                MainWindow.main.Status += date.ToShortDateString() + "\n";
            }

            
        }
    }
}
