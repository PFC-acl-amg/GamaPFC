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
        public Modulos ModuloSeleccionado { get; set; }
        public SelectorDeModulo()
        {
            InitializeComponent();
        }

        private void CooperacionButton_Click(object sender, RoutedEventArgs e)
        {
            ModuloSeleccionado = Modulos.Cooperacion;
            Close();
        }

        private void AtencionesButton_Click(object sender, RoutedEventArgs e)
        {
            ModuloSeleccionado = Modulos.ServicioDeAtenciones;
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ModuloSeleccionado = Modulos.GestionDeSocios;
            Close();
        }
    }
}
