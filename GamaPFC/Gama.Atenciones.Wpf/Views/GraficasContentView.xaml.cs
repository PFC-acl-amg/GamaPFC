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
    /// <summary>
    /// Interaction logic for GraficasView.xaml
    /// </summary>
    public partial class GraficasContentView : UserControl
    {
        public GraficasContentView()
        {
            InitializeComponent();
        }

        private void _ToggleVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (_PanelDeNavegacion.Visibility == Visibility.Visible)
            {
                _PanelDeNavegacion.Width = double.NaN; // para "Auto"
                _PanelDeNavegacion.Visibility = Visibility.Collapsed;
                _ToggleVisibilityButton.Margin = new Thickness(4, 9, 4, 4);

                var da = new DoubleAnimation(180, 0, new Duration(TimeSpan.FromSeconds(0.1)));
                var rt = new RotateTransform();
                _ToggleVisibilityButton.RenderTransform = rt;
                _ToggleVisibilityButton.RenderTransformOrigin = new Point(0.5, 0.5);
                rt.BeginAnimation(RotateTransform.AngleProperty, da);
            }
            else
            {
                _PanelDeNavegacion.Width = 210.0; // para "Auto"
                _PanelDeNavegacion.Visibility = Visibility.Visible;
                _ToggleVisibilityButton.Margin = new Thickness(4, 9, 4, 4);

                var da = new DoubleAnimation(0, 180, new Duration(TimeSpan.FromSeconds(0.1)));
                var rt = new RotateTransform();
                _ToggleVisibilityButton.RenderTransform = rt;
                _ToggleVisibilityButton.RenderTransformOrigin = new Point(0.5, 0.5);
                rt.BeginAnimation(RotateTransform.AngleProperty, da);
            }
        }

        private void DatosPersonalesListBoxItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _DatosPersonalesGrafivasView.Visibility = Visibility.Visible;
            _AtencionesGraficasView.Visibility = Visibility.Hidden;
        }

        private void AtencionesListBoxItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _DatosPersonalesGrafivasView.Visibility = Visibility.Hidden;
            _AtencionesGraficasView.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _FiltroGrid.Visibility = _FiltroGrid.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
