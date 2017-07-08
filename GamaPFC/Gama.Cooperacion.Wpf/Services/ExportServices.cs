using Gama.Common.Eventos;
using Gama.Cooperacion.Business;
using Novacode;
using Prism.Events;
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

namespace Gama.Cooperacion.Wpf.Services
{
    public class ExportService
    {
        private static string _DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private EventAggregator _EventAggregator;
        //+ @"\HelloWorld.docx";

        public ExportService(EventAggregator EventAggregator)
        {
            _EventAggregator = EventAggregator;
        }
        private string GeneratePath(string fileName)
        {
            if (!fileName.Contains(".doc"))
                return _DesktopPath + String.Format(@"\{0}.docx", fileName);
            else
                return _DesktopPath + String.Format(@"\{0}", fileName);
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
        public void ExportarActividad(Actividad act,string fileName)
        {

            var destinyPath = GeneratePath(fileName.Substring(0,10));
            DocX document = DocX.Create(destinyPath);

            string curFile = destinyPath;
            bool isFileInUse;

            isFileInUse = FileInUse(destinyPath);



            // Insertar Parrafo con el titulo de la tabla que se mostrará a continuación
            Paragraph title = document.InsertParagraph().Append(act.Titulo).
                FontSize(20).Font(new FontFamily("Times New Roman"));
            title.Alignment = Alignment.center;

            // Insert a Paragraph into this document.
            Paragraph Parrafo_TablaInfoActividad = document.InsertParagraph();

            Header(document);

            // Tablas de Datos
            Table TablaInfoActividad = document.AddTable(6, 2);
            TablaInfoActividad.Design = TableDesign.MediumList2;
            Parrafo_TablaInfoActividad.InsertTableBeforeSelf(TablaInfoActividad);
            Parrafo_TablaInfoActividad.AppendLine();

            Paragraph title2 = document.InsertParagraph().Append("Cooperantes").
                FontSize(20).Font(new FontFamily("Times New Roman"));
            title2.Alignment = Alignment.center;

            int TamFilas = act.Cooperantes.Count();
            if (TamFilas == 0) TamFilas = 1;
            else TamFilas += 1;
            Paragraph Parrafo_TablaCooperantes = document.InsertParagraph();
            Table TablaCooperantes = document.AddTable(TamFilas, 2); // Calcular los valoes de fila y columnas
            TablaCooperantes.Design = TableDesign.MediumList2;
            Parrafo_TablaCooperantes.InsertTableBeforeSelf(TablaCooperantes);

            Paragraph title3 = document.InsertParagraph().Append("Tareas de la Actividad").
                FontSize(20).Font(new FontFamily("Times New Roman"));
            title3.Alignment = Alignment.center;

            int TamFilasTareas = act.Tareas.Count();
            if (TamFilasTareas == 0) TamFilasTareas = 1;
            else TamFilasTareas += 1;
            Paragraph Parrafo_TablaTareas = document.InsertParagraph();
            Table TablaTareas = document.AddTable(TamFilas, 3); // Calcular los valoes de fila y columnas
            TablaTareas.Design = TableDesign.MediumList2;
            Parrafo_TablaTareas.InsertTableBeforeSelf(TablaTareas);

            TablaInfoActividad.AutoFit = AutoFit.ColumnWidth;
            TablaCooperantes.AutoFit = AutoFit.ColumnWidth;
            TablaTareas.AutoFit = AutoFit.ColumnWidth;
            TablaInfoActividad.SetColumnWidth(0, 2500);
            TablaInfoActividad.SetColumnWidth(1, 7000);
            TablaCooperantes.SetColumnWidth(0, 2500);
            TablaCooperantes.SetColumnWidth(1, 7000);
            TablaCooperantes.SetColumnWidth(0, 2500);
            TablaCooperantes.SetColumnWidth(1, 7000);
            TablaTareas.SetColumnWidth(0, 3000);
            TablaTareas.SetColumnWidth(1, 2500);
            TablaTareas.SetColumnWidth(2, 2500);

            //Insertando los valores en las celda de TablaInfoActividad
            TablaInfoActividad.Rows[1].Cells[0].Paragraphs.First().AppendLine("Descripción");
            TablaInfoActividad.Rows[1].Cells[1].Paragraphs.First().AppendLine(act.Descripcion);
            TablaInfoActividad.Rows[2].Cells[0].Paragraphs.First().AppendLine("Estado");
            TablaInfoActividad.Rows[2].Cells[1].Paragraphs.First().AppendLine(act.Estado.ToString());
            TablaInfoActividad.Rows[3].Cells[0].Paragraphs.First().AppendLine("Fecha Inicio");
            TablaInfoActividad.Rows[3].Cells[1].Paragraphs.First().AppendLine(act.FechaDeInicio.ToString());
            TablaInfoActividad.Rows[4].Cells[0].Paragraphs.First().AppendLine("Fecha Final");
            TablaInfoActividad.Rows[4].Cells[1].Paragraphs.First().AppendLine(act.FechaDeInicio.ToString());
            TablaInfoActividad.Rows[5].Cells[0].Paragraphs.First().AppendLine("Coordinador");
            TablaInfoActividad.Rows[5].Cells[1].Paragraphs.First().AppendLine(act.Coordinador.Nombre);

            // Insertando Nombres de los Cooperantes de la Actividad.
            int pos = 1;
            foreach(var coop in act.Cooperantes)
            {
                TablaCooperantes.Rows[pos].Cells[0].Paragraphs.First().AppendLine("Cooperante");
                TablaCooperantes.Rows[pos].Cells[1].Paragraphs.First().AppendLine(coop.Nombre);
                pos++;
            }
            pos = 1;
            TablaTareas.Rows[0].Cells[0].Paragraphs.First().AppendLine("Descripción");
            TablaTareas.Rows[0].Cells[1].Paragraphs.First().AppendLine("Fecha Final");
            TablaTareas.Rows[0].Cells[2].Paragraphs.First().AppendLine("Responsable");
            foreach (var tar in act.Tareas)
            {
                TablaTareas.Rows[pos].Cells[0].Paragraphs.First().AppendLine(tar.Descripcion);
                TablaTareas.Rows[pos].Cells[1].Paragraphs.First().AppendLine(tar.FechaDeFinalizacion.ToString());
                TablaTareas.Rows[pos].Cells[2].Paragraphs.First().AppendLine(tar.Responsable.Nombre);
                pos++;
            }
            pos = 1;
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
        public void ExportarTodasActividades(List<Actividad> actividades)
        {
            var destinyPath = GeneratePath("ListaActividades");
            DocX document = DocX.Create(destinyPath);

            string curFile = destinyPath;
            bool isFileInUse;

            isFileInUse = FileInUse(destinyPath);

            // Insertar Parrafo con el titulo de la tabla que se mostrará a continuación
            Paragraph title = document.InsertParagraph().Append("Listado de Actividades").
                FontSize(20).Font(new FontFamily("Times New Roman"));
            title.Alignment = Alignment.center;

            // Insert a Paragraph into this document.
            Paragraph Parrafo_TablaInfoActividad = document.InsertParagraph();

            Header(document);

            int TamFilas = actividades.Count();
            if (TamFilas == 0) TamFilas = 1;
            else TamFilas += 1;
            Paragraph Parrafo_TablaActividades = document.InsertParagraph();
            Table TablaActividades = document.AddTable(TamFilas, 6); // Calcular los valoes de fila y columnas
            TablaActividades.Alignment = Alignment.center;
            TablaActividades.Design = TableDesign.MediumList2;
            Parrafo_TablaActividades.InsertTableBeforeSelf(TablaActividades);

            TablaActividades.AutoFit = AutoFit.ColumnWidth;
            TablaActividades.SetColumnWidth(0, 1000);
            TablaActividades.SetColumnWidth(1, 3000);
            TablaActividades.SetColumnWidth(2, 1600);
            TablaActividades.SetColumnWidth(3, 1500);
            TablaActividades.SetColumnWidth(4, 2500);
            TablaActividades.SetColumnWidth(5, 2000);

            //Insertando los valores en las celda de TablaInfoActividad
            TablaActividades.Rows[0].Cells[0].Paragraphs.First().AppendLine("ID");
            TablaActividades.Rows[0].Cells[1].Paragraphs.First().AppendLine("Título");
            TablaActividades.Rows[0].Cells[2].Paragraphs.First().AppendLine("Estado");
            TablaActividades.Rows[0].Cells[3].Paragraphs.First().AppendLine("Inicio");
            TablaActividades.Rows[0].Cells[4].Paragraphs.First().AppendLine("Final");
            TablaActividades.Rows[0].Cells[5].Paragraphs.First().AppendLine("Coordinador");

            int pos = 1;
            int tamTitulo = 0;
            foreach (var act in actividades)
            {
                if (act.Titulo.Length >= 15) tamTitulo = 15;
                else tamTitulo = act.Titulo.Length;
                TablaActividades.Rows[pos].Cells[0].Paragraphs.First().AppendLine(act.Id.ToString());
                TablaActividades.Rows[pos].Cells[1].Paragraphs.First().AppendLine(act.Titulo.Substring(0, tamTitulo));
                TablaActividades.Rows[pos].Cells[2].Paragraphs.First().AppendLine(act.Estado.ToString());
                TablaActividades.Rows[pos].Cells[3].Paragraphs.First().AppendLine(act.FechaDeInicio.Date.ToString());
                TablaActividades.Rows[pos].Cells[4].Paragraphs.First().AppendLine(act.FechaDeFin.Date.ToString());
                TablaActividades.Rows[pos].Cells[5].Paragraphs.First().AppendLine(act.Coordinador.Nombre);
                pos++;
            }
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
        public void ExportarCooperante(Cooperante coop, string fileName)
        {
            var destinyPath = GeneratePath(fileName);
            DocX document = DocX.Create(destinyPath);

            string curFile = destinyPath;
            bool isFileInUse;

            isFileInUse = FileInUse(destinyPath);

            // Insertar Parrafo con el titulo de la tabla que se mostrará a continuación
            Paragraph title = document.InsertParagraph().Append("Información del Cooperante").
                FontSize(20).Font(new FontFamily("Times New Roman"));
            title.Alignment = Alignment.center;

            // Insert a Paragraph into this document.
            Paragraph Parrafo_TablaCooperante = document.InsertParagraph();

            Header(document);

            // Tablas de Datos
            Table TablaCooperante = document.AddTable(6, 3);
            TablaCooperante.Design = TableDesign.MediumList2;
            Parrafo_TablaCooperante.InsertTableBeforeSelf(TablaCooperante);
            Parrafo_TablaCooperante.AppendLine();

            Paragraph title2 = document.InsertParagraph().Append("Otros Datos").
                FontSize(20).Font(new FontFamily("Times New Roman"));
            title2.Alignment = Alignment.center;

            Paragraph P_TablaOtrosDatos = document.InsertParagraph();
            Table OtrosDatos = document.AddTable(9, 2); // Info Contenida en el DNI
            OtrosDatos.Design = TableDesign.MediumList2;
            P_TablaOtrosDatos.InsertTableBeforeSelf(OtrosDatos);

            TablaCooperante.AutoFit = AutoFit.ColumnWidth;
            OtrosDatos.AutoFit = AutoFit.ColumnWidth;
            OtrosDatos.SetColumnWidth(0, 2500);
            OtrosDatos.SetColumnWidth(1, 7000);
            TablaCooperante.SetColumnWidth(0, 2500);
            TablaCooperante.SetColumnWidth(1, 7000);

            Picture NewPic=null;
            if (coop.Foto  != null)
            {
                MemoryStream stream = new MemoryStream(coop.Foto);
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
                TablaCooperante.Rows[1].Cells[0].Paragraphs.First().AppendPicture(NewPic);
            }
            
            TablaCooperante.Rows[1].Cells[0].VerticalAlignment = Novacode.VerticalAlignment.Center;
            TablaCooperante.MergeCellsInColumn(0, 1, 5);
            TablaCooperante.Rows[1].Cells[1].Paragraphs.First().AppendLine(coop.Nombre);
            TablaCooperante.Rows[2].Cells[1].Paragraphs.First().AppendLine(coop.Apellido);
            TablaCooperante.Rows[3].Cells[1].Paragraphs.First().AppendLine(coop.Dni);
            TablaCooperante.Rows[4].Cells[1].Paragraphs.First().AppendLine(coop.FechaDeNacimiento.ToString());
            TablaCooperante.Rows[5].Cells[1].Paragraphs.First().AppendLine(coop.TelefonoMovil);

            // Insertando en la tabla de otros datos
            OtrosDatos.Rows[1].Cells[0].Paragraphs.First().AppendLine("Teléfono Fijo");
            OtrosDatos.Rows[1].Cells[1].Paragraphs.First().AppendLine(coop.telefono);
            OtrosDatos.Rows[2].Cells[0].Paragraphs.First().AppendLine("Teléfono Alternativo");
            OtrosDatos.Rows[2].Cells[1].Paragraphs.First().AppendLine(coop.TelefonoAlternativo);
            OtrosDatos.Rows[3].Cells[0].Paragraphs.First().AppendLine("Email");
            OtrosDatos.Rows[3].Cells[1].Paragraphs.First().AppendLine(coop.Email);
            OtrosDatos.Rows[4].Cells[0].Paragraphs.First().AppendLine("Email Alternativo");
            OtrosDatos.Rows[4].Cells[1].Paragraphs.First().AppendLine(coop.EmailAlternativo);
            OtrosDatos.Rows[5].Cells[0].Paragraphs.First().AppendLine("Provincia");
            OtrosDatos.Rows[5].Cells[1].Paragraphs.First().AppendLine(coop.Provincia);
            OtrosDatos.Rows[6].Cells[0].Paragraphs.First().AppendLine("Municipio - CP");
            OtrosDatos.Rows[6].Cells[1].Paragraphs.First().AppendLine(string.Concat(coop.Municipio," - " + coop.CP));
            OtrosDatos.Rows[7].Cells[0].Paragraphs.First().AppendLine("Localidad");
            OtrosDatos.Rows[7].Cells[1].Paragraphs.First().AppendLine(coop.Localidad);
            OtrosDatos.Rows[8].Cells[0].Paragraphs.First().AppendLine("Dirección");
            OtrosDatos.Rows[8].Cells[1].Paragraphs.First().AppendLine(coop.Calle+", Número "+coop.Numero+", Portal "+coop.Portal+", Piso "+coop.Piso+", Puerta "+coop.Puerta);
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
        public void ExportarTodosCooperantes(List<Cooperante> cooperantes)
        {

        }




        //    // Insertando en la tabla de otros datos
        //    OtrosDatos.Rows[1].Cells[0].Paragraphs.First().AppendLine("Telefonos");
        //    OtrosDatos.Rows[1].Cells[1].Paragraphs.First().AppendLine(socio.Telefono);
        //    OtrosDatos.Rows[2].Cells[0].Paragraphs.First().AppendLine("Dado de Alta");
        //    OtrosDatos.Rows[2].Cells[1].Paragraphs.First().AppendLine(socio.CreatedAt.ToShortDateString());
        //    OtrosDatos.Rows[3].Cells[0].Paragraphs.First().AppendLine("Emails");
        //    OtrosDatos.Rows[3].Cells[1].Paragraphs.First().AppendLine(socio.Email);
        //    OtrosDatos.Rows[4].Cells[0].Paragraphs.First().AppendLine("Facebook");
        //    OtrosDatos.Rows[4].Cells[1].Paragraphs.First().AppendLine(socio.Facebook);
        //    OtrosDatos.Rows[5].Cells[0].Paragraphs.First().AppendLine("LinkedIn");
        //    OtrosDatos.Rows[5].Cells[1].Paragraphs.First().AppendLine(socio.LinkedIn);
        //    OtrosDatos.Rows[6].Cells[0].Paragraphs.First().AppendLine("Twitter");
        //    OtrosDatos.Rows[6].Cells[1].Paragraphs.First().AppendLine(socio.Twitter);
    }

        //public void ExportarSocio(Cooperante socio, string fileName)
        //{
        //    ////Calculo contable---------------
        //    //double TotalPagado = 0;
        //    //double TotalSinPagar = 0;
        //    //double TotalImpagos = 0;
        //    //foreach (var Recibo in socio.PeriodosDeAlta)
        //    //{
        //    //    foreach (var cuot in Recibo.Cuotas)
        //    //    {
        //    //        if (cuot.EstaPagado) TotalPagado = TotalPagado + cuot.CantidadTotal;
        //    //        else
        //    //        {
        //    //            //int SemanaFin = DateTime.Compare(_FechaTest, ActividadSeleccionada.FechaDeFin.AddDays(-7));
        //    //            int FueraPlazo = DateTime.Compare(cuot.Fecha.AddMonths(_MesesParaMoroso), DateTime.Now.Date);
        //    //            if (FueraPlazo == -1) TotalImpagos = TotalImpagos + (cuot.CantidadTotal - cuot.CantidadPagada);
        //    //            else TotalSinPagar = TotalSinPagar + (cuot.CantidadTotal - cuot.CantidadPagada);
        //    //        }
        //    //        //else TotalSinPagar = TotalSinPagar + cuot.CantidadPagada;
        //    //    }
        //    //}
        //    ////Fin Calculo contable-----------
        //    var destinyPath = GeneratePath(fileName);
        //    DocX document = DocX.Create(destinyPath);

        //    string curFile = destinyPath;
        //    bool isFileInUse;

        //    isFileInUse = FileInUse(destinyPath);


        //    // Insertar Parrafo con el titulo de la tabla que se mostrará a continuación
        //    Paragraph title = document.InsertParagraph().Append("Información de Socio").
        //        FontSize(20).Font(new FontFamily("Times New Roman"));
        //    title.Alignment = Alignment.center;

        //    // Insert a Paragraph into this document.
        //    Paragraph P_TablaDatosDNI = document.InsertParagraph();

        //    Header(document);

        //    // Tablas de Datos
        //    Table DatosDNI = document.AddTable(6, 3); // Info Contenida en el DNI
        //    // Diseño de las Tablas
        //    DatosDNI.Design = TableDesign.MediumList2Accent4;
        //    P_TablaDatosDNI.InsertTableBeforeSelf(DatosDNI);
        //    P_TablaDatosDNI.AppendLine();

        //    Paragraph title2 = document.InsertParagraph().Append("Otros Datos").
        //        FontSize(20).Font(new FontFamily("Times New Roman"));
        //    title2.Alignment = Alignment.center;

        //    Paragraph P_TablaOtrosDatos = document.InsertParagraph();
        //    Table OtrosDatos = document.AddTable(7, 2); // Info Contenida en el DNI
        //    OtrosDatos.Design = TableDesign.MediumList2Accent4;
        //    P_TablaOtrosDatos.InsertTableBeforeSelf(OtrosDatos);

        //    Paragraph title3 = document.InsertParagraph().Append("Cuotas").
        //        FontSize(20).Font(new FontFamily("Times New Roman"));
        //    title3.Alignment = Alignment.center;
        //    Paragraph P_TablaCuotas = document.InsertParagraph();
        //    Table Cuotas = document.AddTable(4, 2); // Info Contenida en el DNI
        //    Cuotas.Design = TableDesign.MediumList2Accent5;
        //    P_TablaCuotas.InsertTableBeforeSelf(Cuotas);

        //    DatosDNI.AutoFit = AutoFit.ColumnWidth;
        //    OtrosDatos.AutoFit = AutoFit.ColumnWidth;
        //    Cuotas.AutoFit = AutoFit.ColumnWidth;
        //    DatosDNI.SetColumnWidth(0, 2500);
        //    DatosDNI.SetColumnWidth(1, 7000);
        //    OtrosDatos.SetColumnWidth(0, 2500);
        //    OtrosDatos.SetColumnWidth(1, 7000);
        //    Cuotas.SetColumnWidth(0, 2500);
        //    Cuotas.SetColumnWidth(1, 7000);

        //    //if (!string.IsNullOrEmpty(socio.AvatarPath))
        //    //{
        //    //    string avatar = socio.AvatarPath;
        //    //    //MemoryStream ms = new MemoryStream((byte)avatar);
        //    //    Novacode.Image image = document.AddImage(socio.AvatarPath);

        //    //    // Create a picture (A custom view of an Image).
        //    //    Picture picture = image.CreatePicture();
        //    //    picture.Rotation = 10;
        //    //    picture.SetPictureShape(BasicShapes.cube);
        //    //}


        //    MemoryStream stream = new MemoryStream(socio.Imagen);
        //    Novacode.Image img = document.AddImage(stream); // Create image.
        //    Picture pic1 = img.CreatePicture();     // Create picture.
        //    //pic1.Height = 179;
        //    //pic1.Width = 152;
        //    // Redimensionamos la imagen para que cuadre en la tabla manteniendo sus proporciones
        //    var ratioX = (double)152 / pic1.Width;
        //    var ratioY = (double)179 / pic1.Height;
        //    double ratio;
        //    if (ratioX < ratioY) { ratio = Math.Min(ratioX, ratioY); }
        //    else { ratio = Math.Max(ratioX, ratioY); }

        //    var newWidth = (int)(pic1.Width * ratio);
        //    var newHeight = (int)(pic1.Height * ratio);
        //    Picture NewPic = img.CreatePicture(newHeight, newWidth);

        //    //BitmapImage image = new BitmapImage();
        //    //image.BeginInit();
        //    //image.StreamSource = stream;
        //    //image.EndInit();
        //    //MemoryStream ms = new MemoryStream();
        //    //System.Drawing.Image myImg = System.Drawing.Image.FromFile(@"Images/Silueta.png");

        //    //myImg.Save(ms, myImg.RawFormat);  // Save your picture in a memory stream.
        //    //ms.Seek(0, SeekOrigin.Begin);

        //    //Novacode.Image img = document.AddImage(ms); // Create image.
        //    //Picture pic1 = img.CreatePicture();     // Create picture.

        //    //Insertando los valores en las celda de TablaDatosDNI
        //    DatosDNI.Rows[1].Cells[0].Paragraphs.First().AppendPicture(NewPic);
        //    DatosDNI.Rows[1].Cells[0].VerticalAlignment = Novacode.VerticalAlignment.Center;
        //    DatosDNI.MergeCellsInColumn(0, 1, 5);
        //    DatosDNI.Rows[1].Cells[1].Paragraphs.First().AppendLine(socio.Nombre);
        //    DatosDNI.Rows[2].Cells[1].Paragraphs.First().AppendLine(socio.Nif);
        //    DatosDNI.Rows[3].Cells[1].Paragraphs.First().AppendLine(socio.FechaDeNacimiento.ToString());
        //    DatosDNI.Rows[4].Cells[1].Paragraphs.First().AppendLine(socio.Nacionalidad);
        //    DatosDNI.Rows[5].Cells[1].Paragraphs.First().AppendLine(socio.DireccionPostal);

        //    // Insertando en la tabla de otros datos
        //    OtrosDatos.Rows[1].Cells[0].Paragraphs.First().AppendLine("Telefonos");
        //    OtrosDatos.Rows[1].Cells[1].Paragraphs.First().AppendLine(socio.Telefono);
        //    OtrosDatos.Rows[2].Cells[0].Paragraphs.First().AppendLine("Dado de Alta");
        //    OtrosDatos.Rows[2].Cells[1].Paragraphs.First().AppendLine(socio.CreatedAt.ToShortDateString());
        //    OtrosDatos.Rows[3].Cells[0].Paragraphs.First().AppendLine("Emails");
        //    OtrosDatos.Rows[3].Cells[1].Paragraphs.First().AppendLine(socio.Email);
        //    OtrosDatos.Rows[4].Cells[0].Paragraphs.First().AppendLine("Facebook");
        //    OtrosDatos.Rows[4].Cells[1].Paragraphs.First().AppendLine(socio.Facebook);
        //    OtrosDatos.Rows[5].Cells[0].Paragraphs.First().AppendLine("LinkedIn");
        //    OtrosDatos.Rows[5].Cells[1].Paragraphs.First().AppendLine(socio.LinkedIn);
        //    OtrosDatos.Rows[6].Cells[0].Paragraphs.First().AppendLine("Twitter");
        //    OtrosDatos.Rows[6].Cells[1].Paragraphs.First().AppendLine(socio.Twitter);

        //    Cuotas.Rows[1].Cells[0].Paragraphs.First().AppendLine("Total Pagado");
        //    Cuotas.Rows[1].Cells[1].Paragraphs.First().AppendLine(TotalPagado.ToString());
        //    Cuotas.Rows[2].Cells[0].Paragraphs.First().AppendLine("Total Pendiente");
        //    Cuotas.Rows[2].Cells[1].Paragraphs.First().AppendLine(TotalSinPagar.ToString());
        //    Cuotas.Rows[3].Cells[0].Paragraphs.First().AppendLine("Total Impagos");
        //    Cuotas.Rows[3].Cells[1].Paragraphs.First().AppendLine(TotalImpagos.ToString());
        //    document.InsertParagraph();
        //    // Save this document to disk.
        //    if (!FileInUse(destinyPath))
        //    {
        //        document.Save();
        //    }
        //    else
        //    {
        //        MessageBox.Show("El fichero está abierto. No se realizaron los cambios");
        //    }
        //    //document.Save();
        //    //Process.Start("WINWORD.EXE", destinyPath);        
        //}





        //public void ExportarSocios(List<Cooperante> AllCooperantes)
        //{
        //    var destinyPath = GeneratePath("ListaSocios");
        //    DocX document = DocX.Create(destinyPath);
        //    string curFile = destinyPath;
        //    bool isFileInUse;
        //    isFileInUse = FileInUse(destinyPath);
        //    // Insertar Parrafo con el titulo de la tabla que se mostrará a continuación
        //    Paragraph title = document.InsertParagraph().Append("Listado Actual de Socios").
        //        FontSize(20).Font(new FontFamily("Times New Roman"));
        //    title.Alignment = Alignment.center;
        //    // --------------- Llamda a Header ---------------
        //    document.AddHeaders();
        //    document.DifferentFirstPage = true;

        //    Header header_first = document.Headers.first;
        //    Header headers = document.Headers.odd; // El resto de headers

        //    Paragraph p0 = header_first.InsertParagraph();
        //    p0.Append("Gamá - Gestión de Socios").FontSize(20).Bold();
        //    p0.Alignment = Alignment.center;
        //    // Insert a Paragraph into this document.
        //    Paragraph P_TablaDatosSocios = document.InsertParagraph();
        //    P_TablaDatosSocios.Alignment = Alignment.center;
        //    //-------------- Fin Llamada a Header-----------------
        //    //-------------- Crear la Tabla-----------------------
        //    // NumFilas = Socios.Cont + Titulos + Sumatoria
        //    // NumColumnas = 6 (Nombre,Dni,Telefono,CantidadPagada,CuotasPorPagar,CuotasImpagadas)
        //    // Tablas de Datos
        //    int NumFilas = AllCooperantes.Count + 2;
        //    int NumColumnas = 6;
        //    Table DatosSocios = document.AddTable(NumFilas, NumColumnas); // Info Contenida en el DNI
        //    DatosSocios.Alignment = Alignment.center;
        //    // Diseño de las Tablas
        //    DatosSocios.Design = TableDesign.MediumShading1;

        //    P_TablaDatosSocios.InsertTableBeforeSelf(DatosSocios);
        //    P_TablaDatosSocios.AppendLine();

        //    DatosSocios.AutoFit = AutoFit.ColumnWidth;
        //    DatosSocios.SetColumnWidth(0, 2000);
        //    DatosSocios.SetColumnWidth(1, 1700);
        //    DatosSocios.SetColumnWidth(2, 1700);
        //    DatosSocios.SetColumnWidth(3, 1700);
        //    DatosSocios.SetColumnWidth(4, 1700);
        //    DatosSocios.SetColumnWidth(5, 1900);

        //    //Insertando los valores en las celda de DatosSocios
        //    DatosSocios.Rows[0].Cells[0].Paragraphs.First().AppendLine("Nombre");
        //    DatosSocios.Rows[0].Cells[1].Paragraphs.First().AppendLine("DNI");
        //    DatosSocios.Rows[0].Cells[2].Paragraphs.First().AppendLine("Teléfono");
        //    DatosSocios.Rows[0].Cells[3].Paragraphs.First().AppendLine("CantidadPagada");
        //    DatosSocios.Rows[0].Cells[4].Paragraphs.First().AppendLine("CuotasPorPagar");
        //    DatosSocios.Rows[0].Cells[5].Paragraphs.First().AppendLine("CuotasImpagadas");

        //    int posCooperante = 0;

        //    //double TotalPagadoSocios = 0;
        //    //double TotalSinPagarSocios = 0;
        //    //double TotalImpagosSocios = 0;
        //    //double TotalPagado = 0;
        //    //double TotalSinPagar = 0;
        //    //double TotalImpagos = 0;
        //    for (int i = 1; i <= (NumFilas - 2); i++)
        //    {
        //        posCooperante = i - 1;
        //        //TotalPagado = 0;
        //        //TotalSinPagar = 0;
        //        //TotalImpagos = 0;
        //        Cooperante CooperanteListado = AllCooperantes[posCooperante];
        //        //foreach (var Recibo in socioLista.PeriodosDeAlta)
        //        //{
        //        //    foreach (var cuot in Recibo.Cuotas)
        //        //    {
        //        //        if (cuot.EstaPagado) TotalPagado = TotalPagado + cuot.CantidadTotal;
        //        //        else
        //        //        {
        //        //            //int SemanaFin = DateTime.Compare(_FechaTest, ActividadSeleccionada.FechaDeFin.AddDays(-7));
        //        //            int FueraPlazo = DateTime.Compare(cuot.Fecha.AddMonths(_MesesParaMoroso), DateTime.Now.Date);
        //        //            if (FueraPlazo == -1) TotalImpagos = TotalImpagos + (cuot.CantidadTotal - cuot.CantidadPagada);
        //        //            else TotalSinPagar = TotalSinPagar + (cuot.CantidadTotal - cuot.CantidadPagada);
        //        //        }
        //        //        //else TotalSinPagar = TotalSinPagar + cuot.CantidadPagada;
        //        //    }
        //        //}
        //        //TotalPagadoSocios += TotalPagado;
        //        //TotalSinPagarSocios += TotalSinPagar;
        //        //TotalImpagosSocios += TotalImpagos;
        //        DatosSocios.Rows[i].Cells[0].Paragraphs.First().AppendLine(socioLista.Nombre);
        //        DatosSocios.Rows[i].Cells[1].Paragraphs.First().AppendLine(socioLista.Nif);
        //        DatosSocios.Rows[i].Cells[2].Paragraphs.First().AppendLine(socioLista.Telefono);
        //        DatosSocios.Rows[i].Cells[3].Paragraphs.First().AppendLine(TotalPagado.ToString());
        //        DatosSocios.Rows[i].Cells[4].Paragraphs.First().AppendLine(TotalSinPagar.ToString());
        //        DatosSocios.Rows[i].Cells[5].Paragraphs.First().AppendLine(TotalImpagos.ToString());
        //    }
        //    // Relleno de la linea final con la sumatoria totasl de las cuotas
        //    DatosSocios.Rows[NumFilas - 1].Cells[1].Paragraphs.First().AppendLine("Cantidad Total");
        //    DatosSocios.Rows[NumFilas - 1].Cells[2].Paragraphs.First().AppendLine(socios.Count.ToString());
        //    DatosSocios.Rows[NumFilas - 1].Cells[3].Paragraphs.First().AppendLine(TotalPagadoSocios.ToString());
        //    DatosSocios.Rows[NumFilas - 1].Cells[4].Paragraphs.First().AppendLine(TotalSinPagarSocios.ToString());
        //    DatosSocios.Rows[NumFilas - 1].Cells[5].Paragraphs.First().AppendLine(TotalImpagosSocios.ToString());

        //    document.InsertParagraph();
        //    // Save this document to disk.
        //    if (!FileInUse(destinyPath))
        //    {
        //        document.Save();
        //    }
        //    else
        //    {
        //        MessageBox.Show("El fichero está abierto. No se realizaron los cambios");
        //    }
        //    //document.Save();
        //    //Process.Start("WINWORD.EXE", destinyPath);  

        //}

    }



