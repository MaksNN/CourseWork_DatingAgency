using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Office.Word;
using Npgsql;
using DatabaseWork_NET_Framework_;
using System.Collections;

namespace PostgreSQL_working
{
    public partial class LoginMenu : Form
    {
        public LoginMenu()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text;
            string password = textBox2.Text;
            textBox1.Text = "";
            textBox2.Text = "";

            if (DatabaseOperations.Authenticate(login, password))
            {
                ArrayList user_data = DatabaseOperations.GetUserData(login, password);
                User.SetSettings(user_data[0].ToString(), 
                                 user_data[1].ToString(), 
                                 user_data[2].ToString(), 
                                 user_data[3].ToString(), 
                                 user_data[4].ToString());
                User.SetUserPrivilege((string[])user_data[5],
                                      (string[])user_data[6],
                                      (string[])user_data[7],
                                      (string[])user_data[8],
                                      (bool)(user_data[9]),
                                      (bool)(user_data[10]),
                                      (bool)(user_data[11]),
                                      (bool)(user_data[12]),
                                      (bool)(user_data[13]),
                                      (bool)(user_data[14]),
                                      (bool)(user_data[15]));

                EventLog.AddEventLogInfo("Успешный вход");

                this.Visible = false;
                MainMenu main_menu = new MainMenu();
                main_menu.ShowDialog();
                this.Visible = true;
            }
            else
            {
                ErrorMenu error_menu = new ErrorMenu();
                error_menu.ShowDialog();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
