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
    /// Interaction logic for EditarPersonaView.xaml
    /// </summary>
    public partial class EditarPersonaView : UserControl
    {
        public EditarPersonaView()
        {
            InitializeComponent();
        }

        private void _InformacionPersonalExpander_IsExpandedChanged(object sender, RoutedEventArgs e)
        {
            if (_InformacionPersonalExpander.IsExpanded)
            {
                _MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                _MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
            }
        }

        private void KeyBinding_GotFocus(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
        }
    }
}
