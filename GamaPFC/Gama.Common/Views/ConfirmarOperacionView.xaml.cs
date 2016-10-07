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

namespace Gama.Common.Views
{
    public partial class ConfirmarOperacionView : Window
    {
        private string _Mensaje;

        public ConfirmarOperacionView()
        {
            InitializeComponent();

            _Mensaje = "Mensaje de confirmación por defecto...";
            MensajeTextBlock.Text = Mensaje;
            EstaConfirmado = false;
        }
        public string Mensaje
        {
            get { return _Mensaje; }
            set
            {
                _Mensaje = value;
                MensajeTextBlock.Text = _Mensaje;
            }
        }
        public bool EstaConfirmado { get; set; }

        private void SiButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmarYSalir(true);
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmarYSalir(false);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmarYSalir(false);
        }

        private void ConfirmarYSalir(bool resultado)
        {
            EstaConfirmado = resultado;
            Close();
        }
    }
}
