using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Util
{
    public static class DateUtility
    {
        public static bool IsSameYearMonthDay(DateTime a, DateTime b)
        {
            return (a.Year == b.Year && a.Month == b.Month && a.Day == b.Day);
        }

        public static bool IsSameYearMonth(DateTime a, DateTime b)
        {
            return (a.Year == b.Year && a.Month == b.Month);
        }
    }
}
