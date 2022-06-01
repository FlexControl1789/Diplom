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
    /// Логика взаимодействия для AddPredmet.xaml
    /// </summary>
    public partial class AddPredmet : Window
    {
        string connectionString;
        int ID;
        public AddPredmet()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["AutoSchool"].ConnectionString;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TypeZanCombo.Items.Add("Теория");
            TypeZanCombo.Items.Add("Практика");

            SqlConnection ThisConnection = null;
            ThisConnection = new SqlConnection(connectionString);
            ThisConnection.Open();
            SqlCommand thisCommand4 = ThisConnection.CreateCommand();
            thisCommand4.CommandText = "select FIO from PrepodsInstructors";
            SqlDataReader sqlReader = thisCommand4.ExecuteReader();
            while (sqlReader.Read())
            {
                FIOCombo.Items.Add(sqlReader["FIO"].ToString());
            }
            sqlReader.Close();
            ThisConnection.Close();
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
        bool IsGood(char c)
        {
            if (c >= 'а' && c <= 'я')
                return true;
            if (c >= 'А' && c <= 'Я')
                return true;
            return false;
        }

        private void NamePredmetBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }

        private void NamePredmetBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            var stringData = (string)e.DataObject.GetData(typeof(string));
            if (stringData == null || !stringData.All(IsGood))
                e.CancelCommand();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Predmets pr = new Predmets();
            pr.Show();
            this.Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (NamePredmetBox.Text != "" && TypeZanCombo.Text != "" && FIOCombo.Text != "")
            {
                SqlConnection ThisConnection = null;
                ThisConnection = new SqlConnection(connectionString);
                ThisConnection.Open();
                //ПОИСК ID ПРЕПОДАВАТЕЛЯ
                SqlCommand thisCommand = ThisConnection.CreateCommand();
                thisCommand.CommandText = "select ID_Prepod from PrepodsInstructors where FIO='" + FIOCombo.Text + "'";
                SqlDataReader thisReader = thisCommand.ExecuteReader();
                thisReader.Read();
                if (thisReader.HasRows)
                {
                    ID = Convert.ToInt32(thisReader["ID_Prepod"]);
                }
                thisReader.Close();
                //ПРОВЕРКА НА ДОБАВЛЕНИЕ ПОВТОРНОГО ПРЕДМЕТА
                SqlCommand command2 = ThisConnection.CreateCommand();
                command2.CommandText = "select Name_Predmet from Predmets";
                SqlDataReader reader = command2.ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    if (NamePredmetBox.Text == reader["Name_Predmet"].ToString())
                    {
                        MessageBox.Show("Вы попытались внести в базу предмет, который уже был добавлен" + Environment.NewLine + "Попробуйте другой предмет", "Ошибка!");
                        reader.Close();
                        NamePredmetBox.Clear();
                    }
                    else
                    {
                        reader.Close();
                        var command = ThisConnection.CreateCommand();
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "AddPredmet";
                        command.Parameters.AddWithValue("@Add_Name_Predmet", NamePredmetBox.Text);
                        command.Parameters.AddWithValue("@Add_Type_Zan", TypeZanCombo.Text);
                        command.Parameters.AddWithValue("@Add_ID_Prepod", ID);
                        command.ExecuteNonQuery();
                        ThisConnection.Close();
                        Predmets pr = new Predmets();
                        pr.Show();
                        this.Close();
                    }
                }                  
            }
            else MessageBox.Show("Одно или несколько полей пусты", "Внимание!");
        }
    }
}
