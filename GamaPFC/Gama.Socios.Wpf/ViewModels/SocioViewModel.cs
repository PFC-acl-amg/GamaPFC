using Core;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Wrappers;
using Prism.Commands;
using System.Windows.Input;
using System;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;

namespace Gama.Socios.Wpf.ViewModels
{
    public class SocioViewModel : ViewModelBase
    {
        private bool _EdicionHabilitada;
        private SocioWrapper _Socio;
        private byte[] _imageBytes = null;
        private string _rutaImagen;
        private string _nombreImagen;
        private BitmapImage _imagenSocio;

        public SocioViewModel()
        {
            _EdicionHabilitada = true;
            Socio = new SocioWrapper(new Socio());
            //RutaImagen = new BitmapImage();

            ExaminarAvatarCommand = new DelegateCommand(OnExaminarAvatarCommandExecute);
        }

        public ICommand ExaminarAvatarCommand { get; private set; }

        private void OnExaminarAvatarCommandExecute()
        {
            OpenFileDialog Abrir = new OpenFileDialog();
            BitmapImage auxImagen = new BitmapImage();
            Abrir.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (Abrir.ShowDialog() == true)
            {
                auxImagen.BeginInit();
                auxImagen.UriSource = new Uri(Abrir.FileName);
                auxImagen.EndInit();

                string path = auxImagen.UriSource.OriginalString;
                FileStream sr = new FileStream(path, FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[sr.Length];
                sr.Read(bytes, 0, bytes.Length);
                Socio.ImagenSocio = bytes;
                //ImagenSocio = new BitmapImage(new Uri(Abrir.FileName, UriKind.Absolute));
                //_rutaImagen = Abrir.FileName;
                //RutaImagen = new BitmapImage(new Uri(_rutaImagen));
                //Socio.ImagenSocio = new BitmapImage(new Uri(_rutaImagen));

            }
            //OpenFileDialog op = new OpenFileDialog();
            //op.Title = "Selecciona una imagen";
            //op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
            //  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
            //  "Portable Network Graphic (*.png)|*.png";
            //if (op.ShowDialog() == true)
            //{
            //    //Socio.AvatarPath = new BitmapImage(new Uri(op.FileName));
            //    Socio.AvatarPath = Socio.Id + "-" + DateTime.Now.Ticks +
            //        op.FileName.Substring(
            //            op.FileName.IndexOf(".", op.FileName.Length - 5));

            //    File.Copy(op.FileName, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            //        + @"\IconsAndImages\" + Socio.AvatarPath, true);
            //}
        }
        public BitmapImage ImagenSocio
        {
            get { return _imagenSocio; }
            set { SetProperty(ref _imagenSocio, value); }
        }
        public bool EdicionHabilitada
        {
            get { return _EdicionHabilitada; }
            set { SetProperty(ref _EdicionHabilitada, value); }
        }

        public SocioWrapper Socio
        {
            get { return _Socio; }
            set { SetProperty(ref _Socio, value); }
        }

        public void Load(SocioWrapper wrapper)
        {
            EdicionHabilitada = false;
            Socio = wrapper;
        }
    }
}
