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

namespace UchProcAutoStation
{
    /// <summary>
    /// Логика взаимодействия для HubMenu.xaml
    /// </summary>
    public partial class HubMenu : Window
    {
        public HubMenu()
        {
            InitializeComponent();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void RollUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void PrepodIsntruct_Click(object sender, RoutedEventArgs e)
        {
            Prepodavatel prepod = new Prepodavatel();
            prepod.Show();
            this.Close();
        }

        private void Groups_Click(object sender, RoutedEventArgs e)
        {
            Groups gr = new Groups();
            gr.Show();
            this.Close();
        }

        private void Predmets_Click(object sender, RoutedEventArgs e)
        {
            Predmets pred = new Predmets();
            pred.Show();
            this.Close();
        }

        private void Rasp_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
