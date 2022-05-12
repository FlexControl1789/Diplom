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
    /// Логика взаимодействия для AddGroup.xaml
    /// </summary>
    public partial class AddGroup : Window
    {
        string connectionString;
        public AddGroup()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["AutoSchool"].ConnectionString;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CategoryCombo.Items.Add("A");
            CategoryCombo.Items.Add("B");
            CategoryCombo.Items.Add("C");
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
            Groups gr = new Groups();
            gr.Show();
            this.Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (NameGroupBox.Text != "" && CategoryCombo.Text != "" && SizeGroupBox.Text != "")
            {
                SqlConnection ThisConnection = null;
                ThisConnection = new SqlConnection(connectionString);
                ThisConnection.Open();
                var command = ThisConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "AddGroup";
                command.Parameters.AddWithValue("@Add_id_group", NameGroupBox.Text);
                command.Parameters.AddWithValue("@Add_type_prav", CategoryCombo.Text);
                command.Parameters.AddWithValue("@Add_size_group", SizeGroupBox.Text);
                command.ExecuteNonQuery();

                Groups gr = new Groups();
                gr.Show();
                this.Close();
            }
            else MessageBox.Show("Одно или несколько полей пусты","Внимание!");
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
    }
}
