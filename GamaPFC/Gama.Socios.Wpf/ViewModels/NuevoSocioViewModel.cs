using Core;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Eventos;
using Gama.Socios.Wpf.Services;
using Gama.Socios.Wpf.Wrappers;
using NHibernate;
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
    public class NuevoSocioViewModel : ViewModelBase
    {
        private int _ModificarSocio=0;
        private IEventAggregator _EventAggregator;
        private ISocioRepository _SocioRepository;
        private SocioViewModel _SocioViewModel;
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser

        public NuevoSocioViewModel(
            ISocioRepository socioRepository,
            IEventAggregator eventAggregator,
            SocioViewModel socioViewModel, // Lo necesita porque se apoya en el usercontrol SocioView y tiene que poder pasarle informacion
            ISession session)
        {
            _EventAggregator = eventAggregator;
            _SocioViewModel = socioViewModel;
            _SocioRepository = socioRepository;
            _SocioRepository.Session = session;

            AceptarCommand = new DelegateCommand(OnAceptarCommand_Execute,
                OnAceptarCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand_Execute);

            Socio.PropertyChanged += Socio_PropertyChanged;
        }

        private void Socio_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ((DelegateCommand)AceptarCommand).RaiseCanExecuteChanged();
        }

        public SocioViewModel SocioViewModel
        {
            get { return _SocioViewModel; }
        }

        public SocioWrapper Socio
        {
            get { return _SocioViewModel.Socio; }
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
            if (_ModificarSocio == 0)
            {
                Socio.CreatedAt = DateTime.Now;
                _SocioRepository.Create(Socio.Model);
                Cerrar = true;
            }
            else
            {
                Socio.UpdatedAt = DateTime.Now;
                _SocioRepository.Update(Socio.Model);
                Socio.AcceptChanges();
                _ModificarSocio = 0;
                Cerrar = true;
            }
        }

        private bool OnAceptarCommand_CanExecute()
        {
            return Socio.IsValid;
        }

        private void OnCancelarCommand_Execute()
        {
            Cerrar = true;
        }
        public void Load(Socio socio)
        {
            var wrapper = new SocioWrapper(socio);
            _ModificarSocio = 1;
            _SocioViewModel.Load(wrapper);
            wrapper.IsInEditionMode = true;
            Socio.PropertyChanged += Socio_PropertyChanged;
        }
    }
}
