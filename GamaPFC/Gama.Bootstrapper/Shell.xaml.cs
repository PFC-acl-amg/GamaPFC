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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gama.Bootstrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : MetroWindow
    {
        //private SelectorDeModulo _SelectorDeModulo;

        public Shell()
        {
            //_SelectorDeModulo = new SelectorDeModulo();
            //_SelectorDeModulo.Topmost = true;
            //_SelectorDeModulo.Show();

            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //_SelectorDeModulo = new SelectorDeModulo();
            //preloaderRegion.Children.Add(_SelectorDeModulo);
            //_SelectorDeModulo = null;
        }
    }
}
