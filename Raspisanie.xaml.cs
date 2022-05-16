using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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
    /// Логика взаимодействия для Raspisanie.xaml
    /// </summary>
    public partial class Raspisanie : Window
    {
        string connectionString;
        SqlDataAdapter adapter;
        DataTable deliveryTable;
        int ID_PREPOD, ID_PREDMET, ID_GROUP;
        public Raspisanie()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["AutoSchool"].ConnectionString;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ComboFilter.Items.Add("По времени");
            ComboFilter.Items.Add("По предмету");
            ComboFilter.Items.Add("По преподавателю");
            ComboFilter.Items.Add("По группе");
            string sql = "select Rasp.DayZan, Rasp.TimeZan, Predmets.Name_Predmet, PrepodsInstructors.FIO, Groups.id_group from Rasp, Predmets,PrepodsInstructors, Groups where Rasp.ID_Predm=Predmets.ID_Predm and Rasp.ID_Prepod=PrepodsInstructors.ID_Prepod and Rasp.ID_Gr=Groups.ID_Gr";
            deliveryTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(deliveryTable);
                DGrid.ItemsSource = deliveryTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
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

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddRasp addRasp = new AddRasp();
            addRasp.Show();
            this.Close();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (DGrid.SelectedItems.Count == 0) MessageBox.Show("Не выбрана строка для редактирования", "Ошибка!");
            else
            {
                Data.Edit_DayZan = ((DataRowView)DGrid.SelectedItems[0]).Row["DayZan"].ToString();
                Data.Edit_TimeZan = ((DataRowView)DGrid.SelectedItems[0]).Row["TimeZan"].ToString();
                Data.Edit_Name_Predmet_Rasp = ((DataRowView)DGrid.SelectedItems[0]).Row["Name_Predmet"].ToString();
                Data.Edit_FIO_Rasp = ((DataRowView)DGrid.SelectedItems[0]).Row["FIO"].ToString();
                Data.Edit_id_group_Rasp = ((DataRowView)DGrid.SelectedItems[0]).Row["id_group"].ToString();

                /*EditRasp er = new EditRasp();
                er.Show();
                this.Close();*/
            }
        }

        private void ToPDF_Click(object sender, RoutedEventArgs e)
        {
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("C:/Users/Public/Desktop/Расписание занятий.pdf", FileMode.Create));
            document.Open();
            BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(baseFont, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL);
            PdfPTable table = new PdfPTable(DGrid.Columns.Count);
            PdfPCell cell = new PdfPCell(new Phrase("Расписание занятий автошколы", font));
            cell.Colspan = DGrid.Columns.Count;
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            table.AddCell(cell);
            string[] name = { "День","Время", "Предмет", "Преподаватель", "Группа" };
            for (int j = 0; j < DGrid.Columns.Count; j++)
            {
                cell = new PdfPCell(new Phrase(name[j], font));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                table.AddCell(cell);
            }
            for (int j = 0; j < DGrid.Items.Count; j++)
            {
                for (int i = 0; i < DGrid.Columns.Count; i++)
                {
                    TextBlock b = DGrid.Columns[i].GetCellContent(DGrid.Items[j]) as TextBlock;
                    if (b != null)
                        table.AddCell(new Phrase(b.Text, font));
                }
            }
            document.Add(table);
            document.Close();
            Process.Start("C:/Users/Public/Desktop/Расписание занятий.pdf");
        }

        private void ToExcel_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Microsoft.Office.Interop.Excel.Worksheet sheet1 = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
            for (int j = 0; j < DGrid.Columns.Count; j++)
            {
                Microsoft.Office.Interop.Excel.Range myRange =
                    (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[1, j + 1];
                sheet1.Cells[1, j + 1].font.bold = true;
                myRange.Value2 = DGrid.Columns[j].Header;
            }
            for (int i = 0; i < DGrid.Columns.Count; i++)
            {
                for (int j = 0; j < DGrid.Items.Count; j++)
                {
                    TextBlock a = DGrid.Columns[i].GetCellContent(DGrid.Items[j]) as TextBlock;
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2, i + 1];
                    if (a != null)
                        myRange.Value2 = a.Text;
                }
                sheet1.Columns.AutoFit();
            }
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComboFilter.SelectedIndex == 0)
            {
                deliveryTable.DefaultView.RowFilter = string.Format("[DayZan] LIKE '%{0}%'", FilterBox.Text);
            }
            else if (ComboFilter.SelectedIndex == 1)
            {
                deliveryTable.DefaultView.RowFilter = string.Format("[TimeZan] LIKE '%{0}%'", FilterBox.Text);
            }
            else if (ComboFilter.SelectedIndex == 2)
            {
                deliveryTable.DefaultView.RowFilter = string.Format("[Name_Predmet] LIKE '%{0}%'", FilterBox.Text);
            }
            else if (ComboFilter.SelectedIndex == 3)
            {
                deliveryTable.DefaultView.RowFilter = string.Format("[FIO] LIKE '%{0}%'", FilterBox.Text);
            }
            else if (ComboFilter.SelectedIndex == 4)
            {
                deliveryTable.DefaultView.RowFilter = string.Format("[id_group] LIKE '%{0}%'", FilterBox.Text);
            }
        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterBox.Clear();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            HubMenu hub = new HubMenu();
            hub.Show();
            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DGrid.SelectedIndex != -1)
            {
                DataRowView rowView = (DataRowView)DGrid.SelectedItem;
                DataRow row = rowView.Row;
                SqlConnection sqlConnection = null;
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                //ПОИСК ID ПРЕПОДАВАТЕЛЯ
                SqlCommand thisCommand = sqlConnection.CreateCommand();
                thisCommand.CommandText = "select ID_Prepod from PrepodsInstructors where FIO='" + row["FIO"] + "'";
                SqlDataReader thisReader = thisCommand.ExecuteReader();
                thisReader.Read();
                if (thisReader.HasRows)
                {
                    ID_PREPOD = Convert.ToInt32(thisReader["ID_Prepod"]);
                }
                thisReader.Close();
                //ПОИСК ID ПРЕДМЕТА
                SqlCommand thisCommand1 = sqlConnection.CreateCommand();
                thisCommand1.CommandText = "select ID_Predm from Predmets where Name_Predmet='" + row["Name_Predmet"] + "'";
                SqlDataReader thisReader1 = thisCommand1.ExecuteReader();
                thisReader1.Read();
                if (thisReader1.HasRows)
                {
                    ID_PREDMET = Convert.ToInt32(thisReader1["ID_Predm"]);
                }
                thisReader1.Close();
                //ПОИСК ID ГРУППЫ
                SqlCommand thisCommand2 = sqlConnection.CreateCommand();
                thisCommand2.CommandText = "select ID_Gr from Groups where id_group='" + row["id_group"] + "'";
                SqlDataReader thisReader2 = thisCommand2.ExecuteReader();
                thisReader2.Read();
                if (thisReader2.HasRows)
                {
                    ID_GROUP = Convert.ToInt32(thisReader2["ID_Gr"]);
                }
                thisReader2.Close();
                
                var command = sqlConnection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DelRasp";
                command.Parameters.AddWithValue("@Del_DayZan", row["DayZan"]);
                command.Parameters.AddWithValue("@Del_TimeZan", row["TimeZan"]);
                command.Parameters.AddWithValue("@Del_ID_Predm", ID_PREDMET);
                command.Parameters.AddWithValue("@Del_ID_Prepod", ID_PREPOD);
                command.Parameters.AddWithValue("@Del_ID_Gr", ID_GROUP);
                command.ExecuteNonQuery();
                sqlConnection.Close();

                Raspisanie er = new Raspisanie();
                er.Show();
                this.Close();
            }
            else MessageBox.Show("Перед удалением выберите строку", "Ошибка!");
        }
    }
}
