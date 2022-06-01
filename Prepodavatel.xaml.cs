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
    /// Логика взаимодействия для Prepodavatel.xaml
    /// </summary>
    public partial class Prepodavatel : Window
    {
        string connectionString;
        SqlDataAdapter adapter;
        DataTable deliveryTable;
        public Prepodavatel()
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
            ComboFilter.Items.Add("По ФИО");
            ComboFilter.Items.Add("По специализации");
            ComboFilter.Items.Add("По серии и номеру паспорта");
            ComboFilter.Items.Add("По ИНН");
            ComboFilter.Items.Add("По мобильному телефону");
            ComboFilter.Items.Add("По Mail");
            string sql = "select FIO, TypeUch, PassSerNom, INN, Numbers, Mail from PrepodsInstructors";
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

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ComboFilter.SelectedIndex == 0)
            {
                deliveryTable.DefaultView.RowFilter = string.Format("[FIO] LIKE '%{0}%'", FilterBox.Text);
            }
            else if (ComboFilter.SelectedIndex == 1)
            {
                deliveryTable.DefaultView.RowFilter = string.Format("[TypeUch] LIKE '%{0}%'", FilterBox.Text);
            }
            else if (ComboFilter.SelectedIndex == 2)
            {
                deliveryTable.DefaultView.RowFilter = string.Format("[PassSerNom] LIKE '%{0}%'", FilterBox.Text);
            }
            else if (ComboFilter.SelectedIndex == 3)
            {
                deliveryTable.DefaultView.RowFilter = string.Format("[INN] LIKE '%{0}%'", FilterBox.Text);
            }
            else if (ComboFilter.SelectedIndex == 4)
            {
                deliveryTable.DefaultView.RowFilter = string.Format("[Numbers] LIKE '%{0}%'", FilterBox.Text);
            }
            else if (ComboFilter.SelectedIndex == 5)
            {
                deliveryTable.DefaultView.RowFilter = string.Format("[Mail] LIKE '%{0}%'", FilterBox.Text);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DGrid.SelectedIndex != -1)
            {
                int id = 0;
                SqlConnection Connection23 = null;
                Connection23 = new SqlConnection(connectionString);
                Connection23.Open();
                //Вытягиваем ID нового преподавателя для предмета
                SqlCommand thisCommand23 = Connection23.CreateCommand();
                thisCommand23.CommandText = "select ID_Prepod from PrepodsInstructors where FIO='" + ((DataRowView)DGrid.SelectedItems[0]).Row["FIO"].ToString() + "'";
                SqlDataReader Reader23 = thisCommand23.ExecuteReader();
                Reader23.Read();
                if (Reader23.HasRows)
                {
                    id = Convert.ToInt32(Reader23["ID_Prepod"]);
                }
                Reader23.Close();

                SqlCommand thisCommand24 = Connection23.CreateCommand();
                thisCommand24.CommandText = "select ID_Prepod from Predmets where ID_Prepod='" + id + "'";
                SqlDataReader Reader24 = thisCommand24.ExecuteReader();
                Reader24.Read();
                if (Reader24.HasRows)
                {
                    MessageBox.Show("Нельзя удалить преподавателя, так как с ним связан предмет", "Ошибка!");
                    Reader24.Close();
                }
                else
                {
                    Reader24.Close();
                    DataRowView rowView = (DataRowView)DGrid.SelectedItem;
                    DataRow row = rowView.Row;
                    SqlConnection sqlConnection = null;
                    sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    var command = sqlConnection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "DelPrepod";
                    command.Parameters.AddWithValue("@Del_FIO", row["FIO"]);
                    command.Parameters.AddWithValue("@Del_TypeUch", row["TypeUch"]);
                    command.Parameters.AddWithValue("@Del_PassSerNom", row["PassSerNom"]);
                    command.Parameters.AddWithValue("@Del_INN", row["INN"]);
                    command.Parameters.AddWithValue("@Del_Numbers", row["Numbers"]);
                    command.Parameters.AddWithValue("@Del_Mail", row["Mail"]);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();
                    Prepodavatel er = new Prepodavatel();
                    er.Show();
                    this.Close();
                }              
            }
            else MessageBox.Show("Перед удалением выберите строку", "Ошибка!");
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (DGrid.SelectedItems.Count == 0) MessageBox.Show("Не выбрана строка для редактирования","Ошибка!");
            else
            {
                Data.Edit_FIO = ((DataRowView)DGrid.SelectedItems[0]).Row["FIO"].ToString();
                Data.Edit_TypeUch = ((DataRowView)DGrid.SelectedItems[0]).Row["TypeUch"].ToString();
                Data.Edit_PassSerNom = ((DataRowView)DGrid.SelectedItems[0]).Row["PassSerNom"].ToString();
                Data.Edit_INN = ((DataRowView)DGrid.SelectedItems[0]).Row["INN"].ToString();
                Data.Edit_Numbers = ((DataRowView)DGrid.SelectedItems[0]).Row["Numbers"].ToString();
                Data.Edit_Mail = ((DataRowView)DGrid.SelectedItems[0]).Row["Mail"].ToString();

                EditPrepod EP = new EditPrepod();
                EP.Show();
                this.Close();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddPrepod AP = new AddPrepod();
            AP.Show();
            this.Close();
        }

        private void ToPDF_Click(object sender, RoutedEventArgs e)
        {
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("C:/Users/Public/Desktop/Преподаватели и инструкторы Список.pdf", FileMode.Create));
            document.Open();
            BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(baseFont, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL);
            PdfPTable table = new PdfPTable(DGrid.Columns.Count);
            PdfPCell cell = new PdfPCell(new Phrase("Преподаватели/Инструкторы", font));
            cell.Colspan = DGrid.Columns.Count;
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            table.AddCell(cell);
            string[] name = { "ФИО", "Специализация", "Серия и номер паспорта", "ИНН", "Мобильный телефон", "Mail" };
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
            Process.Start("C:/Users/Public/Desktop/Преподаватели и инструкторы Список.pdf");
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

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            HubMenu hub = new HubMenu();
            hub.Show();
            this.Close();
        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterBox.Clear();
        }
    }
}
