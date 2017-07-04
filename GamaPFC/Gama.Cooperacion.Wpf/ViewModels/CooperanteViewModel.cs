using Core;
using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
using Microsoft.Win32;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class CooperanteViewModel : ViewModelBase
    {
        private CooperanteWrapper _Cooperante;
        public CooperanteViewModel()
        {
            Cooperante = new CooperanteWrapper(new Cooperante());

            // Por defecto lo ponemos a true, para cuando se use al añadir
            // uno nuevo. En ventanas de editar se pondrá a False para obligar
            // a pulsar el botón de 'Habilitar Edición'
            Cooperante.IsInEditionMode = true;

            ExaminarAvatarCommand = new DelegateCommand(OnExaminarAvatarCommandExecute);
        }
        public ICommand ExaminarAvatarCommand { get; private set; }
        private void OnExaminarAvatarCommandExecute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            BitmapImage imagenAuxiliar = new BitmapImage();
            openFileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                imagenAuxiliar.BeginInit();
                imagenAuxiliar.UriSource = new Uri(openFileDialog.FileName);
                imagenAuxiliar.EndInit();

                string imagenPath = imagenAuxiliar.UriSource.OriginalString;
                FileStream imagenFileStream = new FileStream(imagenPath, FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[imagenFileStream.Length];
                imagenFileStream.Read(bytes, 0, bytes.Length);

                Cooperante.Foto = bytes;
            }
        }
        public CooperanteWrapper Cooperante
        {
            get { return _Cooperante; }
            set { SetProperty(ref _Cooperante, value); }
        }
        public void Load(CooperanteWrapper wrapper)
        {
            Cooperante = wrapper;
            Cooperante.IsInEditionMode = false;
        }
    }
}