using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PostgreSQL_working
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
            CheckSecuritySettings();
        }

        private void CheckSecuritySettings()
        {
            if (!User.CheckPrivilege("rows_show", "users"))
                button3.Enabled = false;

            if (!User.CheckPrivilege("rows_add", "users"))
                button2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EventLog.AddEventLogInfo("Начинает работу с БД");
            DatabaseWorkingMenu database_working_menu = new DatabaseWorkingMenu();
            this.Visible = false;
            database_working_menu.ShowDialog();
            string login = User.CheckSettings("login");
            if (login == null)
            {
                EventLog.AddEventLogInfo("Выход из БД");
                Close();
            }
            this.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreateUserMenu create_user_menu = new CreateUserMenu();
            this.Visible = false;
            create_user_menu.ShowDialog();
            this.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            EventLog.AddEventLogInfo("Просматривает данные о пользователях");
            ShowUserMenu show_users_menu = new ShowUserMenu();
            this.Visible = false;
            show_users_menu.ShowDialog();
            this.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            EventLog.AddEventLogInfo("Выход из БД");
            Close();
        }
    }
}
