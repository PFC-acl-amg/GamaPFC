using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Util
{
    public static class ExtensionMethods
    {
        public static bool IsBetween(this DateTime date, DateTime? initialDate, DateTime? finalDate)
        {
            bool result = true;

            if (initialDate.HasValue && initialDate > date)
                result = false;

            if (finalDate.HasValue && finalDate < date)
                result = false;

            return result;
        }

        public static bool IsBetween(this DateTime? date, DateTime? initialDate, DateTime? finalDate)
        {
            if (!date.HasValue)
                return false;

            return IsBetween(date.Value, initialDate, finalDate);
        }
    }
}
