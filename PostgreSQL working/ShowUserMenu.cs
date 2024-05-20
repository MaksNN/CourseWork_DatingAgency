using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using PasswordValidity;

namespace PostgreSQL_working
{
    public partial class ShowUserMenu : Form
    {
        private int current_user_id = new int();
        private string current_password = null;

        public ShowUserMenu()
        {
            InitializeComponent();
            CheckSecuritySettings();
            ArrayList user = FindUser("SELECT * FROM users u JOIN users_privilege up ON u.user_id=up.user_id JOIN positions_directory pd ON u.position=pd.position_id ORDER BY u.user_id LIMIT 1");
            ShowUser(user);
            user = FindUser($"SELECT * FROM users u JOIN users_privilege up ON u.user_id=up.user_id JOIN positions_directory pd ON u.position=pd.position_id WHERE u.user_id > {current_user_id} ORDER BY u.user_id ASC LIMIT 1");
            button1.Enabled = false;
            if (user.Count == 0)
            {
                button2.Enabled = false;
            }
        }

        public void CreateSqlParameter(NpgsqlCommand cmd, string par_name, NpgsqlTypes.NpgsqlDbType par_type, object Value)
        {
            NpgsqlParameter param = cmd.CreateParameter();
            param.ParameterName = par_name;
            param.NpgsqlDbType = par_type;
            param.Value = Value;
            cmd.Parameters.Add(param);
        }

        private void CheckSecuritySettings()
        {
            if (!User.CheckPrivilege("rows_delete", "users"))
                button4.Enabled = false;

            if (!User.CheckPrivilege("rows_change", "users"))
                button6.Enabled = false;
        }

        private void GetUserPrivilege(string table_info, CheckedListBox checkedListBox)
        {
            for (int i = 0; i < checkedListBox.Items.Count; i++)
            {
                checkedListBox.SetItemChecked(i, false);
            }

            string[] tables = table_info.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string table in tables)
            {
                switch (table)
                {
                    case "archive":
                        checkedListBox.SetItemChecked(0, true);
                        break;
                    case "candidate":
                        checkedListBox.SetItemChecked(1, true);
                        break;
                    case "candidate_log":
                        checkedListBox.SetItemChecked(2, true);
                        break;
                    case "client":
                        checkedListBox.SetItemChecked(3, true);
                        break;
                    case "clients_log":
                        checkedListBox.SetItemChecked(4, true);
                        break;
                    case "meeting_log":
                        checkedListBox.SetItemChecked(5, true);
                        break;
                    case "payment_log":
                        checkedListBox.SetItemChecked(6, true);
                        break;
                    case "positions_directory":
                        checkedListBox.SetItemChecked(7, true);
                        break;
                    case "staff_directory":
                        checkedListBox.SetItemChecked(8, true);
                        break;
                    case "users":
                        checkedListBox.SetItemChecked(9, true);
                        break;
                }
            }
        }

        private string SetUserPrivilege(CheckedListBox checkedListBox)
        {
            string table_info = "";

            for (int i = 0; i < checkedListBox.Items.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        if (checkedListBox.GetItemChecked(i) == true)
                            table_info += "archive, ";
                        break;
                    case 1:
                        if (checkedListBox.GetItemChecked(i) == true)
                            table_info += "candidate, ";
                        break;
                    case 2:
                        if (checkedListBox.GetItemChecked(i) == true)
                            table_info += "candidate_log, ";
                        break;
                    case 3:
                        if (checkedListBox.GetItemChecked(i) == true)
                            table_info += "client, ";
                        break;
                    case 4:
                        if (checkedListBox.GetItemChecked(i) == true)
                            table_info += "clients_log, ";
                        break;
                    case 5:
                        if (checkedListBox.GetItemChecked(i) == true)
                            table_info += "meeting_log, ";
                        break;
                    case 6:
                        if (checkedListBox.GetItemChecked(i) == true)
                            table_info += "payment_log, ";
                        break;
                    case 7:
                        if (checkedListBox.GetItemChecked(i) == true)
                            table_info += "positions_directory, ";
                        break;
                    case 8:
                        if (checkedListBox.GetItemChecked(i) == true)
                            table_info += "staff_directory, ";
                        break;
                    case 9:
                        if (checkedListBox.GetItemChecked(i) == true)
                            table_info += "users, ";
                        break;
                }
            }

            return table_info;
        }

        private ArrayList FindUser(string query, List<object[]> parametres = null)
        {
            NpgsqlConnection con = new NpgsqlConnection($"Server=localhost;Port=5433;User ID=maksNN;Password=maksNN;Database=Dating_Agency;");
            con.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(query, con);
            if (parametres != null)
            {
                foreach (object[] parameter in parametres)
                    CreateSqlParameter(cmd, (string)parameter[0], (NpgsqlTypes.NpgsqlDbType)parameter[1], parameter[2]);
            }
            NpgsqlDataReader dr = cmd.ExecuteReader();
            ArrayList user = new ArrayList() { };
            if (dr.HasRows)
            {
                dr.Read();
                user = new ArrayList() { dr.GetInt32(0), dr.GetString(1), dr.GetString(2), dr.GetString(3), dr.GetString(4), dr.GetString(5), dr.GetInt32(6) };
                if (dr[7] is System.DBNull)
                {
                    user.Add(null);
                }
                else
                {
                    user.Add((byte[])dr[7]);
                }
                user.AddRange(new ArrayList { dr.GetInt32(8), dr.GetBoolean(9), dr.GetBoolean(10), dr.GetBoolean(11), dr.GetBoolean(12), dr.GetBoolean(13), dr.GetBoolean(14) });
                user.AddRange(new ArrayList { dr.GetBoolean(15), dr.GetString(16), dr.GetString(17), dr.GetString(18), dr.GetString(19), dr.GetString(21) });
            }
            
            dr.Close();
            con.Close();
            return user;
        }

        private void ShowUser(ArrayList user)
        {
            current_user_id = Convert.ToInt32(user[0]);
            textBox1.Text = Convert.ToString(user[1]);
            textBox2.Text = Convert.ToString(user[2]);
            textBox3.Text = Convert.ToString(user[3]);
            textBox4.Text = Convert.ToString(user[4]);
            textBox5.Text = Convert.ToString(user[5]);

            current_password = Convert.ToString(user[5]);

            int Selected = 0;
            int count = comboBox1.Items.Count;
            for (int i = 0; i <= count - 1; i++)
            {
                comboBox1.SelectedIndex = i;
                string string_1 = comboBox1.GetItemText(comboBox1.SelectedItem);
                string string_2 = Convert.ToString(user[20]);
                if (string_1 == string_2)
                {
                    Selected = i;
                }
            }

            if (comboBox1.Items.Count != 0)
                comboBox1.SelectedIndex = Selected;

            if (user[7] == null)
            {
                pictureBox1.Image = null;
            }
            else
            {
                byte[] byteArray = (byte[])user[7];
                string filePath = String.Concat(Directory.GetCurrentDirectory(), "\\new_image.txt");

                using (MemoryStream memoryStream = new MemoryStream((byte[])user[7]))
                {
                    pictureBox1.Image = System.Drawing.Image.FromStream(memoryStream);
                }
            }

            if (Convert.ToBoolean(user[9]))
                checkBox1.Checked = true;
            else checkBox1.Checked = false;
            if (Convert.ToBoolean(user[10]))
                checkBox2.Checked = true;
            else checkBox2.Checked = false;
            if (Convert.ToBoolean(user[11]))
                checkBox3.Checked = true;
            else checkBox3.Checked = false;
            if (Convert.ToBoolean(user[12]))
                checkBox4.Checked = true;
            else checkBox4.Checked = false;
            if (Convert.ToBoolean(user[13]))
                checkBox5.Checked = true;
            else checkBox5.Checked = false;
            if (Convert.ToBoolean(user[14]))
                checkBox6.Checked = true;
            else checkBox6.Checked = false;
            if (Convert.ToBoolean(user[15]))
                checkBox7.Checked = true;
            else checkBox7.Checked = false;

            string show_table_info = Convert.ToString(user[16]);
            string add_table_info = Convert.ToString(user[17]);
            string delete_table_info = Convert.ToString(user[18]);
            string change_table_info = Convert.ToString(user[19]);

            GetUserPrivilege(show_table_info, checkedListBox1);
            GetUserPrivilege(add_table_info, checkedListBox2);
            GetUserPrivilege(delete_table_info, checkedListBox3);
            GetUserPrivilege(change_table_info, checkedListBox4);
        }

        private void EditUser()
        {
            if (button6.Text == "Изменить данные")
                button6.Text = "Подтвердить изменения";
            else button6.Text = "Изменить данные";


            if (button6.Text == "Подтвердить изменения")
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;

                textBox6.Enabled = false;

                if (User.CheckPrivilege("own_data_change") && User.CheckSettings("login") == textBox4.Text)
                {
                    textBox1.Enabled = true;
                    textBox2.Enabled = true;
                    textBox3.Enabled = true;
                    comboBox1.Enabled = true;
                }

                if (User.CheckPrivilege("another_data_change") && User.CheckSettings("login") != textBox4.Text)
                {
                    textBox1.Enabled = true;
                    textBox2.Enabled = true;
                    textBox3.Enabled = true;
                    comboBox1.Enabled = true;
                }

                if (User.CheckPrivilege("own_login_change") && User.CheckSettings("login") == textBox4.Text)
                {
                    textBox4.Enabled = true;
                }

                if (User.CheckPrivilege("another_login_change") && User.CheckSettings("login") != textBox4.Text)
                {
                    textBox4.Enabled = true;
                }

                if (User.CheckPrivilege("own_password_change") && User.CheckSettings("login") == textBox4.Text)
                {
                    textBox5.Enabled = true;
                }

                if (User.CheckPrivilege("another_password_change") && User.CheckSettings("login") != textBox4.Text)
                {
                    textBox5.Enabled = true;
                }

                if (User.CheckPrivilege("group_policy_change"))
                {
                    checkBox1.Enabled = true;
                    checkBox2.Enabled = true;
                    checkBox3.Enabled = true;
                    checkBox4.Enabled = true;
                    checkBox5.Enabled = true;
                    checkBox6.Enabled = true;
                    checkBox7.Enabled = true;
                    checkedListBox1.Enabled = true;
                    checkedListBox2.Enabled = true;
                    checkedListBox3.Enabled = true;
                    checkedListBox4.Enabled = true;
                }
            }
            else
            {
                ArrayList checkboxes = new ArrayList() { checkBox1.Checked, checkBox2.Checked, checkBox3.Checked, checkBox4.Checked, checkBox5.Checked, checkBox6.Checked, checkBox7.Checked };
                List<bool> checks = new List<bool>();
                for (int i = 0; i < checkboxes.Count; i++)
                {
                    if ((bool)checkboxes[i])
                        checks.Add(true);
                    else checks.Add(false);
                }

                List<string> table_privileges = new List<string>();
                table_privileges.Add(SetUserPrivilege(checkedListBox1));
                table_privileges.Add(SetUserPrivilege(checkedListBox2));
                table_privileges.Add(SetUserPrivilege(checkedListBox3));
                table_privileges.Add(SetUserPrivilege(checkedListBox4));

                NpgsqlConnection con = new NpgsqlConnection($"Server=localhost;Port=5433;User ID=maksNN;Password=maksNN;Database=Dating_Agency;");
                con.Open();

                NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM positions_directory WHERE position = @position", con);
                CreateSqlParameter(cmd, "@position", NpgsqlTypes.NpgsqlDbType.Text, comboBox1.GetItemText(comboBox1.SelectedItem));
                NpgsqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                int position_id = dr.GetInt32(0);
                dr.Close();

                cmd = new NpgsqlCommand($"UPDATE users_privilege " +
                                                      $"SET own_data_change=@check0, " +
                                                      $"own_login_change=@check1, " +
                                                      $"own_password_change=@check2, " +
                                                      $"another_data_change=@check3, " +
                                                      $"another_login_change=@check4, " +
                                                      $"another_password_change=@check5, " +
                                                      $"group_policy_change=@check6, " +
                                                      $"table_show=@table_privileges0, " +
                                                      $"table_add=@table_privileges1, " +
                                                      $"table_delete=@table_privileges2, " +
                                                      $"table_change=@table_privileges3 " +
                                                      $"WHERE user_id=@current_user_id; " +
                                                      $"UPDATE users " +
                                                      $"SET surname=@surname, name=@name, patronymic=@patronymic, login=@login, " +
                                                      $"position=@position " +
                                                      $"WHERE user_id=@current_user_id", con);
                CreateSqlParameter(cmd, "@check0", NpgsqlTypes.NpgsqlDbType.Boolean, checks[0]);
                CreateSqlParameter(cmd, "@check1", NpgsqlTypes.NpgsqlDbType.Boolean, checks[1]);
                CreateSqlParameter(cmd, "@check2", NpgsqlTypes.NpgsqlDbType.Boolean, checks[2]);
                CreateSqlParameter(cmd, "@check3", NpgsqlTypes.NpgsqlDbType.Boolean, checks[3]);
                CreateSqlParameter(cmd, "@check4", NpgsqlTypes.NpgsqlDbType.Boolean, checks[4]);
                CreateSqlParameter(cmd, "@check5", NpgsqlTypes.NpgsqlDbType.Boolean, checks[5]);
                CreateSqlParameter(cmd, "@check6", NpgsqlTypes.NpgsqlDbType.Boolean, checks[6]);
                CreateSqlParameter(cmd, "@table_privileges0", NpgsqlTypes.NpgsqlDbType.Text, table_privileges[0]);
                CreateSqlParameter(cmd, "@table_privileges1", NpgsqlTypes.NpgsqlDbType.Text, table_privileges[1]);
                CreateSqlParameter(cmd, "@table_privileges2", NpgsqlTypes.NpgsqlDbType.Text, table_privileges[2]);
                CreateSqlParameter(cmd, "@table_privileges3", NpgsqlTypes.NpgsqlDbType.Text, table_privileges[3]);
                CreateSqlParameter(cmd, "@current_user_id", NpgsqlTypes.NpgsqlDbType.Integer, current_user_id);
                CreateSqlParameter(cmd, "@surname", NpgsqlTypes.NpgsqlDbType.Text, textBox1.Text);
                CreateSqlParameter(cmd, "@name", NpgsqlTypes.NpgsqlDbType.Text, textBox2.Text);
                CreateSqlParameter(cmd, "@patronymic", NpgsqlTypes.NpgsqlDbType.Text, textBox3.Text);
                CreateSqlParameter(cmd, "@login", NpgsqlTypes.NpgsqlDbType.Text, textBox4.Text);
                CreateSqlParameter(cmd, "@position", NpgsqlTypes.NpgsqlDbType.Integer, position_id);

                cmd.ExecuteNonQuery();

                if (textBox5.Text != current_password)
                {
                    cmd = new NpgsqlCommand($"UPDATE users " +
                                            $"SET password=crypt(@password, gen_salt('md5')) " +
                                            $"WHERE user_id=@current_user_id", con);
                    CreateSqlParameter(cmd, "@password", NpgsqlTypes.NpgsqlDbType.Text, textBox5.Text);
                    CreateSqlParameter(cmd, "@current_user_id", NpgsqlTypes.NpgsqlDbType.Integer, current_user_id);

                    cmd.ExecuteNonQuery();
                }

                con.Close();

                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;

                textBox6.Enabled = true;

                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                comboBox1.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;

                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                checkBox3.Enabled = false;
                checkBox4.Enabled = false;
                checkBox5.Enabled = false;
                checkBox6.Enabled = false;
                checkBox7.Enabled = false;
                checkedListBox1.Enabled = false;
                checkedListBox2.Enabled = false;
                checkedListBox3.Enabled = false;
                checkedListBox4.Enabled = false;
            }
        }

        private void DeleteUser()
        {
            NpgsqlConnection con = new NpgsqlConnection($"Server=localhost;Port=5433;User ID=maksNN;Password=maksNN;Database=Dating_Agency;");
            con.Open();
            NpgsqlCommand cmd = new NpgsqlCommand($"DELETE FROM users_privilege WHERE user_id={current_user_id}; DELETE FROM users WHERE user_id={current_user_id};", con);
            cmd.ExecuteNonQuery();
            ArrayList user = FindUser("SELECT * FROM users u JOIN users_privilege up ON u.user_id=up.user_id JOIN positions_directory pd ON u.position=pd.position_id ORDER BY u.user_id LIMIT 1");
            ShowUser(user);
            user = FindUser($"SELECT * FROM users u JOIN users_privilege up ON u.user_id=up.user_id JOIN positions_directory pd ON u.position=pd.position_id WHERE u.user_id > {current_user_id} ORDER BY u.user_id LIMIT 1");
            button1.Enabled = false;
            button2.Enabled = true;
            if (user.Count == 0)
            {
                button2.Enabled = false;
            }
            con.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ArrayList user = FindUser($"SELECT * FROM users u JOIN users_privilege up ON u.user_id=up.user_id JOIN positions_directory pd ON u.position=pd.position_id WHERE u.user_id < {current_user_id} ORDER BY u.user_id DESC LIMIT 1");
            ShowUser(user);
            user = FindUser($"SELECT * FROM users u JOIN users_privilege up ON u.user_id=up.user_id JOIN positions_directory pd ON u.position=pd.position_id WHERE u.user_id < {current_user_id} ORDER BY u.user_id DESC LIMIT 1");
            button2.Enabled = true;
            if (user.Count == 0)
            {
                button1.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ArrayList user = FindUser($"SELECT * FROM users u JOIN users_privilege up ON u.user_id=up.user_id JOIN positions_directory pd ON u.position=pd.position_id WHERE u.user_id > {current_user_id} ORDER BY u.user_id LIMIT 1");
            ShowUser(user);
            user = FindUser($"SELECT * FROM users u JOIN users_privilege up ON u.user_id=up.user_id JOIN positions_directory pd ON u.position=pd.position_id WHERE u.user_id > {current_user_id} ORDER BY u.user_id LIMIT 1");
            button1.Enabled = true;
            if (user.Count == 0)
            {
                button2.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ArrayList user = FindUser($"SELECT * FROM users u JOIN users_privilege up ON u.user_id=up.user_id JOIN positions_directory pd ON u.position=pd.position_id " +
                                      $"WHERE surname like @pattern or name like @pattern or patronymic like @pattern or login like @pattern ORDER BY u.user_id LIMIT 1",
                                      new List<object[]>() { new object[] { "@pattern", NpgsqlTypes.NpgsqlDbType.Text, textBox6.Text } });
            if (user.Count == 0)
            {
                textBox6.Text = "";
                return;
            }
            ShowUser(user);
            user = FindUser($"SELECT * FROM users u JOIN users_privilege up ON u.user_id=up.user_id JOIN positions_directory pd ON u.position=pd.position_id WHERE u.user_id > {current_user_id} ORDER BY u.user_id LIMIT 1");
            button2.Enabled = true;
            if (user.Count == 0)
            {
                button2.Enabled = false;
            }
            user = FindUser($"SELECT * FROM users u JOIN users_privilege up ON u.user_id=up.user_id JOIN positions_directory pd ON u.position=pd.position_id WHERE u.user_id < {current_user_id} ORDER BY u.user_id LIMIT 1");
            button1.Enabled = true;
            if (user.Count == 0)
            {
                button1.Enabled = false;
            }
            textBox6.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DeleteUser();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            EditUser();
        }

        private void ShowUserMenu_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "dataSet1.positions_directory". При необходимости она может быть перемещена или удалена.
            this.positions_directoryTableAdapter.Fill(this.dataSet1.positions_directory);
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (Password.CheckPasswordValidation(textBox5.Text))
            {
                textBox5.BackColor = Color.FromArgb(255, 34, 94, 121);
            }
            else
            {
                textBox5.BackColor = Color.Red;
            }
        }
    }
}
