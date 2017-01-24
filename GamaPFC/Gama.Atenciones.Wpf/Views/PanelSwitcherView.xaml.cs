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
    /// Interaction logic for PanelSwitcherView.xaml
    /// </summary>
    public partial class PanelSwitcherView : UserControl
    {
        private SolidColorBrush _ActiveBrush = new SolidColorBrush(Color.FromRgb(0X27,0X5B,0X6D));
        private SolidColorBrush _InactiveBrush = new SolidColorBrush(Colors.White);

        public PanelSwitcherView()
        {
            InitializeComponent();
        }

        private void _Title1_Click(object sender, RoutedEventArgs e)
        {
            _Title1.Foreground = _ActiveBrush;
            _Title2.Foreground = _InactiveBrush;
            _Title3.Foreground = _InactiveBrush;
        }

        private void _Title2_Click(object sender, RoutedEventArgs e)
        {
            _Title1.Foreground = _InactiveBrush;
            _Title2.Foreground = _ActiveBrush;
            _Title3.Foreground = _InactiveBrush;
        }

        private void _Title3_Click(object sender, RoutedEventArgs e)
        {
            _Title1.Foreground = _InactiveBrush;
            _Title2.Foreground = _InactiveBrush;
            _Title3.Foreground = _ActiveBrush;
        }
    }
}
