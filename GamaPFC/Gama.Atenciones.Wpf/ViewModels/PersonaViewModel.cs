using Core;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Wrappers;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.IO;
using System.Windows.Input;

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
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Selecciona una imagen";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                Persona.AvatarPath = @"AvatarImages\" + Persona.Id + "-" + DateTime.Now.Ticks +
                    op.FileName.Substring(
                        op.FileName.IndexOf(".", op.FileName.Length - 5));

                if (!Directory.Exists("AvatarImages"))
                {
                    Directory.CreateDirectory("AvatarImages");
                }

                File.Copy(op.FileName, Persona.AvatarPath, true);

                OnPropertyChanged(nameof(Persona));
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

        public object Socio { get; private set; }

        public void Load(PersonaWrapper wrapper)
        {
            EdicionHabilitada = false;
            Persona = wrapper;
        }
    }
}
