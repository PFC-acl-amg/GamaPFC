using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Util
{
    public static class DateUtility
    {
        public static string[] Meses = { "N/A", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"};

        public static bool IsSameYearMonthDay(DateTime a, DateTime b)
        {
            return (a.Year == b.Year && a.Month == b.Month && a.Day == b.Day);
        }

        public static bool IsSameYearMonth(DateTime a, DateTime b)
        {
            return (a.Year == b.Year && a.Month == b.Month);
        }

        public static string ToYearMonth(DateTime a)
        {
            return string.Format("{0} de {1}", Meses[a.Month], a.Year);
        }
    }
}
