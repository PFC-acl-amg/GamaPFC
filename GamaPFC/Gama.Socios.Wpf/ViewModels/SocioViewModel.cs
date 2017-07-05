﻿using Core;
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
        private SocioWrapper _Socio;

        public SocioViewModel()
        {
            Socio = new SocioWrapper(new Socio())
            {
                IsInEditionMode = true
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

                Socio.Imagen = bytes;
            }

        }

        public SocioWrapper Socio
        {
            get { return _Socio; }
            set { SetProperty(ref _Socio, value); }
        }

        public void Load(SocioWrapper wrapper)
        {
            Socio = wrapper;
            Socio.IsInEditionMode = false;
        }
    }
}
