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

namespace Gama.Bootstrapper.Views
{
    /// <summary>
    /// Interaction logic for ExcepctionMessageView.xaml
    /// </summary>
    public partial class ExceptionMessageView : Window
    {
        public ExceptionMessageView()
        {
            InitializeComponent();
        }

        private void _CerrarButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
