using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PasswordValidity
{
    public static class Password
    {
        public static bool CheckPasswordValidation(string password)
        {
            bool low_case_check = false;
            bool high_case_check = false;
            double digit_percent = 0;

            foreach (char c in password)
            {
                if (Regex.IsMatch(c.ToString(), @"[a-z]"))
                {
                    low_case_check = true;
                }
                else if (Regex.IsMatch(c.ToString(), @"[A-Z]"))
                {
                    high_case_check = true;
                }
                else if (!Regex.IsMatch(c.ToString(), @"[0-9]") &&
                         !Regex.IsMatch(c.ToString(), @"[\!,\.,\,,\?,\*,_,\-]"))
                {
                    return false;
                }
            }

            Regex digit_pattern = new Regex(@"[0-9]");
            double a = digit_pattern.Matches(password).Count;
            double b = password.Length;
            digit_percent = a / b;

            if (low_case_check == true && 
                high_case_check == true && 
                digit_percent <= 0.50 &&
                digit_percent > 0 &&
                password.Length >= 7 &&
                password.Length <= 30)
            {
                return true;
            }
            return false;
        }
    }
}
