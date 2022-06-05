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
    /// Логика взаимодействия для EditGroup.xaml
    /// </summary>
    public partial class EditGroup : Window
    {
        string connectionString;
        public EditGroup()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["AutoSchool"].ConnectionString;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CategoryCombo.Items.Add("A");
            CategoryCombo.Items.Add("B");
            CategoryCombo.Items.Add("C");
            NameGroupBox.Text = Data.Edit_id_group;
            CategoryCombo.SelectedItem = Data.Edit_type_prav;
            SizeGroupBox.Text = Data.Edit_size_group;
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
            if (c >= '0' && c <= '9')
                return true;
            return false;
        }

        private void SizeGroupBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }

        private void SizeGroupBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            var stringData = (string)e.DataObject.GetData(typeof(string));
            if (stringData == null || !stringData.All(IsGood))
                e.CancelCommand();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (NameGroupBox.Text != "" && CategoryCombo.Text != "" && SizeGroupBox.Text != "")
            {
                if (NameGroupBox.Text == Data.Edit_id_group && CategoryCombo.Text == Data.Edit_type_prav && SizeGroupBox.Text == Data.Edit_size_group)
                {
                    MessageBox.Show("Вы не сделали изменений","Ошибка!");
                }
                else
                {
                    SqlConnection ThisConnection = null;
                    ThisConnection = new SqlConnection(connectionString);
                    ThisConnection.Open();
                    var command = ThisConnection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "UpdGroup";
                    command.Parameters.AddWithValue("@Old_id_group", Data.Edit_id_group);
                    command.Parameters.AddWithValue("@Old_type_prav", Data.Edit_type_prav);
                    command.Parameters.AddWithValue("@Old_size_group", Data.Edit_size_group);

                    command.Parameters.AddWithValue("@New_id_group", NameGroupBox.Text);
                    command.Parameters.AddWithValue("@New_type_prav", CategoryCombo.Text);
                    command.Parameters.AddWithValue("@New_size_group", SizeGroupBox.Text);
                    command.ExecuteNonQuery();
                    ThisConnection.Close();
                    Groups gr = new Groups();
                    gr.Show();
                    this.Close();
                }       
            }
            else MessageBox.Show("Одно или несколько полей пусты","Внимание!");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Groups gr = new Groups();
            gr.Show();
            this.Close();
        }
    }
}
