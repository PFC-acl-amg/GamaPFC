using Gama.Atenciones.Wpf.DesignTimeData;
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

namespace Gama.Atenciones.Wpf.Views
{
    /// <summary>
    /// Interaction logic for EditarCitasView.xaml
    /// </summary>
    public partial class EditarCitasView : UserControl
    {
        private bool _ToggleCalendarStyleCheckboxIsChecked = false;
        public EditarCitasView()
        {
            InitializeComponent();
            //var vm = new EditarCitasViewModelDTD();
            //DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _Calendario.Style = FindResource("_ListadoCalendarStyle") as Style;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _Calendario.Style = FindResource("_EditarCitasCalendarStyle") as Style;
        }

        private void CircleIconButton_Click(object sender, RoutedEventArgs e)
        {
            _ToggleCalendarStyleCheckboxIsChecked = !_ToggleCalendarStyleCheckboxIsChecked;
            _ToggleCalendarStyleCheckbox.IsChecked = !_ToggleCalendarStyleCheckbox.IsChecked;
        }
    }
}
