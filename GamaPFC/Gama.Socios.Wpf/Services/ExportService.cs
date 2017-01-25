using Gama.Socios.Business;
using Novacode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Gama.Socios.Wpf.Services
{
    public class ExportService
    {
        private static string _DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //+ @"\HelloWorld.docx";

        public ExportService()
        {

        }

        public void ExportarSocio(Socio socio, string fileName)
        {
            var destinyPath = GeneratePath(fileName);
            DocX document = DocX.Create(destinyPath);

            // Insert a Paragraph into this document.
            Paragraph p = document.InsertParagraph();

            Header(document);

            Table table = document.AddTable(11, 2);
            Table t1 = document.InsertTable(table);

            t1.AutoFit = AutoFit.ColumnWidth;
            t1.SetColumnWidth(0, 1500);
            t1.SetColumnWidth(1, 8000);

            if (!string.IsNullOrEmpty(socio.AvatarPath))
            {
                Novacode.Image image = document.AddImage(socio.AvatarPath);

                // Create a picture (A custom view of an Image).
                Picture picture = image.CreatePicture();
                picture.Rotation = 10;
                picture.SetPictureShape(BasicShapes.cube);

                t1.Rows[0].Cells[0].Paragraphs.First().AppendPicture(picture);
            }

            t1.Rows[0].Cells[1].Paragraphs.First().AppendLine(socio.Nombre);
            t1.Rows[1].Cells[0].Paragraphs.First().AppendLine("Nombre");
            t1.Rows[1].Cells[1].Paragraphs.First().AppendLine(socio.Nombre);
            t1.Rows[2].Cells[0].Paragraphs.First().AppendLine("NIF");
            t1.Rows[2].Cells[1].Paragraphs.First().AppendLine(socio.Nif);
            t1.Rows[3].Cells[0].Paragraphs.First().AppendLine("Dirección Postal");
            t1.Rows[3].Cells[1].Paragraphs.First().AppendLine(socio.DireccionPostal);
            t1.Rows[4].Cells[0].Paragraphs.First().AppendLine("Email");
            t1.Rows[4].Cells[1].Paragraphs.First().AppendLine(socio.Email);
            t1.Rows[5].Cells[0].Paragraphs.First().AppendLine("Fecha de Nacimiento");
            t1.Rows[5].Cells[1].Paragraphs.First().AppendLine(socio.FechaDeNacimiento.ToString());
            t1.Rows[6].Cells[0].Paragraphs.First().AppendLine("Linkedin");
            t1.Rows[6].Cells[1].Paragraphs.First().AppendLine(socio.LinkedIn);
            t1.Rows[7].Cells[0].Paragraphs.First().AppendLine("Nacionalidad");
            t1.Rows[7].Cells[1].Paragraphs.First().AppendLine(socio.Nacionalidad);
            t1.Rows[8].Cells[0].Paragraphs.First().AppendLine("Telefono");
            t1.Rows[8].Cells[1].Paragraphs.First().AppendLine(socio.Telefono);
            t1.Rows[9].Cells[0].Paragraphs.First().AppendLine("Twitter");
            t1.Rows[9].Cells[1].Paragraphs.First().AppendLine(socio.Twitter);
            t1.Rows[10].Cells[0].Paragraphs.First().AppendLine("Facebook");
            t1.Rows[10].Cells[1].Paragraphs.First().AppendLine(socio.Facebook);

            document.InsertParagraph();

            // Save this document to disk.
            document.Save();
            //Process.Start("WINWORD.EXE", destinyPath);
        }

        private void Header(DocX document)
        {
            document.AddHeaders();
            document.DifferentFirstPage = true;

            Header header_first = document.Headers.first;
            Header headers = document.Headers.odd; // El resto de headers

            Paragraph p0 = header_first.InsertParagraph();
            p0.Append("Gamá - Gestión de Socios").Bold();

            

            //Novacode.Image image = document.AddImage(logo.);
            //Picture picture = image.CreatePicture();
            //p0.AppendPicture(picture);

            if (!Directory.Exists("Images"))
            {
                Directory.CreateDirectory("Images");
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                var uri = new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/logo_gama.jpg", UriKind.Absolute);
                logo.UriSource = uri;
                logo.EndInit();

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(logo));

                using (var fileStream = new System.IO.FileStream(@"Images/gama_logo,jpg", System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }

            MemoryStream ms = new MemoryStream();
            System.Drawing.Image myImg = System.Drawing.Image.FromFile(@"Images/gama_logo,jpg");

            myImg.Save(ms, myImg.RawFormat);  // Save your picture in a memory stream.
            ms.Seek(0, SeekOrigin.Begin);

            Novacode.Image img = document.AddImage(ms); // Create image.
            Picture pic1 = img.CreatePicture();     // Create picture.

            p0.InsertPicture(pic1, 0); // Insert picture into paragraph.
            //File.Copy(op.FileName, Socio.AvatarPath, true);

            // Insert a Paragraph into the odd Header.
            Paragraph p1 = headers.InsertParagraph();
            p1.Append("Listado de Socios Impresos a " + DateTime.Now.ToLongTimeString()).Bold();
        }

        private string GeneratePath(string fileName)
        {
            if (!fileName.Contains(".doc"))
                return _DesktopPath + String.Format(@"\{0}.docx", fileName);
            else
                return _DesktopPath + String.Format(@"\{0}", fileName);
        }

        public void ExportarSocios(IEnumerable<Socio> socios)
        {

        }
    }
}
