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

namespace PostgreSQL_working
{
    public partial class LoginMenu : Form
    {
        public LoginMenu()
        {
            InitializeComponent();
        }

        public void CreateSqlParameter(NpgsqlCommand cmd, string par_name, NpgsqlTypes.NpgsqlDbType par_type, object Value)
        {
            NpgsqlParameter param = cmd.CreateParameter();
            param.ParameterName = par_name;
            param.NpgsqlDbType = par_type;
            param.Value = Value;
            cmd.Parameters.Add(param);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NpgsqlConnection con = new NpgsqlConnection($"Server=localhost;Port=5432;User ID=maksNN;Password=maksNN;Database=Dating_Agency;");
            con.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM users u JOIN users_privilege up ON u.user_id=up.user_id JOIN positions_directory pd ON u.position=pd.position_id " +
                                                  "WHERE login=@login and password=crypt(@password, password)", con);
            CreateSqlParameter(cmd, "@login", NpgsqlTypes.NpgsqlDbType.Text, textBox1.Text);
            CreateSqlParameter(cmd, "@password", NpgsqlTypes.NpgsqlDbType.Text, textBox2.Text);


            NpgsqlDataReader dr = cmd.ExecuteReader();
            textBox1.Text = "";
            textBox2.Text = "";
            if (dr.Read())
            {
                User.SetSettings(dr.GetString(1), dr.GetString(2), dr.GetString(3), dr.GetString(4), dr.GetString(21));
                User.SetUserPrivilege(dr.GetString(16).Replace(" ", "").Split(','), dr.GetString(17).Replace(" ", "").Split(','), dr.GetString(18).Replace(" ", "").Split(','), dr.GetString(19).Replace(" ", "").Split(','), 
                                      (bool)dr[9], (bool)dr[10], (bool)dr[11], (bool)dr[12], (bool)dr[13], (bool)dr[14], (bool)dr[15]);
                con.Close();

                this.Visible = false;

                EventLog.AddEventLogInfo("Успешный вход");

                MainMenu main_menu = new MainMenu();
                main_menu.ShowDialog();
                this.Visible = true;
            }
            else
            {
                con.Close();
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
