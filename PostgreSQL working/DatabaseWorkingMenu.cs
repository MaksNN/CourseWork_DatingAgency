using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace PostgreSQL_working
{
    public partial class DatabaseWorkingMenu : Form
    {
        private NpgsqlConnection con = new NpgsqlConnection($"Server=localhost;" +
                                                            $"Port=5433;" +
                                                            $"User ID=maksNN;" +
                                                            $"Password=maksNN;" +
                                                            $"Database=Dating_Agency;");
        private BindingSource bindsours = new BindingSource();
        private NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
        DataSet ds = new DataSet();

        public DatabaseWorkingMenu()
        {
            InitializeComponent();
            CheckShowTableSettings();
        }

        private void CheckShowTableSettings()
        {
            if (User.CheckPrivilege("rows_show", "clients_log"))
                button1.Enabled = true;
            if (User.CheckPrivilege("rows_show", "candidate_log"))
                button2.Enabled = true;
            if (User.CheckPrivilege("rows_show", "meeting_log"))
                button3.Enabled = true;
            if (User.CheckPrivilege("rows_show", "payment_log"))
                button4.Enabled = true;
            if (User.CheckPrivilege("rows_show", "archive"))
                button5.Enabled = true;
            if (User.CheckPrivilege("rows_show", "client"))
                button6.Enabled = true;
            if (User.CheckPrivilege("rows_show", "candidate"))
                button7.Enabled = true;
            if (User.CheckPrivilege("rows_show", "positions_directory"))
                button8.Enabled = true;
            if (User.CheckPrivilege("rows_show", "staff_directory"))
                button9.Enabled = true;
        }

        private void CheckSecurityTableSettings(string table_name)
        {
            if (User.CheckPrivilege("rows_add", table_name))
                button13.Enabled = true;
            if (User.CheckPrivilege("rows_change", table_name))
                button14.Enabled = true;
            if (User.CheckPrivilege("rows_delete", table_name))
                button15.Enabled = true;
        }

        private void ShowTable(string[] columns, string table_name)
        {
            con.Open();
            NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM {table_name}", con);
            adapter = new NpgsqlDataAdapter(cmd);
            ds = new DataSet();
            adapter.Fill(ds);

            bindsours.DataSource = ds.Tables[0];
            dataGridView1.DataSource = bindsours;

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                dataGridView1.Columns[i].HeaderText = columns[i];
            }

            EventLog.AddEventLogInfo($"Просматривает данные таблицы: {table_name}");

            con.Close();
        }

        private void MakeReadOnlyMode()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    dataGridView1.Rows[i].Cells[j].ReadOnly = true;
                }
            }
        }

        private void CreateRow()
        {
            bindsours.AddNew();
            dataGridView1.Refresh();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    if (Convert.ToString(dataGridView1.Rows[i].Cells[j].Value) == "")
                    {
                        dataGridView1.Rows[i].Cells[j].ReadOnly = false;
                    }
                }
            }

            EventLog.AddEventLogInfo($"Создает новую запись");
        }

        private void DeleteRow()
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                bindsours.RemoveAt(dataGridView1.SelectedRows[0].Index);
            }

            EventLog.AddEventLogInfo($"Удаляет запись");
        }

        private void EditRow()
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    dataGridView1.SelectedCells[i].ReadOnly = false;
                }
            }

            EventLog.AddEventLogInfo($"Редактирует запись");
        }

        private void FindRow()
        {
            List<string[]> filter_settings = GetFilterSettings();
            SetFilter(filter_settings);
            dataGridView1.Refresh();
        }

        private List<string[]> GetFilterSettings()
        {
            SearchMenu search_menu = new SearchMenu();
            string[] columns = new string[dataGridView1.Columns.Count];
            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = dataGridView1.Columns[i].HeaderText;
            }
            search_menu.AddColumns(columns);
            search_menu.ShowDialog(this);
            return search_menu.GetFilterSettings();
        }

        private void SetFilter(List<string[]> filter_settings)
        {
            Hashtable filter = new Hashtable();
            string[] columns = new string[filter_settings.Count];

            for (int i = 0; i < filter_settings.Count; i++)
            {
                if (filter_settings[i][1] == "...")
                    filter_settings[i][1] = null;

                filter.Add(filter_settings[i][0], filter_settings[i][1]);
                columns[i] = filter_settings[i][0];
            }

            dataGridView1.CurrentCell = null;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    if (filter[columns[i]] == null || row.Cells[i].Value.ToString().StartsWith(filter[columns[i]].ToString()))
                    {
                        row.Visible = true;
                    }
                    else
                    {
                        row.Visible = false;
                        break;
                    }
                }
            }
        }

        private void SaveDataBaseOnServer()
        {
            con.Open();
            adapter.UpdateCommand = new NpgsqlCommandBuilder(adapter).GetUpdateCommand();
            bindsours.EndEdit();
            adapter.Update(ds);
            con.Close();
        }

        private void CreateExcelReport()
        {
            Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook ExcelWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet ExcelWorkSheet;

            ExcelWorkBook = ExcelApp.Workbooks.Add(System.Reflection.Missing.Value);

            ExcelWorkSheet = (Worksheet)ExcelWorkBook.Worksheets.get_Item(1);

            Microsoft.Office.Interop.Excel.Range range_group = ExcelWorkSheet.get_Range("A1", "E1").Cells;
            range_group.Merge(Type.Missing);
            range_group.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
            range_group.EntireRow.Font.Bold = true;
            ExcelWorkSheet.Cells[1] = "ОТЧЕТ:";
            

            for (int i = 1; i < dataGridView1.Rows.Count+2; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    if (i == 1)
                    {
                        ExcelApp.Cells[i + 1, j + 1] = dataGridView1.Columns[j].HeaderText;
                        ExcelApp.Cells[i + 1, j + 1].EntireRow.Font.Bold = true;
                    }
                    else
                    {
                        ExcelApp.Cells[i + 1, j + 1] = dataGridView1.Rows[i-2].Cells[j].Value;
                    }
                }
            }
            ExcelWorkSheet.Columns.AutoFit();

            ExcelApp.Visible = true;
            ExcelApp.UserControl = true;
        }

        private void CreateWordReport()
        {
            string html = "<table cellpadding='5' cellspacing='0' style='border: 1px solid #ccc;font-size: 9pt;font-family:arial'>";

            html += "<tr>";
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                html += "<th style='background-color: #B8DBFD;border: 1px solid #ccc'>" + column.HeaderText + "</th>";
            }
            html += "</tr>";

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                html += "<tr>";
                foreach (DataGridViewCell cell in row.Cells)
                {
                    html += "<td style='width:120px;border: 1px solid #ccc'>" + cell.Value.ToString() + "</td>";
                }
                html += "</tr>";
            }

            html += "</table>";

            string htmlFilePath = String.Concat(Directory.GetParent(Convert.ToString(Directory.GetParent(Directory.GetCurrentDirectory()))), "\\DataGridView.htm");
            if (File.Exists(htmlFilePath))
            {
                File.Delete(htmlFilePath);
            }
            File.WriteAllText(htmlFilePath, html);

            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            Document wordDoc = word.Documents.Open(FileName: htmlFilePath, ReadOnly: false);
            string wordFilePath = String.Concat(Directory.GetParent(Convert.ToString(Directory.GetParent(Directory.GetCurrentDirectory()))), "\\DataGridView.doc");
            if (File.Exists(wordFilePath))
            {
                File.Delete(wordFilePath);
            }
            word.Visible = true;
        }

        private void ResetUser()
        {
            User.ResetSettings();
        }

        private void EnableTableButtons()
        {
            button16.Enabled = true;
            button17.Enabled = true;
            button18.Enabled = true;
            button20.Enabled = true;
            button21.Enabled = true;
        }

        private void DisableTableButtons()
        {
            button13.Enabled = false;
            button14.Enabled = false;
            button15.Enabled = false;
            button16.Enabled = false;
            button17.Enabled = false;
            button18.Enabled = false;
            button20.Enabled = false;
            button21.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowTable(new string[] { "ID-номер клиента", "ID-номер администратора", "Дата регистрации клиента" }, "clients_log");
            MakeReadOnlyMode();
            EnableTableButtons();
            CheckSecurityTableSettings("clients_log");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShowTable(new string[] { "ID-номер кандидата", "ID-номер администратора", "Дата регистрации кандидата" }, "candidate_log");
            MakeReadOnlyMode();
            EnableTableButtons();
            CheckSecurityTableSettings("candidate_log");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowTable(new string[] { "ID-номер приглашения", "ID-номер кандидата", "Дата встречи", "Время встречи" }, "meeting_log");
            MakeReadOnlyMode();
            EnableTableButtons();
            CheckSecurityTableSettings("meeting_log");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ShowTable(new string[] { "ID-номер квитанции об оплате", "ID-номер клиента", "Дата оплаты", "ID-номер кассира", "Оплата наличными", "Сумма оплаты" }, "payment_log");
            MakeReadOnlyMode();
            CheckSecurityTableSettings("payment_log");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ShowTable(new string[] { "ID-номер клиента", "Дата помещения в архив", "ID-номер администратора", "Причина помещения в архив" }, "archive");
            MakeReadOnlyMode();
            EnableTableButtons();
            CheckSecurityTableSettings("archive");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ShowTable(new string[] { "ID-номер клиента", "Фамилия", "Имя", "Отчество", "Возраст", "Пол", "Информация о клиенте", "Дополнительная информация", "Номер телефона" }, "client");
            MakeReadOnlyMode();
            EnableTableButtons();
            CheckSecurityTableSettings("client");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ShowTable(new string[] { "ID-номер кандидата", "Фамилия", "Имя", "Отчество", "Пол", "Возраст", "Информация о кандидате", "Требования кандидата к избраннику", "Номер телефона", "Фотография" }, "candidate");
            MakeReadOnlyMode();
            EnableTableButtons();
            CheckSecurityTableSettings("candidate");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ShowTable(new string[] { "ID-номер должности", "Должность" }, "positions_directory");
            MakeReadOnlyMode();
            EnableTableButtons();
            CheckSecurityTableSettings("positions_directory");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ShowTable(new string[] { "ID-номер сотрудника", "Фамилия", "Имя", "Отчество", "ID-номер должности" }, "staff_directory");
            MakeReadOnlyMode();
            EnableTableButtons();
            CheckSecurityTableSettings("staff_directory");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ShowTable(new string[] { "ID-номер события", "Логин", "Фамилия", "Имя", "Отчество", "Дата события", "Время события", "Операция" }, "event_log");
            MakeReadOnlyMode();
            EnableTableButtons();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            ResetUser();
            EventLog.AddEventLogInfo("Заканчивает работу с БД");
            Close();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            EventLog.AddEventLogInfo("Заканчивает работу с БД");
            Close();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            CreateRow();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            DeleteRow();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            EditRow();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            FindRow();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            SaveDataBaseOnServer();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            bindsours.DataSource = null;
            dataGridView1.Refresh();
            DisableTableButtons();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            CreateExcelReport();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            CreateWordReport();
        }
    }
}
