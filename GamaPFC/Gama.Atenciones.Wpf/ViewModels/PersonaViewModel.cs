using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Wrappers;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class PersonaViewModel : ViewModelBase
    {
        private PersonaWrapper _Persona;
        private Preferencias _Preferencias;

        public PersonaViewModel(Preferencias preferencias)
        {
            _Preferencias = preferencias;
            Persona = new PersonaWrapper(
                new Persona()
                {
                    EstadoCivil = EstadoCivil.NoProporcionado.ToString(),
                    ComoConocioAGama = ComoConocioAGama.NoProporcionado.ToString(),
                    ViaDeAccesoAGama = ViaDeAccesoAGama.NoProporcionado.ToString(),
                    OrientacionSexual = OrientacionSexual.NoProporcionado.ToString(),
                    IdentidadSexual = IdentidadSexual.NoProporcionado.ToString(),
                    NivelAcademico = NivelAcademico.NoProporcionado.ToString(),
                })
            {
                IsInEditionMode = _Preferencias.General_EdicionHabilitadaPorDefecto
            };

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

                Persona.Imagen = bytes; 
            }
        }

        public PersonaWrapper Persona
        {
            get { return _Persona; }
            set { SetProperty(ref _Persona, value); }
        }
        
        public void Load(PersonaWrapper wrapper)
        {
            Persona = wrapper;
            Persona.IsInEditionMode = _Preferencias.General_EdicionHabilitadaPorDefecto;
        }
    }
}