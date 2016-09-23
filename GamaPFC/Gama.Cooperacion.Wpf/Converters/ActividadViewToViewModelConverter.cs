using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Gama.Cooperacion.Wpf.ViewModels;
using System.Windows.Data;

namespace Gama.Cooperacion.Wpf.Converters
{
    public class ActividadViewToViewModelConverter : IValueConverter
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

            var viewModel = view.DataContext as EditarActividadViewModel;
            if (viewModel == null)
                return null;

            return viewModel;
        }
    }
}
