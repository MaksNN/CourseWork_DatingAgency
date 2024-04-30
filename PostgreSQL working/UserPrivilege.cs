using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgreSQL_working
{
    static class UserPrivilege
    {
        static string[] rows_show, rows_add, rows_delete, rows_change;
        static bool own_data_change, own_login_change, own_password_change,
                    another_data_change, another_login_change, another_password_change,
                    group_policy_change;

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
            rows_show = new_rows_show;
            rows_add = new_rows_add;
            rows_delete = new_rows_delete;
            rows_change = new_rows_change;
            own_data_change = new_own_data_change;
            own_login_change = new_own_login_change;
            own_password_change = new_own_password_change;
            another_data_change = new_another_data_change;
            another_login_change = new_another_login_change;
            another_password_change = new_another_password_change;
            group_policy_change = new_group_policy_change;
        }



        public static bool CheckPrivilege(string privilege_name, string table_name = null)
        {
            switch (privilege_name)
            {
                case "rows_show":
                    for (int i = 0; i < rows_show.Length; i++)
                    {
                        if (rows_show[i] == table_name)
                            return true;
                    }
                    return false;
                case "rows_add":
                    for (int i = 0; i < rows_add.Length; i++)
                    {
                        if (rows_add[i] == table_name)
                            return true;
                    }
                    return false;
                case "rows_delete":
                    for (int i = 0; i < rows_delete.Length; i++)
                    {
                        if (rows_delete[i] == table_name)
                            return true;
                    }
                    return false;
                case "rows_change":
                    for (int i = 0; i < rows_change.Length; i++)
                    {
                        if (rows_change[i] == table_name)
                            return true;
                    }
                    return false;
                case "own_data_change":
                    if (own_data_change)
                        return true;
                    return false;
                case "own_login_change":
                    if (own_login_change)
                        return true;
                    return false;
                case "own_password_change":
                    if (own_password_change)
                        return true;
                    return false;
                case "another_data_change":
                    if (another_data_change)
                        return true;
                    return false;
                case "another_login_change":
                    if (another_login_change)
                        return true;
                    return false;
                case "another_password_change":
                    if (another_password_change)
                        return true;
                    return false;
                case "group_policy_change":
                    if (group_policy_change)
                        return true;
                    return false;
                default:
                    return false;
            }
        }
    }
}
