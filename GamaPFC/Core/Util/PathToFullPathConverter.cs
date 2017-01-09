using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Core.Util
{
    public class PathToFullPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = "";

            if (value != null)
            {
                path = AppDomain.CurrentDomain.BaseDirectory + value.ToString();
            }
            else
            {
                path = AppDomain.CurrentDomain.BaseDirectory + @"AvatarImages\default.png";
            }

            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
