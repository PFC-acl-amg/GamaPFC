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
    /// Interaction logic for ActividadInformacionBasicaView.xaml
    /// </summary>
    public partial class ActividadInformacionBasicaView : UserControl
    {
        double? offset;

        public ActividadInformacionBasicaView()
        {
            InitializeComponent();
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            Point mousePosition = Mouse.GetPosition(this);

            if (offset == null)
                offset = popup.HorizontalOffset;

            popup.HorizontalOffset = offset.Value - popup.Child.RenderSize.Width;
        }
    }
}
