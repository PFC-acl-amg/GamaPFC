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
    /// Interaction logic for CitasContentView.xaml
    /// </summary>
    public partial class CitasContentView : UserControl
    {
        private bool _ToggleCalendarStyleCheckboxIsChecked = false;

        public CitasContentView()
        {
            InitializeComponent();
        }

        private void CircleIconButton_Click(object sender, RoutedEventArgs e)
        {
            _ToggleCalendarStyleCheckboxIsChecked = !_ToggleCalendarStyleCheckboxIsChecked;
            _ToggleCalendarStyleCheckbox.IsChecked = !_ToggleCalendarStyleCheckbox.IsChecked;
        }
    }
}
