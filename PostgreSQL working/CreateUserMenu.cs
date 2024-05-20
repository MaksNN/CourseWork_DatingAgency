using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using PasswordValidity;

namespace PostgreSQL_working
{
    public partial class CreateUserMenu : Form
    {
        public CreateUserMenu()
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

        public static byte[] ImageToByteArray(Image image)
        {
            ImageCodecInfo codecInfo = GetEncoderInfo(ImageFormat.Jpeg);

            EncoderParameters encoderParams = new EncoderParameters();
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, codecInfo, encoderParams);
                return ms.ToArray();
            }
        }

        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
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


        public void AddNewUser()
        {
            NpgsqlConnection con = new NpgsqlConnection($"Server=localhost;Port=5433;User ID=maksNN;Password=maksNN;Database=Dating_Agency;");
            con.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM positions_directory", con);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            int pos_id = new int();
            while (dr.Read())
            {
                int position_id = dr.GetInt32(0);
                string position = dr.GetString(1);
                if (position == comboBox1.GetItemText(comboBox1.SelectedItem))
                {
                    pos_id = position_id;
                    break;
                }
            }

            cmd = new NpgsqlCommand($"INSERT INTO users(surname, name, patronymic, login, password, position, photo) " +
                                    $"VALUES(@surname, @name, @patronymic, @login, crypt(@password, gen_salt('md5')), @position, @Image)", con);
            CreateSqlParameter(cmd, "@surname", NpgsqlTypes.NpgsqlDbType.Text, textBox1.Text);
            CreateSqlParameter(cmd, "@name", NpgsqlTypes.NpgsqlDbType.Text, textBox2.Text);
            CreateSqlParameter(cmd, "@patronymic", NpgsqlTypes.NpgsqlDbType.Text, textBox3.Text);
            CreateSqlParameter(cmd, "@login", NpgsqlTypes.NpgsqlDbType.Text, textBox4.Text);
            CreateSqlParameter(cmd, "@password", NpgsqlTypes.NpgsqlDbType.Text, textBox5.Text);
            CreateSqlParameter(cmd, "@position", NpgsqlTypes.NpgsqlDbType.Integer, pos_id);

            if (pictureBox1.Image != null)
                CreateSqlParameter(cmd, "@Image", NpgsqlTypes.NpgsqlDbType.Bytea, ImageToByteArray(pictureBox1.Image));
            else
                CreateSqlParameter(cmd, "@Image", NpgsqlTypes.NpgsqlDbType.Bytea, new byte[0]);

            dr.Close();
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand($"SELECT user_id FROM users WHERE login=@login", con);
            CreateSqlParameter(cmd, "@login", NpgsqlTypes.NpgsqlDbType.Text, textBox4.Text);
            dr = cmd.ExecuteReader();
            dr.Read();
            int new_user_id = dr.GetInt32(0);
            dr.Close();

            ArrayList checkboxes = new ArrayList() { checkBox1.Checked, checkBox2.Checked, checkBox3.Checked, checkBox4.Checked, checkBox5.Checked, checkBox6.Checked, checkBox7.Checked };
            List<bool> checks = new List<bool>();
            foreach (bool check in checkboxes)
            {
                if (check)
                    checks.Add(true);
                else
                    checks.Add(false);
            }

            List<string> table_privileges = new List<string>();
            table_privileges.Add(SetUserPrivilege(checkedListBox1));
            table_privileges.Add(SetUserPrivilege(checkedListBox2));
            table_privileges.Add(SetUserPrivilege(checkedListBox3));
            table_privileges.Add(SetUserPrivilege(checkedListBox4));

            cmd = new NpgsqlCommand($"INSERT INTO users_privilege(user_id, table_show, table_add, table_delete, table_change, own_data_change, own_login_change, own_password_change, another_data_change, another_login_change, another_password_change, group_policy_change) " +
                                    $"VALUES({new_user_id}, '{table_privileges[0]}', '{table_privileges[1]}', '{table_privileges[2]}', '{table_privileges[3]}', " +
                                    $"{checks[0]}, {checks[1]}, {checks[2]}, {checks[3]}, {checks[4]}, {checks[5]}, {checks[6]})", con);
            cmd.ExecuteNonQuery();
            
            con.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddNewUser();
            EventLog.AddEventLogInfo("Создан новый пользователь");
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CreateUserMenu_Load(object sender, EventArgs e)
        {
            this.positions_directoryTableAdapter.Fill(this.dataSet1.positions_directory);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                pictureBox1.Image = Image.FromFile(fileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
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
