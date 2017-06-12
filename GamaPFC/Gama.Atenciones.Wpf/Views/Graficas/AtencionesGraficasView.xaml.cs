using System;
using System.Collections.Generic;
using System.IO;
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

namespace Gama.Atenciones.Wpf.Views.Graficas
{
    /// <summary>
    /// Interaction logic for AtencionesGraficasView.xaml
    /// </summary>
    public partial class AtencionesGraficasView : UserControl
    {
        public AtencionesGraficasView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)piechart.ActualWidth, (int)piechart.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(piechart);

            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
            MemoryStream stream = new MemoryStream();
            png.Save(stream);

            var imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = stream;
            imageSource.EndInit();

            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);

            _Image.Source = rtb;
        }
    }
}
