using Core;
using Gama.Common.CustomControls;
using Gama.Common.Eventos;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Eventos;
using Gama.Socios.Wpf.Services;
using Gama.Socios.Wpf.Views;
using LiveCharts;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Socios.Wpf.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private EventAggregator _EventAggregator;
        private PreferenciasDeSocios _Settings;
        private IPeriodoDeAltaRepository _PriodosDeAltaRepository;
        private ISocioRepository _SocioRepository;
        private ObservableCollection<Socio> _Socios;
        private string[] _Labels;
        private int _MesInicialSocios;
        private bool _VisibleOpcionesFiltro;
        private bool _VisibleContableGeneral;
        private bool _VisibleFiltroFechas;
        private double _CuotasPagadas;
        private double _CantidadCuotasPagadas;
        private double _CuotasPorPagar;
        private double _CantidadTotalPorPagar;
        private double _CuotasImpagadas;
        private double _CantidadTotalImpagadas;
        private DateTime? _FechaFinOpcion;
        private DateTime? _FechaInicioOpcion;
        private int _MesesParaMoroso = 0;

        public DashboardViewModel(ISocioRepository socioRepository,
            IPeriodoDeAltaRepository periodosDeAltaRepository,
            EventAggregator eventAggregator, 
            PreferenciasDeSocios settings,
            ISession session)
        {
            _SocioRepository = socioRepository;
            _SocioRepository.Session = session;
            _PriodosDeAltaRepository = periodosDeAltaRepository;
            _PriodosDeAltaRepository.Session = session;
            _EventAggregator = eventAggregator;
            _Settings = settings;
            _VisibleOpcionesFiltro = false;
            _VisibleContableGeneral = false;
            _VisibleFiltroFechas = false;
            _CuotasPorPagar = 1;
            _CantidadTotalPorPagar =1;
            _CuotasImpagadas = 1;
            _CantidadTotalImpagadas=1;
            _CuotasPagadas = 1;
            _CantidadCuotasPagadas = 1;
            _Socios = new ObservableCollection<Socio>(_SocioRepository.GetAll());
            ListaCompletaSocios = new ObservableCollection<LookupItem>(
                    _Socios
                    .OrderBy(x => x.Id)
                    .Select(a => new LookupItem
                    {
                        Id = a.Id,
                        DisplayMember1 = a.Nombre,
                        DisplayMember2 = a.Nif,
                        Imagen = a.Imagen
                    }));

            ListaParcialSocios = new ObservableCollection<Socio>(_SocioRepository.GetAll());
            _Socios = new ObservableCollection<Socio>(_SocioRepository.GetAll());
            ActualizarDatosContables();

            UltimosSocios = new ObservableCollection<LookupItem>(
                    _Socios
                    .OrderBy(x => x.Id)
                    .Take(_Settings.DashboardUltimosSocios)
                    .Select(a => new LookupItem
                    {
                        Id = a.Id,
                        DisplayMember1 = a.Nombre,
                        DisplayMember2 = a.Nif,
                        Imagen = a.Imagen
                    }));

            SociosCumpliendoBirthdays = new ObservableCollection<LookupItem>(
                _Socios
                    .Where(x => x.IsBirthday())
                    .Take(_Settings.DashboardSociosCumpliendoBirthdays)
                    .Select(a => new LookupItem
                    {
                        Id = a.Id,
                        DisplayMember1 = a.Nombre,
                        DisplayMember2 = a.Nif,
                        Imagen = a.Imagen
                    }));

            SociosMorosos = new ObservableCollection<LookupItem>(
                _Socios
                    .Where(x => x.EsMoroso(_Settings.MesesParaSerConsideradoMoroso))
                    .Take(_Settings.DashboardSociosMorosos)
                    .Select(a => new LookupItem
                    {
                        Id = a.Id,
                        DisplayMember1 = a.Nombre,
                        DisplayMember2 = a.Nif,
                        Imagen = a.Imagen
                    }));

            _EventAggregator.GetEvent<SocioCreadoEvent>().Subscribe(OnSocioCreadoEvent);
            _EventAggregator.GetEvent<SocioDadoDeBajaEvent>().Subscribe(OnSocioDadoDeBajaEvent);
            _EventAggregator.GetEvent<SocioActualizadoEvent>().Subscribe(OnSocioActualizadoEvent);

            _EventAggregator.GetEvent<PreferenciasActualizadasEvent>().Subscribe(OnPreferenciasActualizadasEvent);

            SeleccionarSocioCommand = new DelegateCommand<LookupItem>(OnSeleccionarSocioCommandExecute);
            VisibleFiltroSociosCommand = new DelegateCommand(OnVisibleFiltroSociosCommand);
            MostarFiltroContable = new DelegateCommand(OnMostarFiltroContable);
            MostrarFiltroFechas = new DelegateCommand(OnMostrarFiltroFechas);
            EditarSocioCommand = new DelegateCommand<LookupItem>(OnEditarSocioCommand);

            InicializarGraficos();
        }
        private void ActualizarDatosContables()
        {
            double TotalPagado = 0;
            double TotalSinPagar = 0;
            double TotalImpagos = 0;
            int CPagado = 0;
            int CPorPagar = 0;
            int CImpagada = 0;
            if ((FechaFinOpcion ==null)&&(FechaFinOpcion == null))  // No hay fecha seleccionada => se calcula todo
            {
                var altas = _PriodosDeAltaRepository.GetAll();
                _MesesParaMoroso = _Settings.MesesParaSerConsideradoMoroso;
                //Calculo contable---------------
                
                foreach (var Recibo in altas)
                {
                    foreach (var cuot in Recibo.Cuotas)
                    {
                        if (cuot.EstaPagado)
                        {
                            TotalPagado = TotalPagado + cuot.CantidadTotal;
                            CPagado += 1;
                        }
                        else
                        {
                            //int SemanaFin = DateTime.Compare(_FechaTest, ActividadSeleccionada.FechaDeFin.AddDays(-7));
                            int FueraPlazo = DateTime.Compare(cuot.Fecha.AddMonths(_MesesParaMoroso), DateTime.Now.Date);
                            if (FueraPlazo == -1)
                            {
                                TotalImpagos = TotalImpagos + (cuot.CantidadTotal - cuot.CantidadPagada);
                                CImpagada++;
                            }
                            else
                            {
                                TotalSinPagar = TotalSinPagar + (cuot.CantidadTotal - cuot.CantidadPagada);
                                CPorPagar++;
                            }
                        }
                        //else TotalSinPagar = TotalSinPagar + cuot.CantidadPagada;
                    }
                }
                //Fin Calculo contable-----------
            }
            CuotasPagadas = CPagado;
            CantidadCuotasPagadas = TotalPagado;
            CuotasPorPagar = CPorPagar;
            CantidadTotalPorPagar = TotalSinPagar;
            CuotasImpagadas = CImpagada;
            CantidadTotalImpagadas = TotalImpagos;


        }

        private void OnEditarSocioCommand(LookupItem param)
        {
            var _socio = _SocioRepository.GetById(param.Id);
            var o = new NuevoSocioView();
            var vm = (NuevoSocioViewModel)o.DataContext;

            vm.Load(_socio);
            o.Title = "Editar Socio";
            o.ShowDialog();
        }

        private void OnMostrarFiltroFechas()
        {
            if (VisibleFiltroFechas == false) VisibleFiltroFechas = true;
            else VisibleFiltroFechas = false;
        }

        private void OnMostarFiltroContable()
        {
            if (VisibleContableGeneral == false) VisibleContableGeneral = true;
            else VisibleContableGeneral = false;
        }

        private void OnVisibleFiltroSociosCommand()
        {
            if (VisibleOpcionesFiltro == true) VisibleOpcionesFiltro = false;
            else VisibleOpcionesFiltro = true;
        }

        private void OnPreferenciasActualizadasEvent()
        {
            if (UltimosSocios.Count != _Settings.DashboardUltimosSocios)
            {
                UltimosSocios = new ObservableCollection<LookupItem>(
                        _Socios
                        .OrderBy(x => x.Id)
                        .Take(_Settings.DashboardUltimosSocios)
                        .Select(a => new LookupItem
                        {
                            Id = a.Id,
                            DisplayMember1 = a.Nombre,
                            DisplayMember2 = a.Nif,
                            Imagen = a.Imagen
                        }));

                OnPropertyChanged(nameof(UltimosSocios));
            }

            if (SociosCumpliendoBirthdays.Count != _Settings.DashboardSociosCumpliendoBirthdays)
            {
                SociosCumpliendoBirthdays = new ObservableCollection<LookupItem>(
                    _Socios
                        .Where(x => x.IsBirthday())
                        .Take(_Settings.DashboardSociosCumpliendoBirthdays)
                        .Select(a => new LookupItem
                        {
                            Id = a.Id,
                            DisplayMember1 = a.Nombre,
                            DisplayMember2 = a.Nif,
                            Imagen = a.Imagen
                        }));

                OnPropertyChanged(nameof(SociosCumpliendoBirthdays));
            }

            if (SociosMorosos.Count != _Settings.DashboardSociosMorosos)
            {
                SociosMorosos = new ObservableCollection<LookupItem>(
                    _Socios
                        .Where(x => x.EsMoroso(_Settings.MesesParaSerConsideradoMoroso))
                        .Take(_Settings.DashboardSociosMorosos)
                        .Select(a => new LookupItem
                        {
                            Id = a.Id,
                            DisplayMember1 = a.Nombre,
                            DisplayMember2 = a.Nif,
                            Imagen = a.Imagen
                        }));

                OnPropertyChanged(nameof(SociosMorosos));
            }
        }

        private void InicializarGraficos()
        {
            _Labels = new[] {
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago","Sep","Oct", "Nov", "Dic",
                "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", };

            _MesInicialSocios = 12 + (DateTime.Now.Month - 1) - _Settings.DashboardMesesAMostrarDeSociosNuevos + 1;

            SociosNuevosPorMes = new ChartValues<int>(_SocioRepository.GetSociosNuevosPorMes(
                       _Settings.DashboardMesesAMostrarDeSociosNuevos));
        }

        public ObservableCollection<LookupItem> UltimosSocios { get; private set; }
        public ObservableCollection<LookupItem> ListaCompletaSocios { get; private set; }
        public ObservableCollection<Socio> ListaParcialSocios { get; private set; }
        public ObservableCollection<LookupItem> SociosCumpliendoBirthdays { get; private set; }
        public ObservableCollection<LookupItem> SociosMorosos { get; private set; }
        public ChartValues<int> SociosNuevosPorMes { get; set;}
        public ICommand SeleccionarSocioCommand { get; private set; }
        public ICommand VisibleFiltroSociosCommand { get; private set; }
        public ICommand MostarFiltroContable { get; set; }
        public ICommand MostrarFiltroFechas { get; set; }
        public ICommand EditarSocioCommand { get; set; }


        public string[] SociosLabels =>
            _Labels.Skip(_MesInicialSocios)
                .Take(_Settings.DashboardMesesAMostrarDeSociosNuevos).ToArray();

        private void OnSeleccionarSocioCommandExecute(LookupItem socio)
        {
            _EventAggregator.GetEvent<SocioSeleccionadoEvent>().Publish(socio.Id);
        }

        private void OnSocioCreadoEvent(int id)
        {
            var socio = _SocioRepository.GetById(id);

            var lookupItem = new LookupItem
            {
                Id = socio.Id,
                DisplayMember1 = socio.Nombre,
                DisplayMember2 = socio.Nif,
                Imagen = socio.Imagen
            };

            ListaCompletaSocios.Insert(0, lookupItem);

            // Si es su cumpleaños, añadirllo
            if (socio.IsBirthday())
            {
                SociosCumpliendoBirthdays.Insert(0, lookupItem);
            }
        }

        private void OnSocioDadoDeBajaEvent(int id)
        {
            UltimosSocios.Remove(UltimosSocios.First(x => x.Id == id));
            SociosCumpliendoBirthdays.Remove(SociosCumpliendoBirthdays.First(x => x.Id == id));
            
            // No lo quitamos de la lista de morosos ya que aunque esté dado de baja, queremos
            // que se visualice
        }

        private void OnSocioActualizadoEvent(Socio socioActualizado)
        {
            var socio = socioActualizado;
            int id = socio.Id;

            var socioDesactualizado = ListaCompletaSocios.Where(x => x.Id == id).FirstOrDefault();
            if (socioDesactualizado != null)
            {
                socioDesactualizado.DisplayMember1 = socio.Nombre;
                socioDesactualizado.DisplayMember2 = socio.Nif;
                socioDesactualizado.Imagen = socio.Imagen;
            }

            // Actualizar a cumpleañeros o lista de morosos. Tal vez hay que quitarlo o ponerlo.

            // Si es cumpleañeros y no está en la lista, añadirlo
            if (socioActualizado.IsBirthday() &&
                SociosCumpliendoBirthdays.FirstOrDefault(x => x.Id == id) == null)
            {
                SociosCumpliendoBirthdays.Insert(0, socioDesactualizado);
            }

            // Si ahora no es cumpleañero y está en la lista, quitarlo
            if (!socioActualizado.IsBirthday() &&
                SociosCumpliendoBirthdays.FirstOrDefault(x => x.Id == id) != null)
            {
                SociosCumpliendoBirthdays.Remove(SociosCumpliendoBirthdays.FirstOrDefault(x => x.Id == id));
            }

            // Si es moroso y no está en la lista, añadirlo
            if (socioActualizado.EsMoroso() &&
                SociosMorosos.FirstOrDefault(x => x.Id == id) == null)
            {
                SociosMorosos.Insert(0, socioDesactualizado);
            }

            // Si ahora no es moroso y está en la lista, quitarlo
            if (!socioActualizado.EsMoroso(_Settings.MesesParaSerConsideradoMoroso) &&
                SociosMorosos.FirstOrDefault(x => x.Id == id) != null)
            {
                SociosMorosos.Remove(SociosMorosos.FirstOrDefault(x => x.Id == id));
            }
        }
        public DateTime? FechaInicioOpcion
        {
            get { return _FechaInicioOpcion; }
            set { SetProperty(ref _FechaInicioOpcion, value); }
        }
        public DateTime? FechaFinOpcion
        {
            get { return _FechaFinOpcion; }
            set { SetProperty(ref _FechaFinOpcion, value); }
        }
        public double CuotasPagadas
        {
            get { return _CuotasPagadas; }
            set { SetProperty(ref _CuotasPagadas, value); }
        }
        
        public double CantidadCuotasPagadas
        {
            get { return _CantidadCuotasPagadas; }
            set { SetProperty(ref _CantidadCuotasPagadas, value); }
        }
        public double CuotasPorPagar
        {
            get { return _CuotasPorPagar; }
            set { SetProperty(ref _CuotasPorPagar, value); }
        }
        public double CantidadTotalPorPagar
        {
            get { return _CantidadTotalPorPagar; }
            set { SetProperty(ref _CantidadTotalPorPagar, value); }
        }
        public double CuotasImpagadas
        {
            get { return _CuotasImpagadas; }
            set { SetProperty(ref _CuotasImpagadas, value); }
        }
        public double CantidadTotalImpagadas
        {
            get { return _CantidadTotalImpagadas; }
            set { SetProperty(ref _CantidadTotalImpagadas, value); }
        }
        public bool VisibleFiltroFechas
        {
            get { return _VisibleFiltroFechas; }
            set { SetProperty(ref _VisibleFiltroFechas, value); }
        }
        public bool VisibleContableGeneral
        {
            get { return _VisibleContableGeneral; }
            set { SetProperty(ref _VisibleContableGeneral, value); }
        }
        public bool VisibleOpcionesFiltro
        {
            get { return _VisibleOpcionesFiltro; }
            set { SetProperty(ref _VisibleOpcionesFiltro, value); }
        }
    }
}
