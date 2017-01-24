using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Gama.Common.Resources.Converters
{
    public class IconSourceToFullPathConverter : IValueConverter
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
                path = AppDomain.CurrentDomain.BaseDirectory + @"IconsAndImages\default_search_icon.png";
            }

            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
