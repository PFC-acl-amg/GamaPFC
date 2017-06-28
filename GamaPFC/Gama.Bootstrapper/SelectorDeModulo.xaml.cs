using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace Gama.Bootstrapper
{
    /// <summary>
    /// Interaction logic for SelectorDeModulo.xaml
    /// </summary>
    public partial class SelectorDeModulo : MetroWindow
    {
        public SelectorDeModulo()
        {
            InitializeComponent();
        }

        private void UsuarioTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter & (sender as TextBox).AcceptsReturn == false) MoveToNextUIElement(e);
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((SelectorDeModuloViewModel)DataContext).AccederCommand.Execute(null);
            }
        }

        private void MoveToNextUIElement(KeyEventArgs e)
        {
            // Creating a FocusNavigationDirection object and setting it to a
            // local field that contains the direction selected.
            FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;

            // MoveFocus takes a TraveralReqest as its argument.
            TraversalRequest request = new TraversalRequest(focusDirection);

            // Gets the element with keyboard focus.
            UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;

            // Change keyboard focus.
            if (elementWithFocus != null)
            {
                if (elementWithFocus.MoveFocus(request)) e.Handled = true;
            }
        }

        private void _SociosButton_Checked(object sender, RoutedEventArgs e)
        {
            if (_SociosButton != null && _AtencionesButton != null && _CooperacionButton != null)
            {
                _SociosButton.IsChecked = true;
                _AtencionesButton.IsChecked = false;
                _CooperacionButton.IsChecked = false;
            }
        }

        private void _AtencionesButton_Checked(object sender, RoutedEventArgs e)
        {
            if (_SociosButton != null && _AtencionesButton != null && _CooperacionButton != null)
            {
                _SociosButton.IsChecked = false;
                _AtencionesButton.IsChecked = true;
                _CooperacionButton.IsChecked = false;
            }
        }

        private void _CooperacionButton_Checked(object sender, RoutedEventArgs e)
        {
            if (_SociosButton != null && _AtencionesButton != null && _CooperacionButton != null)
            {
                _SociosButton.IsChecked = false;
                _AtencionesButton.IsChecked = false;
                _CooperacionButton.IsChecked = true;
            }
        }
    }
}
