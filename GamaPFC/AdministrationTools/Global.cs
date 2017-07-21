using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdministrationTools
{
    public static class Global
    {
        public static string User { get; set; }
        public static string Password { get; set; }
        public static bool IsLogged { get; set; } = false;
    }
}