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
    /// Логика взаимодействия для Groups.xaml
    /// </summary>
    public partial class Groups : Window
    {
        string connectionString;
        SqlDataAdapter adapter;
        DataTable deliveryTable;
        int ID_Gr = 0;
        public Groups()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ComboFilter.Items.Add("По названию");
            ComboFilter.Items.Add("По категории");
            ComboFilter.Items.Add("По размеру");
            string sql = "select id_group, type_prav, size_group from Groups";
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

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddGroup ag = new AddGroup();
            ag.Show();
            this.Close();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (DGrid.SelectedItems.Count == 0) MessageBox.Show("Не выбрана строка для редактирования", "Ошибка!");
            else
            {
                Data.Edit_id_group = ((DataRowView)DGrid.SelectedItems[0]).Row["id_group"].ToString();
                Data.Edit_type_prav = ((DataRowView)DGrid.SelectedItems[0]).Row["type_prav"].ToString();
                Data.Edit_size_group = ((DataRowView)DGrid.SelectedItems[0]).Row["size_group"].ToString();

                EditGroup er = new EditGroup();
                er.Show();
                this.Close();
            }
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
                SqlCommand command0 = sqlConnection.CreateCommand();
                command0.CommandText = "select ID_Gr from Groups where id_group='" + row["id_group"] + "'";
                SqlDataReader reader0 = command0.ExecuteReader();
                reader0.Read();
                if (reader0.HasRows)
                {
                    ID_Gr = Convert.ToInt32(reader0["ID_Gr"]);
                }
                reader0.Close();
                SqlCommand command05 = sqlConnection.CreateCommand();
                command05.CommandText = "select ID_Gr from Rasp where ID_Gr='" + ID_Gr + "'";
                SqlDataReader reader05 = command05.ExecuteReader();
                reader05.Read();
                if (reader05.HasRows)
                {
                    MessageBox.Show("Данная группа участвует в расписании","Ошибка!");
                }
                else
                {
                    var command = sqlConnection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "DelGroup";
                    command.Parameters.AddWithValue("@Del_id_group", row["id_group"]);
                    command.Parameters.AddWithValue("@Del_type_prav", row["type_prav"]);
                    command.Parameters.AddWithValue("@Del_size_group", row["size_group"]);
                    command.ExecuteNonQuery();
                    sqlConnection.Close();
                    Groups gr = new Groups();
                    gr.Show();
                    this.Close();
                }
            }
            else MessageBox.Show("Перед удалением выберите строку", "Ошибка!");
        }

        private void ToPDF_Click(object sender, RoutedEventArgs e)
        {
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("C:/Users/Public/Desktop/Учебные группы Список.pdf", FileMode.Create));
            document.Open();
            BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(baseFont, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL);
            PdfPTable table = new PdfPTable(DGrid.Columns.Count);
            PdfPCell cell = new PdfPCell(new Phrase("Учебные группы", font));
            cell.Colspan = DGrid.Columns.Count;
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            table.AddCell(cell);
            string[] name = { "Группа", "Категория", "Размер группы" };
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
            Process.Start("C:/Users/Public/Desktop/Учебные группы Список.pdf");
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
                deliveryTable.DefaultView.RowFilter = string.Format("[id_group] LIKE '%{0}%'", FilterBox.Text);
            }
            else if (ComboFilter.SelectedIndex == 1)
            {
                deliveryTable.DefaultView.RowFilter = string.Format("[type_prav] LIKE '%{0}%'", FilterBox.Text);
            }
            else if (ComboFilter.SelectedIndex == 2)
            {
                deliveryTable.DefaultView.RowFilter = string.Format("[size_group] lIKE '%{0}%'", FilterBox.Text);
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
    }
}
