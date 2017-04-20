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
    /// Interaction logic for AsistentesView.xaml
    /// </summary>
    public partial class AsistentesContentView : UserControl
    {
        public AsistentesContentView()
        {
            InitializeComponent();
        }

        private void _ToggleDateFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (_ListadoDeAsistentesGrid.Visibility == Visibility.Visible)
            {
                _ListadoDeAsistentesGrid.Width = double.NaN; // para "Auto"
                _ListadoDeAsistentesGrid.Visibility = Visibility.Collapsed;
                _ToggleDateFilterButton.Margin = new Thickness(-8,4,4,4);

                var da = new DoubleAnimation(180, 0, new Duration(TimeSpan.FromSeconds(0.1)));
                var rt = new RotateTransform();
                _ToggleDateFilterButton.RenderTransform = rt;
                _ToggleDateFilterButton.RenderTransformOrigin = new Point(0.5, 0.5);
                rt.BeginAnimation(RotateTransform.AngleProperty, da);
            }
            else
            {
                _ListadoDeAsistentesGrid.Width = 190.0; // para "Auto"
                _ListadoDeAsistentesGrid.Visibility = Visibility.Visible;
                _ToggleDateFilterButton.Margin = new Thickness(4, 4, 4, 4);

                var da = new DoubleAnimation(0, 180, new Duration(TimeSpan.FromSeconds(0.1)));
                var rt = new RotateTransform();
                _ToggleDateFilterButton.RenderTransform = rt;
                _ToggleDateFilterButton.RenderTransformOrigin = new Point(0.5, 0.5);
                rt.BeginAnimation(RotateTransform.AngleProperty, da);
            }
        }
    }
}
