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

namespace Gama.Atenciones.Wpf.Views.Asistente
{
    /// <summary>
    /// Interaction logic for DatosPersonales.xaml
    /// </summary>
    public partial class DatosPersonales : UserControl
    {
        public DatosPersonales()
        {
            InitializeComponent();
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            if (ellipse != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(ellipse,
                                    ellipse.Fill.ToString(),
                                    DragDropEffects.Copy);
            }
        }

        private void Ellipse_DragEnter(object sender, DragEventArgs e)
        {
        }

        private void Ellipse_DragLeave(object sender, DragEventArgs e)
        {

        }

        private void Ellipse_Drop(object sender, DragEventArgs e)
        {
            var x = e.Data.GetData(DataFormats.FileDrop) as string[];
            byte[] image = File.ReadAllBytes(x.First());

            _ImagenImageBrush.ImageSource = ByteToImage(image);

        }

        public static ImageSource ByteToImage(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            ImageSource imgSrc = biImg as ImageSource;

            return imgSrc;
        }
    }
}
