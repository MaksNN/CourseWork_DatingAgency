using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Collections;

namespace DatabaseWork_NET_Framework_
{
    public static class DatabaseOperations
    {
        public static void CreateSqlParameter(NpgsqlCommand cmd, string par_name, NpgsqlTypes.NpgsqlDbType par_type, object Value)
        {
            NpgsqlParameter param = cmd.CreateParameter();
            param.ParameterName = par_name;
            param.NpgsqlDbType = par_type;
            param.Value = Value;
            cmd.Parameters.Add(param);
        }

        public static NpgsqlConnection Connect(string server, string port, string login, string password, string database)
        {
            try
            {
                NpgsqlConnection con = new NpgsqlConnection($"Server={server};Port={port};User ID={login};Password={password};Database={database};");
                con.Open();
                return con;
            }
            catch
            {
                throw new ConnectionException("Connection failed");
            }
        }

        public static NpgsqlConnection Disconnect(NpgsqlConnection con)
        {
            con.Close();
            return con;
        }

        public static bool Authenticate(string login, string password)
        {
            NpgsqlConnection con = Connect("localhost", "5433", "maksNN", "maksNN", "Dating_Agency");

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM users u " +
                                                  "WHERE login=@login and password=crypt(@password, password)", con);
            CreateSqlParameter(cmd, "@login", NpgsqlTypes.NpgsqlDbType.Text, login);
            CreateSqlParameter(cmd, "@password", NpgsqlTypes.NpgsqlDbType.Text, password);

            NpgsqlDataReader dr = cmd.ExecuteReader();
            bool check_access = dr.Read();
            con = Disconnect(con);

            if (check_access) return true;
            return false;
        }

        public static ArrayList GetUserData(string login, string password)
        {
            NpgsqlConnection con = Connect("localhost", "5433", "maksNN", "maksNN", "Dating_Agency");

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM users u " +
                                                  "JOIN users_privilege up ON u.user_id=up.user_id " +
                                                  "JOIN positions_directory pd ON u.position=pd.position_id " +
                                                  "WHERE login=@login and password=crypt(@password, password)", con);
            CreateSqlParameter(cmd, "@login", NpgsqlTypes.NpgsqlDbType.Text, login);
            CreateSqlParameter(cmd, "@password", NpgsqlTypes.NpgsqlDbType.Text, password);
            try
            {
                NpgsqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                ArrayList user_data = new ArrayList();

                user_data.AddRange(new string[] { dr.GetString(1), dr.GetString(2), dr.GetString(3), dr.GetString(4), dr.GetString(21) });
                user_data.AddRange(new string[][] { dr.GetString(16).Replace(" ", "").Split(','),
                                                    dr.GetString(17).Replace(" ", "").Split(','),
                                                    dr.GetString(18).Replace(" ", "").Split(','),
                                                    dr.GetString(19).Replace(" ", "").Split(',') });
                user_data.AddRange(new bool[] { (bool)dr[9], (bool)dr[10], (bool)dr[11], (bool)dr[12], (bool)dr[13], (bool)dr[14], (bool)dr[15] });
                con = Disconnect(con);
                return user_data;
            }
            catch
            {
                con = Disconnect(con);
                throw new GetDataException("No data received");
            }
        }
    }

    public class ConnectionException : Exception
    {
        public ConnectionException(string message) : base(message) { }
    }

    public class GetDataException : Exception
    {
        public GetDataException(string message) : base(message) { }
    }
}
