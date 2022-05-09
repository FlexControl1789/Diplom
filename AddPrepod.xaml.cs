using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
    /// Логика взаимодействия для AddPrepod.xaml
    /// </summary>
    public partial class AddPrepod : Window
    {
        string connectionString;
        public AddPrepod()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TypeUchCombo.Items.Add("Преподаватель");
            TypeUchCombo.Items.Add("Инструктор");
        }
        bool IsGood(char c)
        {
            if (c >= '0' && c <= '9')
                return true;
            return false;
        }
        bool IsGood1(char c)
        {
            if (c >= 'а' && c <= 'я')
                return true;
            if (c >= 'А' && c <= 'Я')
                return true;
            return false;
        }

        private void FIOBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood1);
        }

        private void FIOBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            var stringData = (string)e.DataObject.GetData(typeof(string));
            if (stringData == null || !stringData.All(IsGood1))
                e.CancelCommand();
        }

        private void PassSerNomBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }
        private void INNBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }

        private void INNBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            var stringData = (string)e.DataObject.GetData(typeof(string));
            if (stringData == null || !stringData.All(IsGood))
                e.CancelCommand();
        }

        private void NumbersBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (FIOBox.Text != "" && TypeUchCombo.Text != "" && PassSerNomBox.Text != "" && INNBox.Text != "" && NumbersBox.Text != "" && MailBox.Text != "")
            {
                if (MailBox.Text.Contains("@"))
                {
                    if (MailBox.Text.Contains("gmail.com") || MailBox.Text.Contains("mail.ru") || MailBox.Text.Contains("yandex.ru") || MailBox.Text.Contains("ya.ru"))
                    {
                        SqlConnection ThisConnection = null;
                        ThisConnection = new SqlConnection(connectionString);
                        ThisConnection.Open();
                        var command = ThisConnection.CreateCommand();
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "AddPrepod";
                        command.Parameters.AddWithValue("@Add_FIO", FIOBox.Text);
                        command.Parameters.AddWithValue("@Add_TypeUch", TypeUchCombo.Text);
                        command.Parameters.AddWithValue("@Add_PassSerNom", PassSerNomBox.Text);
                        command.Parameters.AddWithValue("@Add_INN", INNBox.Text);
                        command.Parameters.AddWithValue("@Add_Numbers", NumbersBox.Text);
                        command.Parameters.AddWithValue("@Add_Mail", MailBox.Text);
                        command.ExecuteNonQuery();

                        Prepodavatel er = new Prepodavatel();
                        er.Show();
                        this.Close();
                    }
                    else MessageBox.Show("Указан некорректный адрес почты", "Ошибка!");
                }
                else MessageBox.Show("Указан некорректный адрес почты", "Ошибка!");
            }
            else MessageBox.Show("Одно или несколько полей пусты"+Environment.NewLine+"Заполните поля перед добавлением","Ошибка!");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Prepodavatel prep = new Prepodavatel();
            prep.Show();
            this.Close();
        }
    }
}
