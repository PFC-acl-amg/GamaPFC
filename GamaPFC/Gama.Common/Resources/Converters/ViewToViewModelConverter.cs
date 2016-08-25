using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Gama.Common.Resources.Converters
{
    public class ViewToViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var view = value as FrameworkElement;
            if (view == null)
                return null;

            Type viewModelType = Type.GetType((string)parameter);

            //var viewModel = this.Cast<viewModelType>(view.DataContext); //as viewModelType;
            //if (viewModel == null)
            //    return null;

            return null;
        }

        public T Cast<T>(object input)
        {
            return (T)input;
        }
    }
}
