using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Util
{
    public static class StringUtility
    {
        public static string EmailRegexPattern =
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        public static bool EmailIsNotEmptyAndIsMatch(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && EmailIsMatch(email);
        }

        public static bool EmailIsMatch(string email)
        {
            return !Regex.IsMatch(email, EmailRegexPattern, RegexOptions.IgnoreCase);
        }
    }
}
