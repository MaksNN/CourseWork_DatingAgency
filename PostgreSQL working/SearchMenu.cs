using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PostgreSQL_working
{
    public partial class SearchMenu : Form
    {
        private List<string[]> columns = new List<string[]>();

        public SearchMenu()
        {
            InitializeComponent();
        }

        public void AddColumns(string[] new_columns)
        {
            for (int i = 0; i < new_columns.Length; i++)
            {
                string[] column = { new_columns[i], "..." };
                columns.Add(column);
            }
            comboBox1.Items.AddRange(new_columns);
            ShowFilterSettings();
        }

        private void ShowFilterSettings()
        {
            listBox1.Items.Clear();
            for (int i = 0; i < columns.Count; i++)
            {
                listBox1.Items.Add($"{columns[i][0]}: {columns[i][1]}");
            }
        }

        public List<string[]> GetFilterSettings()
        {
            return columns;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    if (columns[i][0] == (string)comboBox1.SelectedItem)
                    {
                        columns[i][1] = textBox1.Text;
                    }
                }
            }
            ShowFilterSettings();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                columns[i][1] = "...";
            }
            Close();
        }
    }
}
