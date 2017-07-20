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
    /// Interaction logic for ActividadDetailView.xaml
    /// </summary>
    public partial class EditarActividadView : UserControl
    {
        public EditarActividadView()
        {
            InitializeComponent();

            var column1 = _InformacionDeActividadGrid.ColumnDefinitions[0];
            var column3 = _InformacionDeActividadGrid.ColumnDefinitions[2];
            column1.Width = new GridLength(0, GridUnitType.Star);
            column3.Width = new GridLength(99, GridUnitType.Star);
            _InformacionDeActividadStackPanel.Visibility = Visibility.Collapsed;
        }

        private void VerOpcionesActividades_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var column1 = _InformacionDeActividadGrid.ColumnDefinitions[0];
            var column3 = _InformacionDeActividadGrid.ColumnDefinitions[2];
            if (column1.Width.Value == 28)
            {
                column1.Width = new GridLength(0, GridUnitType.Star);
                column3.Width = new GridLength(99, GridUnitType.Star);
                _InformacionDeActividadStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                column1.Width = new GridLength(28, GridUnitType.Star);
                column3.Width = new GridLength(71, GridUnitType.Star);
                _InformacionDeActividadStackPanel.Visibility = Visibility.Visible;
            }
        }
    }
}
