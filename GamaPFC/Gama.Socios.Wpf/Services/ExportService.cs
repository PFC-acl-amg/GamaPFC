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
using System.Windows;
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

            string curFile = destinyPath;
            bool isFileInUse;

            isFileInUse = FileInUse(destinyPath);



            // Insertar Parrafo con el titulo de la tabla que se mostrará a continuación
            Paragraph title = document.InsertParagraph().Append("Información de Socio").
                FontSize(20).Font(new FontFamily("Times New Roman"));
                title.Alignment = Alignment.center;

            // Insert a Paragraph into this document.
            Paragraph P_TablaDatosDNI = document.InsertParagraph();

            Header(document);

            // Tablas de Datos
            Table DatosDNI = document.AddTable(6, 3); // Info Contenida en el DNI
            // Diseño de las Tablas
            DatosDNI.Design = TableDesign.MediumList2Accent4;
            P_TablaDatosDNI.InsertTableBeforeSelf(DatosDNI);
            P_TablaDatosDNI.AppendLine();

            Paragraph title2 = document.InsertParagraph().Append("Otros Datos").
                FontSize(20).Font(new FontFamily("Times New Roman"));
                title2.Alignment = Alignment.center;

            Paragraph P_TablaOtrosDatos = document.InsertParagraph();
            Table OtrosDatos = document.AddTable(7, 2); // Info Contenida en el DNI
            OtrosDatos.Design = TableDesign.MediumList2Accent4;
            P_TablaOtrosDatos.InsertTableBeforeSelf(OtrosDatos);

            Paragraph title3 = document.InsertParagraph().Append("Cuotas").
                FontSize(20).Font(new FontFamily("Times New Roman"));
                title3.Alignment = Alignment.center;
            Paragraph P_TablaCuotas = document.InsertParagraph();
            Table Cuotas = document.AddTable(6, 2); // Info Contenida en el DNI
            Cuotas.Design = TableDesign.MediumList2Accent4;
            P_TablaCuotas.InsertTableBeforeSelf(OtrosDatos);

            DatosDNI.AutoFit = AutoFit.ColumnWidth;
            OtrosDatos.AutoFit = AutoFit.ColumnWidth;
            Cuotas.AutoFit = AutoFit.ColumnWidth;
            DatosDNI.SetColumnWidth(0, 2500);
            DatosDNI.SetColumnWidth(1, 7000);
            OtrosDatos.SetColumnWidth(0, 2500);
            OtrosDatos.SetColumnWidth(1, 7000);
            Cuotas.SetColumnWidth(0, 2500);
            Cuotas.SetColumnWidth(1, 7000);

            //if (!string.IsNullOrEmpty(socio.AvatarPath))
            //{
            //    string avatar = socio.AvatarPath;
            //    //MemoryStream ms = new MemoryStream((byte)avatar);
            //    Novacode.Image image = document.AddImage(socio.AvatarPath);

            //    // Create a picture (A custom view of an Image).
            //    Picture picture = image.CreatePicture();
            //    picture.Rotation = 10;
            //    picture.SetPictureShape(BasicShapes.cube);
            //}


            MemoryStream stream = new MemoryStream(socio.Imagen);
            Novacode.Image img = document.AddImage(stream); // Create image.
            Picture pic1 = img.CreatePicture();     // Create picture.
            //pic1.Height = 179;
            //pic1.Width = 152;
            // Redimensionamos la imagen para que cuadre en la tabla manteniendo sus proporciones
            var ratioX = (double)152 / pic1.Width;
            var ratioY = (double)179 / pic1.Height;
            double ratio;
            if (ratioX < ratioY) { ratio = Math.Min(ratioX, ratioY); }
            else { ratio = Math.Max(ratioX, ratioY); }
            
            var newWidth = (int)(pic1.Width * ratio);
            var newHeight = (int)(pic1.Height * ratio);
            Picture NewPic = img.CreatePicture(newHeight, newWidth);

            //BitmapImage image = new BitmapImage();
            //image.BeginInit();
            //image.StreamSource = stream;
            //image.EndInit();
            //MemoryStream ms = new MemoryStream();
            //System.Drawing.Image myImg = System.Drawing.Image.FromFile(@"Images/Silueta.png");

            //myImg.Save(ms, myImg.RawFormat);  // Save your picture in a memory stream.
            //ms.Seek(0, SeekOrigin.Begin);

            //Novacode.Image img = document.AddImage(ms); // Create image.
            //Picture pic1 = img.CreatePicture();     // Create picture.

            //Insertando los valores en las celda de TablaDatosDNI
            DatosDNI.Rows[1].Cells[0].Paragraphs.First().AppendPicture(NewPic);
            DatosDNI.Rows[1].Cells[0].VerticalAlignment = Novacode.VerticalAlignment.Center;
            DatosDNI.MergeCellsInColumn(0, 1, 5);
            DatosDNI.Rows[1].Cells[1].Paragraphs.First().AppendLine(socio.Nombre);
            DatosDNI.Rows[2].Cells[1].Paragraphs.First().AppendLine(socio.Nif);
            DatosDNI.Rows[3].Cells[1].Paragraphs.First().AppendLine(socio.FechaDeNacimiento.ToString());
            DatosDNI.Rows[4].Cells[1].Paragraphs.First().AppendLine(socio.Nacionalidad);
            DatosDNI.Rows[5].Cells[1].Paragraphs.First().AppendLine(socio.DireccionPostal);

            // Insertando en la tabla de otros datos
            OtrosDatos.Rows[1].Cells[0].Paragraphs.First().AppendLine("Telefonos");
            OtrosDatos.Rows[1].Cells[1].Paragraphs.First().AppendLine(socio.Telefono);
            OtrosDatos.Rows[2].Cells[0].Paragraphs.First().AppendLine("Dado de Alta");
            OtrosDatos.Rows[2].Cells[1].Paragraphs.First().AppendLine(socio.CreatedAt.ToShortDateString());
            OtrosDatos.Rows[3].Cells[0].Paragraphs.First().AppendLine("Emails");
            OtrosDatos.Rows[3].Cells[1].Paragraphs.First().AppendLine(socio.Email);
            OtrosDatos.Rows[4].Cells[0].Paragraphs.First().AppendLine("Facebook");
            OtrosDatos.Rows[4].Cells[1].Paragraphs.First().AppendLine(socio.Facebook);
            OtrosDatos.Rows[5].Cells[0].Paragraphs.First().AppendLine("LinkedIn");
            OtrosDatos.Rows[5].Cells[1].Paragraphs.First().AppendLine(socio.LinkedIn);
            OtrosDatos.Rows[6].Cells[0].Paragraphs.First().AppendLine("Twitter");
            OtrosDatos.Rows[6].Cells[1].Paragraphs.First().AppendLine(socio.Twitter);

            document.InsertParagraph();
            // Save this document to disk.
            if (!FileInUse(destinyPath))
            {
                document.Save();
            }
            else
            {
                MessageBox.Show("El fichero está abierto. No se realizaron los cambios");
            }
            //document.Save();
            //Process.Start("WINWORD.EXE", destinyPath);        
        }

        private void Header(DocX document)
        {
            document.AddHeaders();
            document.DifferentFirstPage = true;

            Header header_first = document.Headers.first;
            Header headers = document.Headers.odd; // El resto de headers

            Paragraph p0 = header_first.InsertParagraph();
            p0.Append("Gamá - Gestión de Socios").FontSize(20).Bold();
            p0.Alignment = Alignment.center;

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

                using (var fileStream = new System.IO.FileStream(@"Images/gama_logo.jpg", System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
            
            MemoryStream ms = new MemoryStream();
            System.Drawing.Image myImg = System.Drawing.Image.FromFile(@"Images/gama_logo.jpg");

            myImg.Save(ms, myImg.RawFormat);  // Save your picture in a memory stream.
            ms.Seek(0, SeekOrigin.Begin);

            Novacode.Image img = document.AddImage(ms); // Create image.
            Picture pic1 = img.CreatePicture();     // Create picture.



            //p0.InsertPicture(pic1, 0); // Insert picture into paragraph.
            //File.Copy(op.FileName, Socio.AvatarPath, true);

            // Insert a Paragraph into the odd Header.
            //Paragraph p1 = headers.InsertParagraph();
            // p1.Append("Listado de Socios Impresos a " + DateTime.Now.ToLongTimeString()).Bold();
        }

        private string GeneratePath(string fileName)
        {
            if (!fileName.Contains(".doc"))
                return _DesktopPath + String.Format(@"\{0}.docx", fileName);
            else
                return _DesktopPath + String.Format(@"\{0}", fileName);
        }

        public void ExportarSocios(List<Socio> socios)
        {
            var destinyPath = GeneratePath("ListaSocios");
            DocX document = DocX.Create(destinyPath);
            string curFile = destinyPath;
            bool isFileInUse;
            isFileInUse = FileInUse(destinyPath);
            // Insertar Parrafo con el titulo de la tabla que se mostrará a continuación
            Paragraph title = document.InsertParagraph().Append("Listado Actual de Socios").
                FontSize(20).Font(new FontFamily("Times New Roman"));
            title.Alignment = Alignment.center;
            // --------------- Llamda a Header ---------------
            document.AddHeaders();
            document.DifferentFirstPage = true;

            Header header_first = document.Headers.first;
            Header headers = document.Headers.odd; // El resto de headers

            Paragraph p0 = header_first.InsertParagraph();
            p0.Append("Gamá - Gestión de Socios").FontSize(20).Bold();
            p0.Alignment = Alignment.center;
            // Insert a Paragraph into this document.
            Paragraph P_TablaDatosSocios = document.InsertParagraph();
            P_TablaDatosSocios.Alignment = Alignment.center;
            //-------------- Fin Llamada a Header-----------------
            //-------------- Crear la Tabla-----------------------
            // NumFilas = Socios.Cont + Titulos + Sumatoria
            // NumColumnas = 6 (Nombre,Dni,Telefono,CantidadPagada,CuotasPorPagar,CuotasImpagadas)
            // Tablas de Datos
            int NumFilas = socios.Count +2;
            int NumColumnas = 6;
            Table DatosSocios = document.AddTable(NumFilas, NumColumnas); // Info Contenida en el DNI
            DatosSocios.Alignment = Alignment.center;
            // Diseño de las Tablas
            DatosSocios.Design = TableDesign.MediumShading1;
            
            P_TablaDatosSocios.InsertTableBeforeSelf(DatosSocios);
            P_TablaDatosSocios.AppendLine();

            DatosSocios.AutoFit = AutoFit.ColumnWidth;
            DatosSocios.SetColumnWidth(0, 2000);
            DatosSocios.SetColumnWidth(1, 1700);
            DatosSocios.SetColumnWidth(2, 1700);
            DatosSocios.SetColumnWidth(3, 1700);
            DatosSocios.SetColumnWidth(4, 1700);
            DatosSocios.SetColumnWidth(5, 1900);

            //Insertando los valores en las celda de DatosSocios
            DatosSocios.Rows[0].Cells[0].Paragraphs.First().AppendLine("Nombre");
            DatosSocios.Rows[0].Cells[1].Paragraphs.First().AppendLine("DNI");
            DatosSocios.Rows[0].Cells[2].Paragraphs.First().AppendLine("Teléfono");
            DatosSocios.Rows[0].Cells[3].Paragraphs.First().AppendLine("CantidadPagada");
            DatosSocios.Rows[0].Cells[4].Paragraphs.First().AppendLine("CuotasPorPagar");
            DatosSocios.Rows[0].Cells[5].Paragraphs.First().AppendLine("CuotasImpagadas");

            int posSocio = 0;
            for(int i= 1; i <= (NumFilas-2); i++)
            {
                posSocio = i - 1;
                Socio socioLista = socios[posSocio];
                DatosSocios.Rows[i].Cells[0].Paragraphs.First().AppendLine(socioLista.Nombre);
                DatosSocios.Rows[i].Cells[1].Paragraphs.First().AppendLine(socioLista.Nif);
                DatosSocios.Rows[i].Cells[2].Paragraphs.First().AppendLine(socioLista.Telefono);
                DatosSocios.Rows[i].Cells[3].Paragraphs.First().AppendLine("Por Calcular");
                DatosSocios.Rows[i].Cells[4].Paragraphs.First().AppendLine("Por Calcular");
                DatosSocios.Rows[i].Cells[5].Paragraphs.First().AppendLine("Por Calcular");
            }
            // Relleno de la linea final con la sumatoria totasl de las cuotas
            DatosSocios.Rows[NumFilas-1].Cells[1].Paragraphs.First().AppendLine("Cantidad Total");
            DatosSocios.Rows[NumFilas-1].Cells[2].Paragraphs.First().AppendLine(socios.Count.ToString());
            DatosSocios.Rows[NumFilas-1].Cells[3].Paragraphs.First().AppendLine("Por Calcular");
            DatosSocios.Rows[NumFilas-1].Cells[4].Paragraphs.First().AppendLine("Por Calcular");
            DatosSocios.Rows[NumFilas-1].Cells[5].Paragraphs.First().AppendLine("Por Calcular");

            document.InsertParagraph();
            // Save this document to disk.
            if (!FileInUse(destinyPath))
            {
                document.Save();
            }
            else
            {
                MessageBox.Show("El fichero está abierto. No se realizaron los cambios");
            }
            //document.Save();
            //Process.Start("WINWORD.EXE", destinyPath);  

        }
        static bool FileInUse(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    
                }
                return false;
            }
            catch (IOException ex)
            {
                return true;
            }
        }

    }
}
