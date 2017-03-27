using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Wrappers;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class AsistenteViewModel : ViewModelBase
    {
        private AsistenteWrapper _Asistente;

        public AsistenteViewModel()
        {
            Asistente = new AsistenteWrapper(new Asistente());

            // Por defecto lo ponemos a true, para cuando se use al añadir
            // uno nuevo. En ventanas de editar se pondrá a False para obligar
            // a pulsar el botón de 'Habilitar Edición'
            Asistente.IsInEditionMode = true;

            ExaminarAvatarCommand = new DelegateCommand(OnExaminarAvatarCommandExecute);
        }

        public ICommand ExaminarAvatarCommand { get; private set; }

        public AsistenteWrapper Asistente
        {
            get { return _Asistente; }
            set { SetProperty(ref _Asistente, value); }
        }
        
        // Este método es llamado cuando se va a editar un asistente, por eso
        // marcamos 'IsInEditionMode' como False, para obligar a pulsar el botón
        // de 'Habilitar Edición'
        public void Load(AsistenteWrapper wrapper)
        {
            Asistente = wrapper;
            Asistente.IsInEditionMode = false;
        }

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

                Asistente.Imagen = bytes;
            }
        }
    }
}
