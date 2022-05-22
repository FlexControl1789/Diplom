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
    /// Логика взаимодействия для AddRasp.xaml
    /// </summary>
    public partial class AddRasp : Window
    {
        string connectionString;
        int ID_PREDMET, ID_PREPOD, ID_GROUP; 
        public AddRasp()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["AutoSchool"].ConnectionString;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DayCombo.Items.Add("Понедельник");
            DayCombo.Items.Add("Вторник");
            DayCombo.Items.Add("Среда");
            DayCombo.Items.Add("Четверг");
            DayCombo.Items.Add("Пятница");
            //
            TimeCombo.Items.Add("08:00:00");
            TimeCombo.Items.Add("09:30:00");
            TimeCombo.Items.Add("11:00:00");
            TimeCombo.Items.Add("12:30:00");
            TimeCombo.Items.Add("13:30:00");
            TimeCombo.Items.Add("15:00:00");
            TimeCombo.Items.Add("16:30:00");
            //ЗАГРУЗКА ПРЕДМЕТОВ
            SqlConnection ThisConnection = null;
            ThisConnection = new SqlConnection(connectionString);
            ThisConnection.Open();
            SqlCommand thisCommand = ThisConnection.CreateCommand();
            thisCommand.CommandText = "select Name_Predmet from Predmets";
            SqlDataReader sqlReader = thisCommand.ExecuteReader();
            while (sqlReader.Read())
            {
                PredmCombo.Items.Add(sqlReader["Name_Predmet"].ToString());
            }
            sqlReader.Close();
            //ЗАГРУЗКА ГРУПП
            SqlCommand sqlCommand = ThisConnection.CreateCommand();
            sqlCommand.CommandText = "select id_group from Groups";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                GroupCombo.Items.Add(sqlDataReader["id_group"].ToString());
            }
            sqlDataReader.Close();
            ThisConnection.Close();
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

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Raspisanie rasp = new Raspisanie();
            rasp.Show();
            this.Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (DayCombo.Text != "" && TimeCombo.Text != "" && PredmCombo.Text != "" && PrepodCombo.Text != "" && GroupCombo.Text != "")
            {
                //ПОИСК ID ПРЕПОДАВАТЕЛЯ
                SqlConnection ThisConnection = null;
                ThisConnection = new SqlConnection(connectionString);
                ThisConnection.Open();
                SqlCommand thisCommand = ThisConnection.CreateCommand();
                thisCommand.CommandText = "select ID_Prepod from PrepodsInstructors where FIO='" + PrepodCombo.Text + "'";
                SqlDataReader thisReader = thisCommand.ExecuteReader();
                thisReader.Read();
                if (thisReader.HasRows)
                {
                    ID_PREPOD = Convert.ToInt32(thisReader["ID_Prepod"]);
                }
                thisReader.Close();
                //ПОИСК ID ПРЕДМЕТА
                SqlCommand command2 = ThisConnection.CreateCommand();
                command2.CommandText = "select ID_Predm from Predmets where Name_Predmet='" + PredmCombo.Text + "'";
                SqlDataReader thisReader2 = command2.ExecuteReader();
                thisReader2.Read();
                if (thisReader2.HasRows)
                {
                    ID_PREDMET = Convert.ToInt32(thisReader2["ID_Predm"]);
                }
                thisReader2.Close();
                //ПОИСК ID ГРУППЫ
                SqlCommand command3 = ThisConnection.CreateCommand();
                command3.CommandText = "select ID_Gr from Groups where id_group='" + GroupCombo.Text.ToString() + "'";
                SqlDataReader thisReader3 = command3.ExecuteReader();
                thisReader3.Read();
                if (thisReader3.HasRows)
                {
                    ID_GROUP = Convert.ToInt32(thisReader3["ID_Gr"]);
                }
                thisReader3.Close();
                //ПРОВЕРКА НА ПОПЫТКУ ВВОДА ЗАНЯТИЯ С КОНФЛИКТОМ ВРЕМЕНИ
                SqlCommand command4 = ThisConnection.CreateCommand();
                command4.CommandText = "select DayZan, TimeZan, ID_Prepod from Rasp where ID_Prepod='" + ID_PREPOD + "' and TimeZan='" + TimeCombo.Text + "' and DayZan='" + DayCombo.Text + "'";
                SqlDataReader reader4 = command4.ExecuteReader();
                if (reader4.HasRows)
                {
                    MessageBox.Show("Занятие на такой день и время с таким преподавателем уже существует","Ошибка!");
                    reader4.Close();
                }
                else
                {
                    reader4.Close();
                    //ДОБАВЛЕНИЕ ЗАНЯТИЯ В РАСПИСАНИЕ
                    var command = ThisConnection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "AddRasp";
                    command.Parameters.AddWithValue("@Add_DayZan", DayCombo.Text);
                    command.Parameters.AddWithValue("@Add_TimeZan", TimeCombo.Text);
                    command.Parameters.AddWithValue("@Add_ID_Predm_Rasp", ID_PREDMET);
                    command.Parameters.AddWithValue("@Add_ID_Prepod_Rasp", ID_PREPOD);
                    command.Parameters.AddWithValue("@Add_ID_Gr_Rasp", ID_GROUP);
                    command.ExecuteNonQuery();
                    ThisConnection.Close();
                    Raspisanie pr = new Raspisanie();
                    pr.Show();
                    this.Close();
                }                                             
            }
            else MessageBox.Show("Одно или несколько полей пусты", "Внимание!");
        }

        private void PredmCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PredmCombo.SelectedItem.ToString() != "")
            {
                PrepodCombo.IsEnabled = true;
                PrepodCombo.Items.Clear();
                SqlConnection ThisConnection1 = null;
                ThisConnection1 = new SqlConnection(connectionString);
                ThisConnection1.Open();
                SqlCommand thisCommand1 = ThisConnection1.CreateCommand();
                thisCommand1.CommandText = "select PrepodsInstructors.FIO from PrepodsInstructors,Predmets where Predmets.ID_Prepod=PrepodsInstructors.ID_Prepod and Predmets.Name_Predmet='" + PredmCombo.SelectedItem.ToString() + "'";
                SqlDataReader sqlReader1 = thisCommand1.ExecuteReader();
                while (sqlReader1.Read())
                {
                    PrepodCombo.Items.Add(sqlReader1["FIO"].ToString());
                }
                sqlReader1.Close();
            }
            else
            { 
                PrepodCombo.IsEnabled = false;
                PrepodCombo.Items.Clear();
            }
        }
    }
}
