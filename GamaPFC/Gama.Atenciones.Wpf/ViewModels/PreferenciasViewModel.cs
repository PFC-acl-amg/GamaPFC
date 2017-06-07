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

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class PreferenciasViewModel
    {
        private PreferenciasDeAtenciones _Preferencias;
        private IEventAggregator _EventAggregator;

        public PreferenciasViewModel(PreferenciasDeAtenciones preferencias,
            IEventAggregator eventAggregator)
        {
            _Preferencias = preferencias;
            _EventAggregator = eventAggregator;

            Preferencias = new PreferenciasDeAtencionesWrapper(preferencias);

            GuardarCambiosCommand = new DelegateCommand(
                OnGuardarCambiosCommandExecute,
                () => Preferencias.IsChanged);

            CancelarCambiosCommand = new DelegateCommand(
                OnCancelarCambiosCommandExecute,
                () => Preferencias.IsChanged);

            Preferencias.PropertyChanged += Preferencias_PropertyChanged;
        }

        private void Preferencias_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ((DelegateCommand)GuardarCambiosCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CancelarCambiosCommand).RaiseCanExecuteChanged();
        }

        public ICommand GuardarCambiosCommand { get; private set; }
        public ICommand CancelarCambiosCommand { get; private set; }
        public ICommand ExaminarBackupPathCommand { get; private set; }

        public PreferenciasDeAtencionesWrapper Preferencias { get; private set; }

        private void OnExaminarBackupPathCommand()
        {

        }

        private void OnGuardarCambiosCommandExecute()
        {
            Preferencias.AcceptChanges();
            Preferencias.Model.Serializar();
            _EventAggregator.GetEvent<PreferenciasActualizadasEvent>().Publish();
        }

        private void OnCancelarCambiosCommandExecute()
        {
            Preferencias.RejectChanges();
        }
    }
}
