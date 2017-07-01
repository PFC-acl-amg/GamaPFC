using Core;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Wrappers;
using Gama.Common.Eventos;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using static System.Environment;
using Gama.Common.Debug;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class PreferenciasViewModel : ViewModelBase
    {
        private Preferencias _Preferencias;
        private IEventAggregator _EventAggregator;
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser

        public PreferenciasViewModel(Preferencias preferencias,
            IEventAggregator eventAggregator)
        {
            Debug.StartStopWatch();
            _Preferencias = preferencias;
            _EventAggregator = eventAggregator;

            Preferencias = new PreferenciasWrapper(preferencias);

            GuardarCambiosCommand = new DelegateCommand(
                OnGuardarCambiosCommandExecute,
                () => Preferencias.IsChanged);

            CancelarCambiosCommand = new DelegateCommand(OnCancelarCambiosCommandExecute);

            ExaminarBackupPathCommand = new DelegateCommand(OnExaminarBackupPathCommand);

            Preferencias.PropertyChanged += Preferencias_PropertyChanged;
            Debug.StopWatch("PreferenciasView");
        }

        private void Preferencias_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ((DelegateCommand)GuardarCambiosCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CancelarCambiosCommand).RaiseCanExecuteChanged();
        }

        public ICommand GuardarCambiosCommand { get; private set; }
        public ICommand CancelarCambiosCommand { get; private set; }
        public ICommand ExaminarBackupPathCommand { get; private set; }

        public PreferenciasWrapper Preferencias { get; private set; }

        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }

        private void OnExaminarBackupPathCommand()
        {
            var dialog = new CommonOpenFileDialog();

            dialog.Title = "My Title";
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = Preferencias.AutomaticBackupPath;

            dialog.AddToMostRecentlyUsedList = false;
            dialog.AllowNonFileSystemItems = false;
            dialog.DefaultDirectory = Preferencias.AutomaticBackupPath;
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Preferencias.AutomaticBackupPath = dialog.FileName;
            }
        }

        private void OnGuardarCambiosCommandExecute()
        {
            Preferencias.AcceptChanges();
            Preferencias.Model.Serializar();
            _EventAggregator.GetEvent<PreferenciasActualizadasEvent>().Publish();
            Cerrar = true;
        }

        private void OnCancelarCambiosCommandExecute()
        {
            Preferencias.RejectChanges();
            Cerrar = true;
        }
    }
}
