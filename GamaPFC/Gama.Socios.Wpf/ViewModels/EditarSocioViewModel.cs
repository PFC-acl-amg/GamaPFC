using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Gama.Socios.Wpf.Services;
using System.Windows.Input;
using Gama.Socios.Wpf.Wrappers;
using Gama.Socios.Wpf.Eventos;
using System.ComponentModel;
using Gama.Common.Views;
using Gama.Socios.Business;
using Prism;
using System.Collections.ObjectModel;

namespace Gama.Socios.Wpf.ViewModels
{
    public class EditarSocioViewModel : ViewModelBase, IConfirmNavigationRequest, IActiveAware
    {
        private EditarCuotasViewModel _CuotasVM;
        private IEventAggregator _EventAggregator;
        private EditarPeriodosDeAltaViewModel _EditarPeriodosDeAltaViewModel;
        private ISocioRepository _SocioRepository;
        private SocioViewModel _SocioVM;
        private bool _VisibleEstadoPagosCliente;
        private PreferenciasDeSocios _Settings;
        private int _MesesParaMoroso = 0;
        private int _CuotasPorPagarSocio;
        private double _CantidadTotalPorPagar;
        private int _CuotasImpagadasSocio;
        private double _CantidadImpagada;

        public EditarSocioViewModel(
            IEventAggregator eventAggregator,
            ISocioRepository socioRepository,
            SocioViewModel socioVM,
            EditarCuotasViewModel cuotasVM,
            EditarPeriodosDeAltaViewModel periodosDeAltaVM,
            PreferenciasDeSocios settings,
            ISession session)
        {
            _Settings = settings;
            _EventAggregator = eventAggregator;
            _SocioRepository = socioRepository;
            _SocioVM = socioVM;
            _CuotasVM = cuotasVM;
            _EditarPeriodosDeAltaViewModel = periodosDeAltaVM;

            _SocioRepository.Session = session;
            _CuotasVM.Session = session;
            _EditarPeriodosDeAltaViewModel.Session = session;

            _VisibleEstadoPagosCliente = false;
            
            ListaCuotasPorpagarSocio = new ObservableCollection<Cuota>();
            ListaCuotasImpagosSocios = new ObservableCollection<Cuota>();
            NuevoPeriodoDeAltaCommand = new DelegateCommand(OnNuevoPeriodoDeAltaCommandExecute);

            HabilitarEdicionCommand = new DelegateCommand(
                OnHabilitarEdicionCommand,
                () => !_SocioVM.Socio.IsInEditionMode);

            ActualizarCommand = new DelegateCommand(
                OnActualizarCommand,
                () => _SocioVM.Socio.IsInEditionMode
                   && Socio.IsChanged
                   && Socio.IsValid);

            CancelarEdicionCommand = new DelegateCommand(OnCancelarEdicionCommand,
                () => _SocioVM.Socio.IsInEditionMode);

            DarDeAltaBajaCommand = new DelegateCommand(OnDarDeAltaBajaCommandExecute);
            VisibleContabilidadSocioCommand = new DelegateCommand(OnVisibleContabilidadSocioCommand);

            _EventAggregator.GetEvent<ContabilidadModificadaEvent>().Subscribe(OnContabilidadModificadaEvent);

            _SocioVM.PropertyChanged += SocioVM_PropertyChanged;
        }
        public ObservableCollection<Cuota> ListaCuotasPorpagarSocio { get; private set; }
        public ObservableCollection<Cuota> ListaCuotasImpagosSocios { get; private set; }

        private void OnVisibleContabilidadSocioCommand()
        {
            if (VisibleEstadoPagosCliente == true) VisibleEstadoPagosCliente = false;
            else VisibleEstadoPagosCliente = true;
        }
        public double CantidadImpagada
        {
            get { return _CantidadImpagada; }
            set { SetProperty(ref _CantidadImpagada, value); }
        }

        public int CuotasImpagadasSocio
        {
            get { return _CuotasImpagadasSocio; }
            set { SetProperty(ref _CuotasImpagadasSocio, value); }
        }
        public double CantidadTotalPorPagar
        {
            get { return _CantidadTotalPorPagar; }
            set { SetProperty(ref _CantidadTotalPorPagar, value); }
        }
        public int CuotasPorPagarSocio
        {
            get { return _CuotasPorPagarSocio; }
            set { SetProperty(ref _CuotasPorPagarSocio, value); }
        }
        public bool VisibleEstadoPagosCliente
        {
            get { return _VisibleEstadoPagosCliente; }
            set { SetProperty(ref _VisibleEstadoPagosCliente, value); }
        }

        private string _TextoDeDarDeAltaBaja;
        public string TextoDeDarDeAltaBaja
        {
            get { return _TextoDeDarDeAltaBaja; }
            set
            {
                _TextoDeDarDeAltaBaja = value;
                OnPropertyChanged();
            }
        }

        public SocioWrapper Socio
        {
            get { return _SocioVM.Socio; }
        }

        public SocioViewModel SocioVM
        {
            get { return _SocioVM; }
        }

        public EditarCuotasViewModel CuotasVM
        {
            get { return _CuotasVM; }
        }

        public EditarPeriodosDeAltaViewModel EditarPeriodosDeAltaViewModel
        {
            get { return _EditarPeriodosDeAltaViewModel; }
        }

        public ICommand NuevoPeriodoDeAltaCommand { get; private set; }
        public ICommand HabilitarEdicionCommand { get; private set; }
        public ICommand ActualizarCommand { get; private set; }
        public ICommand CancelarEdicionCommand { get; private set; }
        public ICommand DarDeAltaBajaCommand { get; private set; }
        public ICommand VisibleContabilidadSocioCommand { get; set; }

        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }

            set
            {
                SetProperty(ref _IsActive, value);
                if (_IsActive)
                    _EventAggregator.GetEvent<SocioSeleccionadoChangedEvent>().Publish(Socio.Id);
            }
        }
        private void OnNuevoPeriodoDeAltaCommandExecute()
        {
            _EditarPeriodosDeAltaViewModel.AddPeriodoDeAlta();
        }

        private void OnActualizarCommand()
        {
            _SocioRepository.Update(Socio.Model);
            _SocioVM.Socio.AcceptChanges();
            _SocioVM.Socio.IsInEditionMode = false;
            _SocioVM.Socio.IsInEditionMode = false;
            RefrescarTitulo(Socio.Nombre);
           
        }

        private void OnHabilitarEdicionCommand()
        {
            _SocioVM.Socio.IsInEditionMode = true;
        }

        private void OnCancelarEdicionCommand()
        {
            Socio.RejectChanges();
            _SocioVM.Socio.IsInEditionMode = false;
        }

        private void OnDarDeAltaBajaCommandExecute()
        {
            if (Socio.EstaDadoDeAlta)
            {
                Socio.EstaDadoDeAlta = false;
                TextoDeDarDeAltaBaja = "Dar de alta";
            }
            else
            {
                Socio.EstaDadoDeAlta = true;
                TextoDeDarDeAltaBaja = "Dar de baja";
            }

            _EventAggregator.GetEvent<SocioActualizadoEvent>().Publish(Socio.Model);
        }

        public bool IsNavigationTarget(int id)
        {
            return (Socio.Id == id);
        }

        public override void OnActualizarServidor()
        {
            //if (!Persona.IsChanged)
            //{
            //    var persona = new PersonaWrapper(
            //        (Persona)
            //        _PersonaRepository.GetById(Persona.Id)
            //        .DecryptFluent());

            //    _PersonaVM.Load(persona);
            //    _AtencionesVM.Load(_PersonaVM.Persona);
            //    _CitasVM.Load(_PersonaVM.Persona);
            //    RefrescarTitulo(persona.Nombre);
            //    _AtencionesVM.VerAtenciones = false;
            //}
        }

        public void OnNavigatedTo(int id)
        {
            try
            {
                if (string.IsNullOrEmpty(this.Socio.Nombre))
                {
                    var Socio = new SocioWrapper(
                   _SocioRepository.GetById(id));
                    _SocioVM.Load(Socio);
                    _EditarPeriodosDeAltaViewModel.Load(_SocioVM.Socio);
                    RefrescarTitulo(Socio.Nombre);
                    CalcularDebito(Socio);
                    TextoDeDarDeAltaBaja = Socio.EstaDadoDeAlta ? "Dar de baja" : "Dar de alta";
                }
                else return;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void RefrescarTitulo(string nombre)
        {
            if (nombre.Length > 20)
            {
                Title = nombre.Substring(0, 20) + "...";
            }
            else
            {
                Title = nombre;
            }
        }

        private void OnContabilidadModificadaEvent(int Num)
        {
            CalcularDebito(Socio);
        }
        private void CalcularDebito(SocioWrapper socioSeleccionado)
        {
            double TotalSinPagar = 0;
            double TotalImpagos = 0;
            int CPorPagar = 0;
            int CImpagada = 0;
            ListaCuotasPorpagarSocio.Clear();
            ListaCuotasImpagosSocios.Clear();
            var altas = socioSeleccionado.PeriodosDeAlta;
            _MesesParaMoroso = _Settings.MesesParaSerConsideradoMoroso;
                //Calculo contable---------------
            foreach (var Recibo in altas)
            {
                foreach (var cuot in Recibo.Cuotas)
                {
                    if (!cuot.EstaPagado)
                    {
                        int FueraPlazo = DateTime.Compare(cuot.Fecha.AddMonths(_MesesParaMoroso), DateTime.Now.Date);
                        if (FueraPlazo == -1)
                        {
                            ListaCuotasImpagosSocios.Add(cuot.Model);
                            TotalImpagos = TotalImpagos + (cuot.CantidadTotal - cuot.CantidadPagada);
                            CImpagada++;
                        }
                        else
                        {
                            ListaCuotasPorpagarSocio.Add(cuot.Model);
                            TotalSinPagar = TotalSinPagar + (cuot.CantidadTotal - cuot.CantidadPagada);
                            CPorPagar++;
                        }
                    }
                 }
             }
                //Fin Calculo contable-----------  
            CuotasPorPagarSocio = CPorPagar;
            CantidadTotalPorPagar = TotalSinPagar;
            CuotasImpagadasSocio = CImpagada;
            CantidadImpagada = TotalImpagos;
            OnPropertyChanged("ListaCuotasPorpagarSocio");
            OnPropertyChanged("ListaCuotasImpagosSocios");
        }

        private void InvalidateCommands()
        {
            ((DelegateCommand)HabilitarEdicionCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)ActualizarCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CancelarEdicionCommand).RaiseCanExecuteChanged();
        }

        private void SocioVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_SocioVM.Socio.IsInEditionMode))
            {
                InvalidateCommands();
            }
            else if (e.PropertyName == nameof(Socio))
            {
                Socio.PropertyChanged += (s, ea) => {
                    if (ea.PropertyName == nameof(Socio.IsChanged)
                        || ea.PropertyName == nameof(Socio.IsValid))
                    {
                        InvalidateCommands();
                    }
                };
            }
        }

        public bool ConfirmNavigationRequest()
        {
            if (Socio.IsChanged)
            {
                var o = new ConfirmarOperacionView();
                o.Mensaje = "Si sale se perderán los cambios, ¿Desea salir de todas formas?";
                o.ShowDialog();

                return o.EstaConfirmado;
            }

            return true;
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext,
            Action<bool> continuationCallback)
        {
            if (Socio.IsChanged)
            {
                var o = new ConfirmarOperacionView();
                o.Mensaje = "Si sale se perderán los cambios, ¿Desea salir de todas formas?";
                o.ShowDialog();

                continuationCallback.Invoke(o.EstaConfirmado);
            }
        }
    }
}
