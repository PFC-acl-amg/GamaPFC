using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Gama.Cooperacion.Wpf.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class MonthNameConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var meses = new List<string>{ "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto",
            //    "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            var meses = new List<string>{ "ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO",
                "SEP", "OCT", "NOV", "DIC" };

            return meses[((int)value) - 1];
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
