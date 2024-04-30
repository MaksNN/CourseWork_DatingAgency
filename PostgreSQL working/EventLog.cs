using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace PostgreSQL_working
{
    static class EventLog
    {
        public static void AddEventLogInfo(string operation)
        {
            NpgsqlConnection con = new NpgsqlConnection($"Server=localhost;Port=5432;User ID=maksNN;Password=maksNN;Database=Dating_Agency;");
            con.Open();
            NpgsqlCommand cmd = new NpgsqlCommand($"INSERT INTO event_log(user_login, user_surname, user_name, user_patronymic, event_data, event_time, operation) " +
                                                  $"VALUES('{User.CheckSettings("login")}', '{User.CheckSettings("surname")}', '{User.CheckSettings("name")}', '{User.CheckSettings("patronymic")}', @event_data, @event_time, '{operation}')", con);
            DateTime event_date = DateTime.Now.Date;
            TimeSpan event_time = new TimeSpan(DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.TimeOfDay.Seconds);
            cmd.Parameters.Add(new NpgsqlParameter("@event_data", NpgsqlTypes.NpgsqlDbType.Date));
            cmd.Parameters[0].Value = event_date;
            cmd.Parameters.Add(new NpgsqlParameter("@event_time", NpgsqlTypes.NpgsqlDbType.Time));
            cmd.Parameters[1].Value = event_time;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static TimeSpan StripMilliseconds(this TimeSpan time)
        {
            return new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);
        }
    }
}
