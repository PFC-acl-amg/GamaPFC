using Core;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class NuevoAsistenteViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private IAsistenteRepository _AsistenteRepository;
        private AsistenteViewModel _AsistenteViewModel;
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser

        public NuevoAsistenteViewModel(
            IAsistenteRepository asistenteRepository,
            IEventAggregator eventAggregator,
            AsistenteViewModel asistenteViewModel,
            ISession session)
        {
            _EventAggregator = eventAggregator;
            _AsistenteViewModel = asistenteViewModel;
            _AsistenteRepository = asistenteRepository;
            _AsistenteRepository.Session = session;

            AceptarCommand = new DelegateCommand(OnAceptarCommand_Execute,
                OnAceptarCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand_Execute);

            Asistente.PropertyChanged += Asistente_PropertyChanged;
        }

        private void Asistente_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ((DelegateCommand)AceptarCommand).RaiseCanExecuteChanged();
        }

        public AsistenteViewModel AsistenteViewModel
        {
            get { return _AsistenteViewModel; }
        }

        public AsistenteWrapper Asistente
        {
            get { return _AsistenteViewModel.Asistente; }
        }

        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }

        public ICommand AceptarCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }

        private void OnAceptarCommand_Execute()
        {
            _AsistenteRepository.Create(Asistente.Model);
            Cerrar = true;
        }

        private bool OnAceptarCommand_CanExecute()
        {
            return Asistente.IsValid;
        }

        private void OnCancelarCommand_Execute()
        {
            Cerrar = true;
        }
    }
}
