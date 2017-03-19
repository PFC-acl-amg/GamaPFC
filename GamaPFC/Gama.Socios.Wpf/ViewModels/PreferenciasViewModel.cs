using Gama.Common.Eventos;
using Gama.Socios.Wpf.Services;
using Gama.Socios.Wpf.Wrappers;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Socios.Wpf.ViewModels
{
    public class PreferenciasViewModel
    {
        private PreferenciasDeSocios _Settings;
        private IEventAggregator _EventAggregator;

        public PreferenciasViewModel(PreferenciasDeSocios settings,
            IEventAggregator eventAggregator)
        {
            _Settings = settings;
            _EventAggregator = eventAggregator;

            Preferencias = new PreferenciasDeSociosWrapper(settings);

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

        public PreferenciasDeSociosWrapper Preferencias { get; private set; }

        private void OnGuardarCambiosCommandExecute()
        {
            Preferencias.AcceptChanges();
            _EventAggregator.GetEvent<PreferenciasActualizadasEvent>().Publish();
        }

        private void OnCancelarCambiosCommandExecute()
        {
            Preferencias.RejectChanges();
        }
    }
}
