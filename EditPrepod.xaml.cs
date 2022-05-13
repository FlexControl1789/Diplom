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
using UchProcAutoStation.Classes;

namespace UchProcAutoStation
{
    /// <summary>
    /// Логика взаимодействия для EditPrepod.xaml
    /// </summary>
    public partial class EditPrepod : Window
    {
        string connectionString;
        public EditPrepod()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["AutoSchool"].ConnectionString;
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

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (FIOBox.Text != null && TypeUchCombo.Text != null && PassSerNomBox.Text != null && INNBox.Text != null && NumbersBox.Text != null && MailBox.Text != null)
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
                        command.CommandText = "UpdPrepod";
                        command.Parameters.AddWithValue("@Old_FIO", Data.Edit_FIO);
                        command.Parameters.AddWithValue("@Old_TypeUch", Data.Edit_TypeUch);
                        command.Parameters.AddWithValue("@Old_PassSerNom", Data.Edit_PassSerNom);
                        command.Parameters.AddWithValue("@Old_INN", Data.Edit_INN);
                        command.Parameters.AddWithValue("@Old_Numbers", Data.Edit_Numbers);
                        command.Parameters.AddWithValue("@Old_Mail", Data.Edit_Mail);

                        command.Parameters.AddWithValue("@New_FIO", FIOBox.Text);
                        command.Parameters.AddWithValue("@New_TypeUch", TypeUchCombo.Text);
                        command.Parameters.AddWithValue("@New_PassSerNom", PassSerNomBox.Text);
                        command.Parameters.AddWithValue("@New_INN", INNBox.Text);
                        command.Parameters.AddWithValue("@New_Numbers", NumbersBox.Text);
                        command.Parameters.AddWithValue("@New_Mail", MailBox.Text);
                        command.ExecuteNonQuery();
                        ThisConnection.Close();
                        Prepodavatel er = new Prepodavatel();
                        er.Show();
                        this.Close();
                    }
                    else MessageBox.Show("Указан некорректный адрес почты", "Ошибка!");
                }
                else MessageBox.Show("Указан некорректный адрес почты", "Ошибка!");
            }
            else MessageBox.Show("Одно или несколько полей пусты" + Environment.NewLine + "Заполните поля перед редактированием", "Ошибка!");
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

        private void INNBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }

        private void NumbersBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }

        private void INNBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            var stringData = (string)e.DataObject.GetData(typeof(string));
            if (stringData == null || !stringData.All(IsGood))
                e.CancelCommand();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TypeUchCombo.Items.Add("Преподаватель");
            TypeUchCombo.Items.Add("Инструктор");

            FIOBox.Text = Data.Edit_FIO;
            TypeUchCombo.SelectedItem = Data.Edit_TypeUch;
            PassSerNomBox.Text = Data.Edit_PassSerNom;
            INNBox.Text = Data.Edit_INN;
            NumbersBox.Text = Data.Edit_Numbers;
            MailBox.Text = Data.Edit_Mail;
        }

        private void PassSerNomBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Prepodavatel prep = new Prepodavatel();
            prep.Show();
            this.Close();
        }
    }
}
