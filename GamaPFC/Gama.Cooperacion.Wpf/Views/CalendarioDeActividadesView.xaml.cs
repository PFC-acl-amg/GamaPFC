using Gama.Cooperacion.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gama.Cooperacion.Wpf.Views
{
    /// <summary>
    /// Lógica de interacción para CalendarioActividadesView.xaml
    /// </summary>
    public partial class CalendarioDeActividadesView : UserControl
    {
        private bool _ToggleCalendarStyleCheckboxIsChecked = false;

        public CalendarioDeActividadesView()
        {
            InitializeComponent();
        }

        private void CircleIconButton_Click(object sender, RoutedEventArgs e)
        {
            _ToggleCalendarStyleCheckboxIsChecked = !_ToggleCalendarStyleCheckboxIsChecked;
            _ToggleCalendarStyleCheckbox.IsChecked = !_ToggleCalendarStyleCheckbox.IsChecked;

            var viewModel = DataContext as CalendarioDeActividadesViewModel;
            if (!_ToggleCalendarStyleCheckboxIsChecked)
            {
                viewModel._AplicarFiltroDeFecha = false;
                viewModel.FiltrarPorFecha();
            }
            else
            {
                viewModel._AplicarFiltroDeFecha = true;
                viewModel.FiltrarPorFecha();
            }
        }
    }
}
