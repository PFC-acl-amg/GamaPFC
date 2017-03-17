using Gama.Socios.Wpf.Services;
using Gama.Socios.Wpf.Wrappers;
using Prism.Commands;
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
        private IPreferenciasDeSocios _Settings;

        public PreferenciasViewModel(PreferenciasDeSocios settings)
        {
            _Settings = settings;
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

        public IPreferenciasDeSocios SociosSettings
        {
            get { return _Settings; }
        }

        public PreferenciasDeSociosWrapper Preferencias { get; private set; }

        private void OnGuardarCambiosCommandExecute()
        {
            Preferencias.AcceptChanges();
        }

        private void OnCancelarCambiosCommandExecute()
        {
            Preferencias.RejectChanges();
        }
    }
}
