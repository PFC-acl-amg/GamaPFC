using Core;
using Gama.Common.Eventos;
using Gama.Socios.Wpf.Services;
using Gama.Socios.Wpf.Wrappers;
using Prism.Commands;
using Prism.Events;
using System.Windows.Input;

namespace Gama.Socios.Wpf.ViewModels
{
    public class PreferenciasViewModel : ViewModelBase
    {
        private Preferencias _Preferencias;
        private IEventAggregator _EventAggregator;
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser

        public PreferenciasViewModel(Preferencias preferencias,
            IEventAggregator eventAggregator)
        {
            _Preferencias = preferencias;
            _EventAggregator = eventAggregator;

            Preferencias = new PreferenciasWrapper(preferencias);

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
        public PreferenciasWrapper Preferencias { get; private set; }

        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
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
