using Core;
using Gama.Atenciones.Business;
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
        private bool _EdicionHabilitada;
        private PersonaWrapper _Persona;

        public PersonaViewModel()
        {
            _EdicionHabilitada = true;
            Persona = new PersonaWrapper(new Persona());

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

        public bool EdicionHabilitada
        {
            get { return _EdicionHabilitada; }
            set { SetProperty(ref _EdicionHabilitada, value); }
        }

        public PersonaWrapper Persona
        {
            get { return _Persona; }
            set { SetProperty(ref _Persona, value); }
        }
        
        public void Load(PersonaWrapper wrapper)
        {
            EdicionHabilitada = false;
            Persona = wrapper;
        }
    }
}


        //Se mantiene comentado porque nos resulta código útil
        //private void OnExaminarAvatarCommandExecute()
        //{
        //    OpenFileDialog op = new OpenFileDialog();
        //    op.Title = "Selecciona una imagen";
        //    op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
        //      "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
        //      "Portable Network Graphic (*.png)|*.png";
        //    if (op.ShowDialog() == true)
        //    {
        //        Persona.AvatarPath = Persona.Id + "-" + DateTime.Now.Ticks +
        //            op.FileName.Substring(
        //                op.FileName.IndexOf(".", op.FileName.Length - 5));
        //
        //        File.Copy(op.FileName, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        //            + @"\IconsAndImages\" + Persona.AvatarPath, true);
        //
        //        OnPropertyChanged(nameof(Persona));
        //    }
        //}