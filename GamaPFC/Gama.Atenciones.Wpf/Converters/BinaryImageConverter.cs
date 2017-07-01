using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Xceed.Wpf.DataGrid.Converters;

namespace Gama.Atenciones.Wpf.Converters
{
    public class BinaryImageConverter : IValueConverter
    {
        object IValueConverter.Convert(object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value != null && value is byte[])
            {
                byte[] bytes = value as byte[];

                MemoryStream stream = new MemoryStream(bytes);

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;

                try
                {
                    image.EndInit();
                }
                catch (NotSupportedException ex)
                {
                    throw;
                }

                return image;
            }

            //Imagen por defecto
            if (value == null)
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri("pack://application:,,,/Core;component/Resources/Images/persona_dummy.png");
                logo.EndInit();
                return logo;
            }

            // No se debería llegar aquí
            return null;
        }

        object IValueConverter.ConvertBack(object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            return ImageToByte(value as BitmapImage);
        }

        public static byte[] GetBitmapImageFromUriSource(Uri uriSource)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = uriSource;
            bitmapImage.EndInit();

            return ImageToByte(bitmapImage);
        }
        
        public static byte[] ImageToByte(BitmapImage imageSource)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageSource));

            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                return ms.ToArray();
            }
        }
    }
}
