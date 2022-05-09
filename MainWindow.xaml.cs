using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
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

namespace UchProcAutoStation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString;
        string login;
        public MainWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["AutoSchool"].ConnectionString;
        }

        private void RollUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://лига-драйв.рф/index.php");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection ThisConnection = new SqlConnection(connectionString);
            ThisConnection.Open();
            SqlCommand thisCommand = ThisConnection.CreateCommand();
            if (LogBox.Text.Length > 0)
            {
                if (PassBox.Password.Length > 0)
                {
                    thisCommand.CommandText = "select LoginAuth, PasswordAuth from DataAuth where LoginAuth='" + LogBox.Text + "' and PasswordAuth='" + PassBox.Password + "'";
                    SqlDataReader thisReader = thisCommand.ExecuteReader();
                    thisReader.Read();
                    if (thisReader.HasRows)
                        login = thisReader["LoginAuth"].ToString();
                    else
                        login = null;
                    thisReader.Close();
                    if (login != null)
                    {
                        MessageBox.Show("Вы авторизовались", "Добро пожаловать!");
                        HubMenu hub = new HubMenu();
                        hub.Show();
                        this.Close();
                    }
                    else MessageBox.Show("Логин или пароль неверны." + Environment.NewLine + "Введите корректные данные", "Ошибка!");
                }
                else MessageBox.Show("Поле пароля пустое"+Environment.NewLine+"Введите пароль", "Ошибка!");
            }
            else MessageBox.Show("Поле логина пустое" + Environment.NewLine + "Введите логин", "Ошибка!");
        }
    }
}
