using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgreSQL_working
{
    static class User
    {
        static string surname, name, patronymic, login, position;

        public static void SetSettings(string new_surname, 
                                       string new_name, 
                                       string new_patronymic, 
                                       string new_login, 
                                       string new_position)
        {
            surname = new_surname;
            name = new_name;
            patronymic = new_patronymic;
            login = new_login;
            position = new_position;
        }

        public static string CheckSettings(string argument)
        {
            switch (argument)
            {
                case "surname":
                    return surname;
                case "name":
                    return name;
                case "patronymic":
                    return patronymic;
                case "login":
                    return login;
                case "position":
                    return position;
                default: return "";
            }
        }

        public static void ResetSettings()
        {
            surname = null;
            name = null;
            patronymic = null;
            login = null;
            position = null;
        }

        public static void SetUserPrivilege(string[] new_rows_show, 
                                            string[] new_rows_add, 
                                            string[] new_rows_delete, 
                                            string[] new_rows_change, 
                                            bool new_own_data_change, 
                                            bool new_own_login_change, 
                                            bool new_own_password_change, 
                                            bool new_another_data_change, 
                                            bool new_another_login_change, 
                                            bool new_another_password_change, 
                                            bool new_group_policy_change)
        {
            UserPrivilege.SetUserPrivilege(new_rows_show, new_rows_add, new_rows_delete, new_rows_change, new_own_data_change, new_own_login_change, new_own_password_change, new_another_data_change, new_another_login_change, new_another_password_change, new_group_policy_change);
        }

        public static bool CheckPrivilege(string privilege_name, string table_name = null)
        {
            if (UserPrivilege.CheckPrivilege(privilege_name, table_name))
                return true;
            return false;
        }
    }
}
