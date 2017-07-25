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
        private PreferenciasDeSocios _Preferencias;
        private ExportService _ExportService;
        private IPeriodoDeAltaRepository _PeriodosDeAltaRepository;
        private ISocioRepository _SocioRepository;
        private ICuotaRepository _CuotaRepository;
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
        private int _NumSociosFiltrados = 0;
        private int _NumTotalSocios = 0;
        private string _NacionEscogida;
        private string _EdadEscogida;
        private string _EstadoAltaEscogido;

        public DashboardViewModel(ISocioRepository socioRepository,
            ExportService exportService,
            ICuotaRepository cuotaRepository,
            IPeriodoDeAltaRepository periodosDeAltaRepository,
            EventAggregator eventAggregator,
            PreferenciasDeSocios preferencias,
            ISession session)
        {
            _SocioRepository = socioRepository;
            _CuotaRepository = cuotaRepository;
            _ExportService = exportService;
            _PeriodosDeAltaRepository = periodosDeAltaRepository;
            _EventAggregator = eventAggregator;
            _Preferencias = preferencias;

            _CuotaRepository.Session = session;
            _SocioRepository.Session = session;
            _PeriodosDeAltaRepository.Session = session;

            _VisibleOpcionesFiltro = true;
            _VisibleContableGeneral = true;
            _VisibleFiltroFechas = false;

            _CuotasPorPagar = 1;
            _CantidadTotalPorPagar = 1;
            _CuotasImpagadas = 1;
            _CantidadTotalImpagadas = 1;
            _CuotasPagadas = 1;
            _CantidadCuotasPagadas = 1;

            _Socios = new ObservableCollection<Socio>(_SocioRepository.GetAll());

            Socios = new ObservableCollection<LookupItem>(
                    _Socios
                    .OrderBy(x => x.Id)
                    .Select(a => new LookupItem
                    {
                        Id = a.Id,
                        FechaDeNacimiento = a.FechaDeNacimiento,
                        Telefono = a.Telefono,
                        Email = a.Email,
                        DisplayMember1 = a.Nombre,
                        DisplayMember2 = a.Nif,
                        Imagen = a.Imagen,
                        Nacionalidad = a.Nacionalidad,
                        EstaDadoDeAlta = a.EstaDadoDeAlta
                    }));

            SociosFiltrados = new ObservableCollection<LookupItem>(Socios);
            ActualizarDatosContables();

            SociosMorosos = new ObservableCollection<LookupItem>(
                _Socios
                    .Where(x => x.EsMoroso(_Preferencias.MesesParaSerConsideradoMoroso))
                    .Select(a => new LookupItem
                    {
                        Id = a.Id,
                        DisplayMember1 = a.Nombre,
                        DisplayMember2 = a.Nif,
                        Imagen = a.Imagen
                    }));

            Edades = new ObservableCollection<string>();
            Nacionalidades = new ObservableCollection<string>();
            EstadosDeAlta = new ObservableCollection<string>();

            _EventAggregator.GetEvent<SocioCreadoEvent>().Subscribe(OnSocioCreadoEvent);
            _EventAggregator.GetEvent<SocioDadoDeBajaEvent>().Subscribe(OnSocioDadoDeBajaEvent);
            _EventAggregator.GetEvent<SocioActualizadoEvent>().Subscribe(OnSocioActualizadoEvent);
            _EventAggregator.GetEvent<PeriodoDeAltaActualizadoEvent>().Subscribe(OnNuevasCuotasSinContar);
            _EventAggregator.GetEvent<PreferenciasActualizadasEvent>().Subscribe(OnPreferenciasActualizadasEvent);
            _EventAggregator.GetEvent<ContabilidadModificadaEvent>().Subscribe(OnContabilidadModificadaEvent);

            SeleccionarSocioCommand = new DelegateCommand<LookupItem>(OnSeleccionarSocioCommandExecute);
            VisibleFiltroSociosCommand = new DelegateCommand(OnVisibleFiltroSociosCommand);
            MostrarFiltroFechas = new DelegateCommand(OnMostrarFiltroFechas);
            EditarSocioCommand = new DelegateCommand<LookupItem>(OnEditarSocioCommand);
            AplicarFiltroCommand = new DelegateCommand(OnAplicarFiltroCommand_Execute);
            ResetearFiltroCommand = new DelegateCommand(OnResetearFiltroCommand);
            MostarFiltroContable = new DelegateCommand(OnMostarFiltroContable);
            ExportarListaFiltrada = new DelegateCommand(OnExportarListaFiltrada);
            FiltroContableCommand = new DelegateCommand(OnFiltroContableCommand);
            ResetearFechaContableCommand = new DelegateCommand(OnResetearFechaContableCommand);

            PrepararFiltro();
            OnAplicarFiltroCommand_Execute();
        }

        //----------------------------------------
        // Contenedores, Listas, Entidades, Views, ViewModels, ...
        //----------------------------------------
        public ObservableCollection<LookupItem> Socios { get; private set; }
        public ObservableCollection<LookupItem> SociosMorosos { get; private set; }
        public ObservableCollection<LookupItem> SociosFiltrados { get; set; }
        public ObservableCollection<string> Nacionalidades { get; set; }
        public ObservableCollection<string> Edades { get; set; }
        public ObservableCollection<string> EstadosDeAlta { get; set; }
        public ChartValues<int> SociosNuevosPorMes { get; set; }
        //----------------------------------------
        // ICommands - Declaraciones
        //----------------------------------------
        public ICommand SeleccionarSocioCommand { get; private set; }
        public ICommand VisibleFiltroSociosCommand { get; private set; }
        public ICommand MostarFiltroContable { get; set; }
        public ICommand MostrarFiltroFechas { get; set; }
        public ICommand EditarSocioCommand { get; set; }
        public ICommand AplicarFiltroCommand { get; set; }
        public ICommand ResetearFiltroCommand { get; set; }
        public ICommand ExportarListaFiltrada { get; set; }
        public ICommand FiltroContableCommand { get; set; }
        public ICommand ResetearFechaContableCommand { get; set; }

        //----------------------------------------
        // ICommands - Implementaciones
        //----------------------------------------
        private void OnSeleccionarSocioCommandExecute(LookupItem socio)
        {
            _EventAggregator.GetEvent<SocioSeleccionadoEvent>().Publish(socio.Id);
        }
        private void OnVisibleFiltroSociosCommand()
        {
            if (VisibleOpcionesFiltro == true) VisibleOpcionesFiltro = false;
            else
            {
                PrepararFiltro();
                VisibleOpcionesFiltro = true;
            }

        }
        private void OnMostrarFiltroFechas()
        {
            if (VisibleFiltroFechas == false) VisibleFiltroFechas = true;
            else VisibleFiltroFechas = false;
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
        private void OnAplicarFiltroCommand_Execute()
        {
            var listadoAuxiliar = new List<LookupItem>(Socios);

            if (NacionEscogida != "Todas")
                listadoAuxiliar = listadoAuxiliar.Where(x => x.Nacionalidad == NacionEscogida).ToList();

            if (EstadoAltaEscogido == "Alta")
                listadoAuxiliar = listadoAuxiliar.Where(x => x.EstaDadoDeAlta).ToList();
            else if (EstadoAltaEscogido == "Baja")
                listadoAuxiliar = listadoAuxiliar.Where(x => !x.EstaDadoDeAlta).ToList();

            if (EdadEscogida != "Todas")
            {
                int edadMinima = 0;
                int edadMaxima = 200;
                switch (EdadEscogida)
                {
                    case "Hasta 25":
                        edadMinima = 0;
                        edadMaxima = 25;
                        break;
                    case "26-40":
                        edadMinima = 26;
                        edadMaxima = 40;
                        break;
                    case "41-55":
                        edadMinima = 41;
                        edadMaxima = 55;
                        break;
                    case "56-65":
                        edadMinima = 56;
                        edadMaxima = 65;
                        break;
                    case "Más de 65":
                        edadMinima = 65;
                        edadMaxima = 200;
                        break;
                }

                listadoAuxiliar = listadoAuxiliar.Where(
                    x => (DateTime.Now.Year - x.FechaDeNacimiento.Value.Year) >= edadMinima
                       && (DateTime.Now.Year - x.FechaDeNacimiento.Value.Year) <= edadMaxima).ToList();
            }

            SociosFiltrados.Clear();
            SociosFiltrados.AddRange(listadoAuxiliar);

            NumeroTotalDeSociosFiltrados = SociosFiltrados.Count;
            NumeroTotalDeSocios = Socios.Count;
            OnPropertyChanged(nameof(SociosFiltrados));
        }
        private void OnResetearFiltroCommand()
        {
            EdadEscogida = "Todas";
            NacionEscogida = "Todas";
            EstadoAltaEscogido = "Todos";
            OnAplicarFiltroCommand_Execute();
        }
        private void OnMostarFiltroContable()
        {
            if (VisibleContableGeneral == false)
            {
                VisibleContableGeneral = true;

            }
            else VisibleContableGeneral = false;
        }
        private void OnExportarListaFiltrada()
        {

            List<LookupItem> ListaFiltrada;
            ListaFiltrada = new List<LookupItem>();
            foreach (var item in Socios) ListaFiltrada.Add(item);
            _ExportService.ExportarListaFiltrada(ListaFiltrada);
        }
        private void OnFiltroContableCommand()
        {
            DateTime? FInicio = FechaInicioOpcion;
            DateTime? FFinal = FechaFinOpcion;
            double TotalPagado = 0;
            double TotalSinPagar = 0;
            double TotalImpagos = 0;
            CuotasPagadas = 0;
            CantidadCuotasPagadas = 0;
            CuotasPorPagar = 0;
            CantidadTotalPorPagar = 0;
            CuotasImpagadas = 0;
            CantidadTotalImpagadas = 0;
            int CPagado = 0;
            int CPorPagar = 0;
            int CImpagada = 0;
            var altas = _PeriodosDeAltaRepository.GetAll();
            _MesesParaMoroso = _Preferencias.MesesParaSerConsideradoMoroso;
            if ((FechaFinOpcion != null) && (FechaFinOpcion != null))  // No hay fecha seleccionada => se calcula todo
            {
                foreach (var Recibo in altas)
                {
                    foreach (var cuot in Recibo.Cuotas)
                    {
                        int Inicio = DateTime.Compare(cuot.Fecha, (DateTime)FInicio);
                        int Final = DateTime.Compare(cuot.Fecha, (DateTime)FFinal);
                        if ((Inicio == 1) && (Final == -1))
                        {
                            if (cuot.EstaPagado)
                            {
                                TotalPagado = TotalPagado + cuot.CantidadTotal;
                                CPagado += 1;
                            }
                            else
                            {
                                TotalSinPagar = TotalSinPagar + (cuot.CantidadTotal - cuot.CantidadPagada);
                                CPorPagar++;
                            }
                        }
                        int FIImpago = DateTime.Compare(cuot.Fecha, (DateTime)FInicio.Value.AddMonths(-_MesesParaMoroso));
                        int FFimpago = DateTime.Compare(cuot.Fecha, (DateTime)FFinal.Value.AddMonths(-_MesesParaMoroso));
                        if ((FIImpago == -1) || (FFimpago == -1))
                        {
                            if (cuot.EstaPagado == false)
                            {
                                TotalImpagos = TotalImpagos + (cuot.CantidadTotal - cuot.CantidadPagada);
                                CImpagada++;
                            }
                        }

                    }
                }
                //Fin Calculo contable-----------
            }
            else
            {
                foreach (var Recibo in altas)
                {
                    foreach (var cuot in Recibo.Cuotas)
                    {
                        int Inicio = DateTime.Compare(cuot.Fecha, (DateTime)FInicio);
                        int Final = DateTime.Compare(cuot.Fecha, DateTime.Now);
                        if ((Inicio == 1) && (Final == -1))
                        {
                            if (cuot.EstaPagado)
                            {
                                TotalPagado = TotalPagado + cuot.CantidadTotal;
                                CPagado += 1;
                            }
                            else
                            {
                                TotalSinPagar = TotalSinPagar + (cuot.CantidadTotal - cuot.CantidadPagada);
                                CPorPagar++;
                            }
                        }
                        int FIImpago = DateTime.Compare(cuot.Fecha, (DateTime)FInicio.Value.AddMonths(-_MesesParaMoroso));
                        int FFimpago = DateTime.Compare(cuot.Fecha, (DateTime)FFinal.Value.AddMonths(-_MesesParaMoroso));
                        if ((FIImpago == -1) || (FFimpago == -1))
                        {
                            TotalImpagos = TotalImpagos + (cuot.CantidadTotal - cuot.CantidadPagada);
                            CImpagada++;
                        }

                    }
                }
            }
            CuotasPagadas = CPagado;
            CantidadCuotasPagadas = TotalPagado;
            CuotasPorPagar = CPorPagar;
            CantidadTotalPorPagar = TotalSinPagar;
            CuotasImpagadas = CImpagada;
            CantidadTotalImpagadas = TotalImpagos;
        }
        private void OnResetearFechaContableCommand()
        {
            FechaInicioOpcion = null;
            FechaFinOpcion = null;
            CuotasPagadas = 0;
            CantidadCuotasPagadas = 0;
            CuotasPorPagar = 0;
            CantidadTotalPorPagar = 0;
            CuotasImpagadas = 0;
            CantidadTotalImpagadas = 0;
            ActualizarDatosContables();

        }
        
        //--------------------------------------------------
        // Eventos - 
        //--------------------------------------------------
        private void OnNuevasCuotasSinContar(int obj)
        {
            OnResetearFechaContableCommand();
        }
        private void OnPreferenciasActualizadasEvent()
        {
            if (SociosMorosos.Count != _Preferencias.DashboardSociosMorosos)
            {
                SociosMorosos = new ObservableCollection<LookupItem>(
                    _Socios
                        .Where(x => x.EsMoroso(_Preferencias.MesesParaSerConsideradoMoroso))
                        .Take(_Preferencias.DashboardSociosMorosos)
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
        private void OnSocioCreadoEvent(int id)
        {
            var socio = _SocioRepository.GetById(id);
            var lookupItem = new LookupItem
            {
                Id = socio.Id,
                FechaDeNacimiento = socio.FechaDeNacimiento,
                Telefono = socio.Telefono,
                Email = socio.Email,
                DisplayMember1 = socio.Nombre,
                DisplayMember2 = socio.Nif,
                Imagen = socio.Imagen,
                Nacionalidad = socio.Nacionalidad,
                EstaDadoDeAlta = socio.EstaDadoDeAlta
            };
            SociosFiltrados.Insert(0, lookupItem);
            Socios.Add(lookupItem);
            PrepararFiltro();
            OnPropertyChanged("SociosFiltrados");
        }
        private void OnSocioDadoDeBajaEvent(int id)
        {
            // Se pone a Dado de baja pero no se elimina de la base de datos.
        }
        private void OnSocioActualizadoEvent(Socio socioActualizado)
        {
            //var socio = socioActualizado;
            int id = socioActualizado.Id;

            var socioDesactualizado = SociosFiltrados.Where(x => x.Id == id).FirstOrDefault();
            if (socioDesactualizado != null)
            {
                socioDesactualizado.Id = socioActualizado.Id;
                socioDesactualizado.FechaDeNacimiento = socioActualizado.FechaDeNacimiento;
                socioDesactualizado.Telefono = socioActualizado.Telefono;
                socioDesactualizado.Email = socioActualizado.Email;
                socioDesactualizado.DisplayMember1 = socioActualizado.Nombre;
                socioDesactualizado.DisplayMember2 = socioActualizado.Nif;
                socioDesactualizado.Imagen = socioActualizado.Imagen;
                socioDesactualizado.Nacionalidad = socioActualizado.Nacionalidad;
                socioDesactualizado.EstaDadoDeAlta = socioActualizado.EstaDadoDeAlta;
            }
            var socioDesactualizado1 = Socios.Where(x => x.Id == id).FirstOrDefault();
            if (socioDesactualizado != null)
            {
                socioDesactualizado1.Id = socioActualizado.Id;
                socioDesactualizado1.FechaDeNacimiento = socioActualizado.FechaDeNacimiento;
                socioDesactualizado1.Telefono = socioActualizado.Telefono;
                socioDesactualizado1.Email = socioActualizado.Email;
                socioDesactualizado1.DisplayMember1 = socioActualizado.Nombre;
                socioDesactualizado1.DisplayMember2 = socioActualizado.Nif;
                socioDesactualizado1.Imagen = socioActualizado.Imagen;
                socioDesactualizado1.Nacionalidad = socioActualizado.Nacionalidad;
                socioDesactualizado1.EstaDadoDeAlta = socioActualizado.EstaDadoDeAlta;
            }
            // Si es moroso y no está en la lista, añadirlo
            if (socioActualizado.EsMoroso() &&
                SociosMorosos.FirstOrDefault(x => x.Id == id) == null)
            {
                SociosMorosos.Insert(0, socioDesactualizado);
            }

            // Si ahora no es moroso y está en la lista, quitarlo
            if (!socioActualizado.EsMoroso(_Preferencias.MesesParaSerConsideradoMoroso) &&
                SociosMorosos.FirstOrDefault(x => x.Id == id) != null)
            {
                SociosMorosos.Remove(SociosMorosos.FirstOrDefault(x => x.Id == id));
            }
        }
        private void OnContabilidadModificadaEvent(int Num)
        {
            ActualizarDatosContables();
        }
        //--------------------------------------------------
        // Bindings - 
        //--------------------------------------------------
        public string NacionEscogida
        {
            get { return _NacionEscogida; }
            set
            {
                if (_NacionEscogida != value)
                {
                    _NacionEscogida = value;
                    OnPropertyChanged();
                }

            }
        }
        public string EdadEscogida
        {
            get { return _EdadEscogida; }
            set
            {
                if (_EdadEscogida != value)
                {
                    _EdadEscogida = value;
                    OnPropertyChanged();
                }

            }
        }
        public string EstadoAltaEscogido
        {
            get { return _EstadoAltaEscogido; }
            set
            {
                if (_EstadoAltaEscogido != value)
                {
                    _EstadoAltaEscogido = value;
                    OnPropertyChanged();
                }

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
        public int NumeroTotalDeSocios
        {
            get { return _NumTotalSocios; }
            set { SetProperty(ref _NumTotalSocios, value); }
        }
        public int NumeroTotalDeSociosFiltrados
        {
            get { return _NumSociosFiltrados; }
            set { SetProperty(ref _NumSociosFiltrados, value); }
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

        //--------------------------------------------------
        // Procedimiento y Finciones Auxiliares - 
        //--------------------------------------------------
        private void PrepararFiltro()
        {
            SociosFiltrados = new ObservableCollection<LookupItem>(
                    Socios
                    .OrderBy(x => x.Id)
                    .Select(a => new LookupItem
                    {
                        Id = a.Id,
                        FechaDeNacimiento = a.FechaDeNacimiento,
                        Telefono = a.Telefono,
                        Email = a.Email,
                        DisplayMember1 = a.DisplayMember1,
                        DisplayMember2 = a.DisplayMember2,
                        Imagen = a.Imagen,
                        Nacionalidad = a.Nacionalidad,
                        EstaDadoDeAlta = a.EstaDadoDeAlta
                    }));

            Nacionalidades.Clear();
            Nacionalidades.Add("Todas");
            Nacionalidades.AddRange(Socios.Select(x => x.Nacionalidad).Distinct());
            NacionEscogida = "Todas";

            Edades.Clear();
            Edades.AddRange(new string[] { "Todas", "Hasta 25", "26-40", "41-55", "56-65", "Más de 65" });
            EdadEscogida = "Todas";

            EstadosDeAlta.Clear();
            EstadosDeAlta.AddRange(new string[] { "Todos", "Alta", "Baja" });
            EstadoAltaEscogido = "Todos";
        }
        private void ActualizarDatosContables()
        {
            double TotalPagado = 0;
            double TotalSinPagar = 0;
            double TotalImpagos = 0;
            int CPagado = 0;
            int CPorPagar = 0;
            int CImpagada = 0;
            var altas = _PeriodosDeAltaRepository.GetAll();
            var AllCuotas = _CuotaRepository.GetAll();
            _MesesParaMoroso = _Preferencias.MesesParaSerConsideradoMoroso;
            if ((FechaFinOpcion == null) && (FechaFinOpcion == null))  // No hay fecha seleccionada => se calcula todo
            {
                foreach (var cuot in AllCuotas)
                {
                    if (cuot.EstaPagado)
                    {
                        TotalPagado = TotalPagado + cuot.CantidadTotal;
                        CPagado += 1;
                    }
                    else
                    {
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
                }
            }
            CuotasPagadas = CPagado;
            CantidadCuotasPagadas = TotalPagado;
            CuotasPorPagar = CPorPagar;
            CantidadTotalPorPagar = TotalSinPagar;
            CuotasImpagadas = CImpagada;
            CantidadTotalImpagadas = TotalImpagos;
        }
        
    }
}
