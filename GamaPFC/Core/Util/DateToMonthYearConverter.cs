using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Core.Util
{
    public class DateToMonthYearConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = value as DateTime?;

            if (date.HasValue)
            {
                return string.Format("{0} {1}", DateUtility.Meses[date.Value.Month].ToUpper().Substring(0, 3), date.Value.Year);
            }
            else
            {
                return "error";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
