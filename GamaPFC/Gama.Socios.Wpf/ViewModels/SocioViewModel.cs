using Core;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Wrappers;
using Prism.Commands;
using System.Windows.Input;
using System;
using Microsoft.Win32;
using System.IO;

namespace Gama.Socios.Wpf.ViewModels
{
    public class SocioViewModel : ViewModelBase
    {
        private bool _EdicionHabilitada;
        private SocioWrapper _Socio;

        public SocioViewModel()
        {
            _EdicionHabilitada = true;
            Socio = new SocioWrapper(new Socio());

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
                //Socio.AvatarPath = new BitmapImage(new Uri(op.FileName));
                Socio.AvatarPath = Socio.Id + "-" + DateTime.Now.Ticks +
                    op.FileName.Substring(
                        op.FileName.IndexOf(".", op.FileName.Length - 5));

                File.Copy(op.FileName, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + @"\IconsAndImages\" + Socio.AvatarPath, true);
            }
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
