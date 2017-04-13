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
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_DateFilterStackPanel.Visibility == Visibility.Visible)
            {
                _DateFilterStackPanel.Visibility = Visibility.Collapsed;
                _ToggleDateFilterButton.Margin = new Thickness(4, 4, 4, -34);

                var da = new DoubleAnimation(0, 180, new Duration(TimeSpan.FromSeconds(0.1)));
                var rt = new RotateTransform();
                _ToggleDateFilterButton.RenderTransform = rt;
                _ToggleDateFilterButton.RenderTransformOrigin = new Point(0.5, 0.5);
                rt.BeginAnimation(RotateTransform.AngleProperty, da);
            }
            else
            {
                _DateFilterStackPanel.Visibility = Visibility.Visible;
                _ToggleDateFilterButton.Margin = new Thickness(4);

                var da = new DoubleAnimation(180, 0, new Duration(TimeSpan.FromSeconds(0.1)));
                var rt = new RotateTransform();
                _ToggleDateFilterButton.RenderTransform = rt;
                _ToggleDateFilterButton.RenderTransformOrigin = new Point(0.5, 0.5);
                rt.BeginAnimation(RotateTransform.AngleProperty, da);
            }
        }
    }
}
