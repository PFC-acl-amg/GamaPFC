using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Wrappers;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class AgregarCooperanteViewModel : ViewModelBase
    {
        private CooperanteWrapper _NuevoCooperante;
        private double _TamW;
        private double _TamH;
        private BitmapImage _Fotito;
        public AgregarCooperanteViewModel()
        {
            NuevoCooperante = new CooperanteWrapper(new Cooperante());


            ExaminarFotoCommand = new DelegateCommand(OnExaminarFotoCommandExecute);
        }
        public ICommand ExaminarFotoCommand { get; private set; }
        private void OnExaminarFotoCommandExecute()
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
                //-------
                string path = auxImagen.UriSource.OriginalString;
                FileStream sr = new FileStream(path, FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[sr.Length];
                sr.Read(bytes, 0, bytes.Length);
                NuevoCooperante.Foto = bytes;
                TamH = 105;
                TamW = 200;
            }
        }
        public BitmapImage Fotito
        {
            get { return _Fotito; }
            set { SetProperty(ref _Fotito, value); }
        }
        public CooperanteWrapper NuevoCooperante
        {
            get { return _NuevoCooperante; }
            set { SetProperty(ref _NuevoCooperante, value); }
        }
        public double TamW
        {
            get { return _TamW; }
            set { SetProperty(ref _TamW, value); }
        }
        public double TamH
        {
            get { return _TamH; }
            set { SetProperty(ref _TamH, value); }
        }
    }
}
