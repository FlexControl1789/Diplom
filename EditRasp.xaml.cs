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
    /// Логика взаимодействия для EditRasp.xaml
    /// </summary>
    public partial class EditRasp : Window
    {
        string connectionString;
        int ID_PREDMET, ID_PREPOD, ID_GROUP;
        int ID_PREDMET_OLD, ID_PREPOD_OLD, ID_GROUP_OLD;
        public EditRasp()
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
            //ПОИСК СТАРОГО ID ПРЕДМЕТА
            SqlCommand sql1Command = ThisConnection.CreateCommand();
            sql1Command.CommandText = "select ID_Predm from Predmets where Name_Predmet='" + Data.Edit_Name_Predmet_Rasp + "'";
            SqlDataReader sql1Reader = sql1Command.ExecuteReader();
            sql1Reader.Read();
            if (sql1Reader.HasRows)
            {
                ID_PREDMET_OLD = Convert.ToInt32(sql1Reader["ID_Predm"]);
            }
            sql1Reader.Close();
            //ПОИСК СТАРОГО ID ПРЕПОДАВАТЕЛЯ
            SqlCommand sql2Command = ThisConnection.CreateCommand();
            sql2Command.CommandText = "select ID_Prepod from PrepodsInstructors where FIO='" + Data.Edit_FIO_Rasp + "'";
            SqlDataReader sql2Reader = sql2Command.ExecuteReader();
            sql2Reader.Read();
            if (sql2Reader.HasRows)
            {
                ID_PREPOD_OLD = Convert.ToInt32(sql2Reader["ID_Prepod"]);
            }
            sql2Reader.Close();
            //ПОИСК СТАРОГО ID ГРУППЫ
            SqlCommand sql3Command = ThisConnection.CreateCommand();
            sql3Command.CommandText = "select ID_Gr from Groups where id_group='" + Data.Edit_id_group_Rasp + "'";
            SqlDataReader sql3Reader = sql3Command.ExecuteReader();
            sql3Reader.Read();
            if (sql3Reader.HasRows)
            {
                ID_GROUP_OLD = Convert.ToInt32(sql3Reader["ID_Gr"]);
            }
            sql3Reader.Close();
            ThisConnection.Close();
            //
            DayCombo.SelectedItem = Data.Edit_DayZan;
            TimeCombo.SelectedItem = Data.Edit_TimeZan;
            PredmCombo.SelectedItem = Data.Edit_Name_Predmet_Rasp;
            PrepodCombo.SelectedItem = Data.Edit_FIO_Rasp;
            GroupCombo.SelectedItem = Data.Edit_id_group_Rasp;
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

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (DayCombo.Text == Data.Edit_DayZan && TimeCombo.Text == Data.Edit_TimeZan && PredmCombo.Text == Data.Edit_Name_Predmet_Rasp && PrepodCombo.Text == Data.Edit_FIO_Rasp && GroupCombo.Text == Data.Edit_id_group_Rasp)
            {
                MessageBox.Show("Вы не сделали изменений","Ошибка!");
            }
            else
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
                //ПРОВЕРКА НА ПОПЫТКУ ВВОДА ЗАНЯТИЯ С ТЕМИ ЖЕ ДАННЫМИ
                SqlCommand command4 = ThisConnection.CreateCommand();
                command4.CommandText = "select DayZan, TimeZan, ID_Prepod from Rasp where ID_Prepod='" + ID_PREPOD + "' and TimeZan='" + TimeCombo.Text + "' and DayZan='" + DayCombo.Text + "'";
                SqlDataReader reader4 = command4.ExecuteReader();
                if (reader4.HasRows)
                {
                    MessageBox.Show("Занятие на такой день и время с таким преподавателем уже существует", "Ошибка!");
                    reader4.Close();
                }
                reader4.Close();
                //ДОБАВЛЕНИЕ ЗАНЯТИЯ В РАСПИСАНИЕ
                var command = ThisConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "UpdRasp";
                command.Parameters.AddWithValue("@Old_DayZan", Data.Edit_DayZan);
                command.Parameters.AddWithValue("@Old_TimeZan", Data.Edit_TimeZan);
                command.Parameters.AddWithValue("@Old_ID_Predm", ID_PREDMET_OLD);
                command.Parameters.AddWithValue("@Old_ID_Prepod", ID_PREPOD_OLD);
                command.Parameters.AddWithValue("@Old_ID_Gr", ID_GROUP_OLD);

                command.Parameters.AddWithValue("@New_DayZan", DayCombo.Text);
                command.Parameters.AddWithValue("@New_TimeZan", TimeCombo.Text);
                command.Parameters.AddWithValue("@New_ID_Predm", ID_PREDMET);
                command.Parameters.AddWithValue("@New_ID_Prepod", ID_PREPOD);
                command.Parameters.AddWithValue("@New_ID_Gr", ID_GROUP);
                command.ExecuteNonQuery();
                ThisConnection.Close();
                Raspisanie pr = new Raspisanie();
                pr.Show();
                this.Close();
            }           
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Raspisanie rasp = new Raspisanie();
            rasp.Show();
            this.Close();
        }
    }
}
