using Gama.Atenciones.Wpf.ViewModels;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gama.Atenciones.Wpf.Views
{
    public static class ExMeth
    {
        public static DependencyObject FindChild(this DependencyObject reference, string childName, Type childType)
        {
            DependencyObject foundChild = null;
            if (reference != null)
            {
                int childrenCount = VisualTreeHelper.GetChildrenCount(reference);
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(reference, i);
                    // If the child is not of the request child type child
                    if (child.GetType() != childType)
                    {
                        // recursively drill down the tree
                        foundChild = FindChild(child, childName, childType);
                    }
                    else if (!string.IsNullOrEmpty(childName))
                    {
                        var frameworkElement = child as FrameworkElement;
                        // If the child's name is set for search
                        if (frameworkElement != null && frameworkElement.Name == childName)
                        {
                            // if the child's name is of the request name
                            foundChild = child;
                            break;
                        }
                    }
                    else
                    {
                        // child element found.
                        foundChild = child;
                        break;
                    }
                }
            }
            return foundChild;
        }
    }
    /// <summary>
    /// Interaction logic for CitasContentView.xaml
    /// </summary>
    public partial class CitasContentView : UserControl
    {

        CitasContentViewModel _ViewModel;
        private bool _ToggleCalendarStyleCheckboxIsChecked = false;

        public CitasContentView()
        {
            InitializeComponent();

            //Loaded += CitasContentView_Loaded;
        }

        private void CircleIconButton_Click(object sender, RoutedEventArgs e)
        {
            _ToggleCalendarStyleCheckboxIsChecked = !_ToggleCalendarStyleCheckboxIsChecked;
            _ToggleCalendarStyleCheckbox.IsChecked = !_ToggleCalendarStyleCheckbox.IsChecked;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (_NavegationStackPanel.Visibility == Visibility.Visible)
            //    _CollapseFilter();
            //else
            //    _ExpandFilter();
        }

        private void CitasContentView_Loaded(object sender, RoutedEventArgs e)
        {
            _ViewModel = DataContext as CitasContentViewModel;
            
            if (_ViewModel.Preferencias.CitasContent_MostrarFiltroDeFechaPorDefecto)
                _Calendario.ExpandNavigation();
            else
                _Calendario.CollapseNavigation();
        }
    }
}
