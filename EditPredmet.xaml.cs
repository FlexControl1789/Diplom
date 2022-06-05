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
    /// Логика взаимодействия для EditPredmet.xaml
    /// </summary>
    public partial class EditPredmet : Window
    {
        string connectionString;
        int ID_New, ID_Old;
        public EditPredmet()
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
            SqlCommand thisCommand = ThisConnection.CreateCommand();
            thisCommand.CommandText = "select FIO from PrepodsInstructors";
            SqlDataReader sqlReader = thisCommand.ExecuteReader();
            while (sqlReader.Read())
            {
                FIOCombo.Items.Add(sqlReader["FIO"].ToString());
            }
            sqlReader.Close();
            ThisConnection.Close();

            NamePredmetBox.Text = Data.Edit_Name_Predmet;
            TypeZanCombo.SelectedItem = Data.Edit_Type_Zan;
            FIOCombo.SelectedItem = Data.Edit_FIO_Pr;
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

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (NamePredmetBox.Text != "" && TypeZanCombo.Text != "" && FIOCombo.Text != "")
            {
                if (NamePredmetBox.Text == Data.Edit_Name_Predmet && TypeZanCombo.Text == Data.Edit_Type_Zan && FIOCombo.Text == Data.Edit_FIO_Pr)
                {
                    MessageBox.Show("Вы не сделали изменений","Ошибка!");
                }
                else
                {
                    SqlConnection ThisConnection = null;
                    ThisConnection = new SqlConnection(connectionString);
                    ThisConnection.Open();
                    //Вытягиваем ID нового преподавателя для предмета
                    SqlCommand thisCommand = ThisConnection.CreateCommand();
                    thisCommand.CommandText = "select ID_Prepod from PrepodsInstructors where FIO='" + FIOCombo.Text + "'";
                    SqlDataReader thisReader = thisCommand.ExecuteReader();
                    thisReader.Read();
                    if (thisReader.HasRows)
                    {
                        ID_New = Convert.ToInt32(thisReader["ID_Prepod"]);
                    }
                    thisReader.Close();
                    //Проверяем на попытку изменения при "неизменности" полей
                    SqlCommand command2 = ThisConnection.CreateCommand();
                    command2.CommandText = "select Name_Predmet, Type_Zan, ID_Prepod from Predmets";
                    SqlDataReader reader = command2.ExecuteReader();
                    reader.Read();
                    if (reader.HasRows)
                    {
                        if (NamePredmetBox.Text == reader["Name_Predmet"].ToString() && TypeZanCombo.Text == reader["Type_Zan"].ToString() && ID_New == Convert.ToInt32(reader["ID_Prepod"]))
                        {
                            MessageBox.Show("Вы ничего не изменили" + Environment.NewLine + "Измените какое-либо из полей", "Ошибка!");
                            reader.Close();
                        }
                        else
                        {
                            reader.Close();
                            //Вытягиваем ID старого преподавателя для его изменения
                            SqlCommand command3 = ThisConnection.CreateCommand();
                            command3.CommandText = "select ID_Prepod from PrepodsInstructors where FIO='" + Data.Edit_FIO_Pr + "'";
                            SqlDataReader reader1 = command3.ExecuteReader();
                            reader1.Read();
                            if (reader1.HasRows)
                            {
                                ID_Old = Convert.ToInt32(reader1["ID_Prepod"]);
                                reader1.Close();
                                var command = ThisConnection.CreateCommand();
                                command.CommandType = CommandType.StoredProcedure;
                                command.CommandText = "UpdPredmet";
                                command.Parameters.AddWithValue("@Old_Name_Predmet", Data.Edit_Name_Predmet);
                                command.Parameters.AddWithValue("@Old_Type_Zan", Data.Edit_Type_Zan);
                                command.Parameters.AddWithValue("@Old_ID_Prepod", ID_Old);

                                command.Parameters.AddWithValue("@New_Name_Predmet", NamePredmetBox.Text);
                                command.Parameters.AddWithValue("@New_Type_Zan", TypeZanCombo.Text);
                                command.Parameters.AddWithValue("@New_ID_Prepod", ID_New);
                                command.ExecuteNonQuery();
                                ThisConnection.Close();
                                Predmets pr = new Predmets();
                                pr.Show();
                                this.Close();
                            }
                        }
                    }
                }                      
            }
            else MessageBox.Show("Одно или несколько полей пусты", "Внимание!");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Predmets pr = new Predmets();
            pr.Show();
            this.Close();
        }
    }
}
