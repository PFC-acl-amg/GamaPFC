using Gama.Atenciones.Business;
using Novacode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Gama.Atenciones.Wpf.Services
{
    public class ExportService
    {
        private string _DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public ExportService()
        {
            CitasPasadas = new List<Cita>();
            CitasFuturas = new List<Cita>();
        }

        private void Header(DocX document)
        {
            document.AddHeaders();
            document.DifferentFirstPage = true;

            Header header_first = document.Headers.first;
            Header headers = document.Headers.odd; // El resto de headers

            Paragraph p0 = header_first.InsertParagraph();
            p0.Append("Gamá - Servicio de Atenciones").FontSize(20).Bold();
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

                using (var fileStream = new System.IO.FileStream(@"Images/logo_gama.jpg", System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }

            MemoryStream ms = new MemoryStream();
            System.Drawing.Image myImg = System.Drawing.Image.FromFile(@"Images/logo_gama.jpg");

            myImg.Save(ms, myImg.RawFormat);  // Save your picture in a memory stream.
            ms.Seek(0, SeekOrigin.Begin);

            Novacode.Image img = document.AddImage(ms); // Create image.
            Picture pic1 = img.CreatePicture();     // Create picture.

            p0.InsertPicture(pic1, 0); // Insert picture into paragraph.
                                       //File.Copy(op.FileName, Socio.AvatarPath, true);

            //Insert a Paragraph into the odd Header.
            Paragraph p1 = headers.InsertParagraph();
            p1.Append("Listado de peronas impresas a " + DateTime.Now.ToLongTimeString()).Bold();
        }

        private string GeneratePath(string fileName)
        {
            if (!fileName.Contains(".doc"))
                return _DesktopPath + String.Format(@"\{0}.docx", fileName);
            else
                return _DesktopPath + String.Format(@"\{0}", fileName);
        }

        private bool _FileInUse(string path)
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
        private List<Cita> CitasFuturas { get; set; }
        private List<Cita> CitasPasadas { get; set; }
        private DateTime _Now;
        
        public void ExportarPersona(Persona persona, string fileName)
        {
            DocX document = DocX.Create(fileName);

            bool isFileInUse;

            isFileInUse = _FileInUse(fileName);

            // Insertar Parrafo con el titulo de la tabla que se mostrará a continuación
            Paragraph title = document.InsertParagraph().Append("Información de Persona").
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

            if (persona.Imagen != null)
            {
                MemoryStream ms = new MemoryStream(persona.Imagen);
                Novacode.Image image = document.AddImage(ms);

                // Create a picture (A custom view of an Image).
                Picture picture = image.CreatePicture();
                picture.Rotation = 10;
                picture.SetPictureShape(BasicShapes.cube);
            }


            MemoryStream stream = new MemoryStream(persona.Imagen);
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
            DatosDNI.Rows[1].Cells[1].Paragraphs.First().AppendLine(persona.Nombre);
            DatosDNI.Rows[2].Cells[1].Paragraphs.First().AppendLine(persona.Nif);
            DatosDNI.Rows[3].Cells[1].Paragraphs.First().AppendLine(persona.FechaDeNacimiento.ToString());
            DatosDNI.Rows[4].Cells[1].Paragraphs.First().AppendLine(persona.Nacionalidad);
            DatosDNI.Rows[5].Cells[1].Paragraphs.First().AppendLine(persona.DireccionPostal);

            // Insertando en la tabla de otros datos
            OtrosDatos.Rows[1].Cells[0].Paragraphs.First().AppendLine("Telefonos");
            OtrosDatos.Rows[1].Cells[1].Paragraphs.First().AppendLine(persona.Telefono);
            OtrosDatos.Rows[2].Cells[0].Paragraphs.First().AppendLine("Dado de Alta");
            OtrosDatos.Rows[2].Cells[1].Paragraphs.First().AppendLine(persona.CreatedAt.ToShortDateString());
            OtrosDatos.Rows[3].Cells[0].Paragraphs.First().AppendLine("Emails");
            OtrosDatos.Rows[3].Cells[1].Paragraphs.First().AppendLine(persona.Email);
            OtrosDatos.Rows[4].Cells[0].Paragraphs.First().AppendLine("Facebook");
            OtrosDatos.Rows[4].Cells[1].Paragraphs.First().AppendLine(persona.Facebook);
            OtrosDatos.Rows[5].Cells[0].Paragraphs.First().AppendLine("LinkedIn");
            OtrosDatos.Rows[5].Cells[1].Paragraphs.First().AppendLine(persona.LinkedIn);
            OtrosDatos.Rows[6].Cells[0].Paragraphs.First().AppendLine("Twitter");
            OtrosDatos.Rows[6].Cells[1].Paragraphs.First().AppendLine(persona.Twitter);

            document.InsertParagraph();
            // Save this document to disk.
            if (!_FileInUse(fileName))
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
        public void ExportarAsistente(Asistente ats,string fileName)
        {
            var destinyPath = GeneratePath(fileName);
            DocX document = DocX.Create(destinyPath);

            string curFile = destinyPath;
            bool isFileInUse;

            isFileInUse = _FileInUse(destinyPath);

            // Insertar Parrafo con el titulo de la tabla que se mostrará a continuación
            Paragraph title = document.InsertParagraph().Append("Información del Asistente").
                FontSize(20).Font(new FontFamily("Times New Roman"));
            title.Alignment = Alignment.center;

            // Insert a Paragraph into this document.
            Paragraph Parrafo_TablaAsistente = document.InsertParagraph();

            //Header(document);

            // Tablas de Datos
            Table TablaAsistente = document.AddTable(5, 3);
            TablaAsistente.Design = TableDesign.MediumList2;
            Parrafo_TablaAsistente.InsertTableBeforeSelf(TablaAsistente);
            Parrafo_TablaAsistente.AppendLine();

            Paragraph title2 = document.InsertParagraph().Append("Próximas Citas").
                FontSize(20).Font(new FontFamily("Times New Roman"));
            title2.Alignment = Alignment.center;
            // Calculo de número de citas pasadas.
            _Now = DateTime.Now.Date;
            CitasFuturas.Clear();
            CitasFuturas.AddRange(ats.Citas.Where(c => c.Fecha >= _Now));
            CitasPasadas.Clear();
            CitasPasadas.AddRange(ats.Citas.Where(c => c.Fecha < _Now));
            int NumFilasFuturas = CitasFuturas.Count;
            int NumFilasPasadas = CitasPasadas.Count;
            if (NumFilasFuturas == 0) NumFilasFuturas = 1;
            else NumFilasFuturas++;
            if (NumFilasPasadas == 0) NumFilasPasadas = 1;
            else NumFilasPasadas++;
            Paragraph P_TablaProximasCitas = document.InsertParagraph();
            Table TablaProximasCitas = document.AddTable(NumFilasFuturas, 4); // Info Contenida en el DNI
            TablaProximasCitas.Design = TableDesign.MediumList2;
            P_TablaProximasCitas.InsertTableBeforeSelf(TablaProximasCitas);

            Paragraph title3 = document.InsertParagraph().Append("Citas Pasadas").
               FontSize(20).Font(new FontFamily("Times New Roman"));
            title3.Alignment = Alignment.center;

            Paragraph P_TablaPasadas = document.InsertParagraph();
            Table TablaPasadas = document.AddTable(NumFilasPasadas, 4); // Info Contenida en el DNI
            TablaPasadas.Design = TableDesign.MediumList2;
            P_TablaPasadas.InsertTableBeforeSelf(TablaPasadas);

            TablaAsistente.AutoFit = AutoFit.ColumnWidth;
            TablaProximasCitas.AutoFit = AutoFit.ColumnWidth;
            TablaPasadas.AutoFit = AutoFit.ColumnWidth;
            TablaAsistente.SetColumnWidth(0, 2500);
            TablaAsistente.SetColumnWidth(1, 7000);
            TablaProximasCitas.SetColumnWidth(0, 3000);
            TablaProximasCitas.SetColumnWidth(1, 2000);
            TablaProximasCitas.SetColumnWidth(2, 2000);
            TablaProximasCitas.SetColumnWidth(3, 2000);
            TablaPasadas.SetColumnWidth(0, 3000);
            TablaPasadas.SetColumnWidth(1, 2000);
            TablaPasadas.SetColumnWidth(2, 2000);
            TablaPasadas.SetColumnWidth(3, 2000);

            Picture NewPic = null;
            if (ats.Imagen != null)
            {
                MemoryStream stream = new MemoryStream(ats.Imagen);
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
                NewPic = img.CreatePicture(newHeight, newWidth);
            }
            if (NewPic != null)
            {
                TablaAsistente.Rows[1].Cells[0].Paragraphs.First().AppendPicture(NewPic);
            }

            TablaAsistente.Rows[1].Cells[0].VerticalAlignment = Novacode.VerticalAlignment.Center;
            TablaAsistente.MergeCellsInColumn(0, 1, 4);
            TablaAsistente.Rows[1].Cells[1].Paragraphs.First().AppendLine(ats.Nombre);
            TablaAsistente.Rows[2].Cells[1].Paragraphs.First().AppendLine(ats.Apellidos);
            TablaAsistente.Rows[3].Cells[1].Paragraphs.First().AppendLine(ats.Nif);
            TablaAsistente.Rows[4].Cells[1].Paragraphs.First().AppendLine(ats.TelefonoMovil);

            // Insertando en la tabla Citas Futuras
            int pos = 1;
            TablaProximasCitas.Rows[0].Cells[0].Paragraphs.First().AppendLine("Nombre");
            TablaProximasCitas.Rows[0].Cells[1].Paragraphs.First().AppendLine("Fecha");
            TablaProximasCitas.Rows[0].Cells[2].Paragraphs.First().AppendLine("Hora");
            TablaProximasCitas.Rows[0].Cells[3].Paragraphs.First().AppendLine("Sala");
            foreach (var cit in CitasFuturas)
            {
                TablaProximasCitas.Rows[pos].Cells[0].Paragraphs.First().AppendLine(cit.Persona.Nombre);
                TablaProximasCitas.Rows[pos].Cells[1].Paragraphs.First().AppendLine((cit.Fecha.Date).ToString());
                TablaProximasCitas.Rows[pos].Cells[2].Paragraphs.First().AppendLine(string.Concat( string.Concat(cit.Hora.ToString(),':'),cit.Minutos.ToString()));
                TablaProximasCitas.Rows[pos].Cells[3].Paragraphs.First().AppendLine(cit.Sala);
                pos++;
            }
            pos = 1;
            TablaPasadas.Rows[0].Cells[0].Paragraphs.First().AppendLine("Nombre");
            TablaPasadas.Rows[0].Cells[1].Paragraphs.First().AppendLine("Fecha");
            TablaPasadas.Rows[0].Cells[2].Paragraphs.First().AppendLine("Hora");
            TablaPasadas.Rows[0].Cells[3].Paragraphs.First().AppendLine("Sala");
            foreach (var cit in CitasPasadas)
            {
                TablaPasadas.Rows[pos].Cells[0].Paragraphs.First().AppendLine(cit.Persona.Nombre);
                TablaPasadas.Rows[pos].Cells[1].Paragraphs.First().AppendLine((cit.Fecha.Date).ToString());
                TablaPasadas.Rows[pos].Cells[2].Paragraphs.First().AppendLine(string.Concat(cit.Hora.ToString(), cit.Minutos.ToString()));
                TablaPasadas.Rows[pos].Cells[3].Paragraphs.First().AppendLine(cit.Sala);
                pos++;
            }
            document.InsertParagraph();
            // Save this document to disk.
            if (!_FileInUse(destinyPath))
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
        public void ExportarTodasCitas(List<Cita> citas)
        {
            var destinyPath = GeneratePath("ListaCitas");
            DocX document = DocX.Create(destinyPath);

            string curFile = destinyPath;
            bool isFileInUse;

            isFileInUse = _FileInUse(destinyPath);

            //Header(document);

            Paragraph title2 = document.InsertParagraph().Append("Próximas Citas").
                 FontSize(20).Font(new FontFamily("Times New Roman"));
            title2.Alignment = Alignment.center;
            // Calculo de número de citas pasadas.
            _Now = DateTime.Now.Date;
            CitasFuturas.Clear();
            CitasFuturas.AddRange(citas.Where(c => c.Fecha >= _Now));
            CitasPasadas.Clear();
            CitasPasadas.AddRange(citas.Where(c => c.Fecha < _Now));
            int NumFilasFuturas = CitasFuturas.Count;
            int NumFilasPasadas = CitasPasadas.Count;
            if (NumFilasFuturas == 0) NumFilasFuturas = 1;
            else NumFilasFuturas++;
            if (NumFilasPasadas == 0) NumFilasPasadas = 1;
            else NumFilasPasadas++;
            Paragraph P_TablaProximasCitas = document.InsertParagraph();
            Table TablaProximasCitas = document.AddTable(NumFilasFuturas, 4); // Info Contenida en el DNI
            TablaProximasCitas.Design = TableDesign.MediumList2;
            P_TablaProximasCitas.InsertTableBeforeSelf(TablaProximasCitas);

            Paragraph title3 = document.InsertParagraph().Append("Citas Pasadas").
               FontSize(20).Font(new FontFamily("Times New Roman"));
            title3.Alignment = Alignment.center;

            Paragraph P_TablaPasadas = document.InsertParagraph();
            Table TablaPasadas = document.AddTable(NumFilasPasadas, 4); // Info Contenida en el DNI
            TablaPasadas.Design = TableDesign.MediumList2;
            P_TablaPasadas.InsertTableBeforeSelf(TablaPasadas);
            
            TablaProximasCitas.AutoFit = AutoFit.ColumnWidth;
            TablaPasadas.AutoFit = AutoFit.ColumnWidth;
            TablaProximasCitas.SetColumnWidth(0, 3000);
            TablaProximasCitas.SetColumnWidth(1, 2000);
            TablaProximasCitas.SetColumnWidth(2, 2000);
            TablaProximasCitas.SetColumnWidth(3, 2000);
            TablaPasadas.SetColumnWidth(0, 3000);
            TablaPasadas.SetColumnWidth(1, 2000);
            TablaPasadas.SetColumnWidth(2, 2000);
            TablaPasadas.SetColumnWidth(3, 2000);

            // Insertando en la tabla Citas Futuras
            int pos = 1;
            TablaProximasCitas.Rows[0].Cells[0].Paragraphs.First().AppendLine("Nombre");
            TablaProximasCitas.Rows[0].Cells[1].Paragraphs.First().AppendLine("Fecha");
            TablaProximasCitas.Rows[0].Cells[2].Paragraphs.First().AppendLine("Hora");
            TablaProximasCitas.Rows[0].Cells[3].Paragraphs.First().AppendLine("Sala");
            foreach (var cit in CitasFuturas)
            {
                TablaProximasCitas.Rows[pos].Cells[0].Paragraphs.First().AppendLine(cit.Persona.Nombre);
                TablaProximasCitas.Rows[pos].Cells[1].Paragraphs.First().AppendLine((cit.Fecha.Date).ToString());
                TablaProximasCitas.Rows[pos].Cells[2].Paragraphs.First().AppendLine(string.Concat(string.Concat(cit.Hora.ToString(), ':'), cit.Minutos.ToString()));
                TablaProximasCitas.Rows[pos].Cells[3].Paragraphs.First().AppendLine(cit.Sala);
                pos++;
            }
            pos = 1;
            TablaPasadas.Rows[0].Cells[0].Paragraphs.First().AppendLine("Nombre");
            TablaPasadas.Rows[0].Cells[1].Paragraphs.First().AppendLine("Fecha");
            TablaPasadas.Rows[0].Cells[2].Paragraphs.First().AppendLine("Hora");
            TablaPasadas.Rows[0].Cells[3].Paragraphs.First().AppendLine("Sala");
            foreach (var cit in CitasPasadas)
            {
                TablaPasadas.Rows[pos].Cells[0].Paragraphs.First().AppendLine(cit.Persona.Nombre);
                TablaPasadas.Rows[pos].Cells[1].Paragraphs.First().AppendLine((cit.Fecha.Date).ToString());
                TablaPasadas.Rows[pos].Cells[2].Paragraphs.First().AppendLine(string.Concat(cit.Hora.ToString(), cit.Minutos.ToString()));
                TablaPasadas.Rows[pos].Cells[3].Paragraphs.First().AppendLine(cit.Sala);
                pos++;
            }
            document.InsertParagraph();
            // Save this document to disk.
            if (!_FileInUse(destinyPath))
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


    }
}
