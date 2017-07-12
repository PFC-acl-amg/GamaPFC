using Core;
using Gama.Common;
using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Views;
using Gama.Cooperacion.Wpf.Wrappers;
using LiveCharts;
using LiveCharts.Wpf;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public static class ActividadesOpciones
    {
        public static string SeleccionParaListar = "None";
        public static List<bool> CheckBoxSeleccionados;
        public static List<bool> CheckBoxMeses;
    }
    
    public class DashboardViewModel : ViewModelBase
    {
        private IRegionManager _regionManager;
        private IActividadRepository _actividadRepository;
        private IEventAggregator _eventAggregator;
        private ICooperanteRepository _cooperanteRepository;
        private Preferencias _settings;
        private CooperanteWrapper _NuevoCooperante;
        private Cooperante _CooperanteSeleccionado;
        private int _mesInicialActividades;
        private string[] _Labels;
        private int _mesInicialCooperantes;
        private int _CooperantesMostrados;
        private bool _VisibleListaCooperantes;
        private bool _VisibleListaActividades;
        private bool _VisibleListaActividadesCooperante;
        private bool _VisibleDatosDNI;
        private bool _VisibleContacto;
        private bool _VisibleDireccion;
        private bool _VisibleCooperanteSeleccionado;
        private bool _VisibleListaTodosCooperantes;
        private bool _VisibleImagenSeleccionCooperante;
        private bool _VisibleDatosCooperanteSeleccionado;
        private bool _VisibleOpcionesActividades;
        private int _EnCursoCount;
        private int _NoComenzado;
        private int _Finalizado;
        private int _ProximasFinalizaciones;
        private int _FueraPlazo;
        private bool _FueraPlazoSeleccionado;
        private bool _FinalizadasSeleccionadas;
        private int _IdActividad;
        private bool _VisibleInfoAct;
        private bool _VisibleAviso;
        private DateTime _FechaInicioActividad;
        private DateTime _FechaFinalActividad;
        private string _NombreCoordinador;
        private int _ApellidoCoordinador;
        private string _MensajeAviso;
        private int _OpcionYear;
        private bool _EnCursoSeleccionado;
        private bool _PorComenzarSeleccionado;
        private bool _ProximasFechasSeleccionado;
        private bool _Enero;
        private bool _Febrero;
        private bool _Marzo;
        private bool _Abril;
        private bool _Mayo;
        private bool _Junio;
        private bool _Julio;
        private bool _Agosto;
        private bool _Septiembre;
        private bool _Octubre;
        private bool _Noviembre;
        private bool _Diciembre;
        private IEventoRepository _EventoRepository;
        private bool _VisibleOpcionesListar;
        private List<Evento> _Evento;

        private readonly int itemCount;

        public DashboardViewModel(
            IRegionManager regionManager,
            IActividadRepository actividadRepository,
            ICooperanteRepository cooperanteRepository,
            IEventoRepository eventoRepository,
            IEventAggregator eventAggregator, 
            Preferencias settings,
            ISession session)
        {
            Gama.Common.Debug.Debug.StartWatch();
            _regionManager = regionManager;
            _actividadRepository = actividadRepository;
            _cooperanteRepository = cooperanteRepository;
            _EventoRepository = eventoRepository;
            _actividadRepository.Session = session;
            _cooperanteRepository.Session = session;
            _EventoRepository.Session = session;
            _eventAggregator = eventAggregator;
            _settings = settings;
            _CooperantesMostrados = 0;

            _VisibleOpcionesListar = false;
            _VisibleInfoAct = false;
            _VisibleListaActividades = true;
            _VisibleListaCooperantes = false;
            _VisibleDatosDNI = false;
            _VisibleListaActividadesCooperante = false;
            _VisibleContacto = false;
            _VisibleDireccion = false;
            _VisibleCooperanteSeleccionado = false;
            _VisibleListaTodosCooperantes = false;
            _VisibleImagenSeleccionCooperante = true;
            _VisibleDatosCooperanteSeleccionado = false;
            _VisibleOpcionesActividades = false;
            _EnCursoSeleccionado = true;
            _PorComenzarSeleccionado = true;
            _ProximasFechasSeleccionado = true;
            _FueraPlazoSeleccionado = true;
            _FinalizadasSeleccionadas = true;

            ActividadesOpciones.CheckBoxSeleccionados = new List<bool>();
            ActividadesOpciones.CheckBoxMeses = new List<bool>();
            _Evento = new List<Evento>();
            _IdActividad = 100;
            _OpcionYear = DateTime.Today.Year;
            //_EnCursoCount = ColeccionEstadosActividades.EstadosActividades["Comenzado"];
            //_NoComenzado = ColeccionEstadosActividades.EstadosActividades["NoComenzado"];
            //_FueraPlazo = ColeccionEstadosActividades.EstadosActividades["FueraPlazo"];
            //_ProximasFinalizaciones = ColeccionEstadosActividades.EstadosActividades["ProximasFinalizaciones"];
            //_Finalizado = ColeccionEstadosActividades.EstadosActividades["Finalizado"];
            _FechaInicioActividad = new DateTime(2017, 01, 01);
            this.itemCount = 10;
            this.Items = new ObservableCollection<Item>();

            for (var i = 0; i < this.itemCount; i++)
            {
                this.Items.Add(new Item("Thi is item number " + i));
            }
            EventoActividad = new ObservableCollection<Evento>(_EventoRepository.GetAll());
            EventosFiltrados = new ObservableCollection<Evento>(_EventoRepository.GetAll());
            //Where(x => x.Id == obj.Id).FirstOrDefault();
            ListaCompletaActividades = new ObservableCollection<Actividad>(_actividadRepository.GetAll());
            ListaParcialActividades = new ObservableCollection<Actividad>(_actividadRepository.GetAll());
            ListaDeActividades = new ObservableCollection<LookupItem>(
                _actividadRepository.GetAll()
                    .Where(a=> a.Estado == Estado.Comenzado)
                    .OrderBy(a => a.FechaDeFin)
                    //.Take(_settings.DashboardActividadesAMostrar)
                .Select(a => new LookupItem
                {
                    Id = a.Id,
                    //DisplayMember1 = LookupItem.ShortenStringForDisplay(a.Titulo,
                    //    _settings.DashboardActividadesLongitudDeTitulos),
                    DisplayMember1 = a.Titulo,
                    Id_Coordinador = a.Coordinador.Id,
                    FechaDeInicioActividad = a.FechaDeInicio,
                }));
            ListaDeActividadesCooperante = new ObservableCollection<LookupItem>();
            ListaDeActividadesCoordina = new ObservableCollection<LookupItem>();
            //ListaDeActividadesCooperante = new ObservableCollection<LookupItem>(
            //    ListaCompletaActividades
            //        .OrderBy(a => a.FechaDeFin)
            //        .Take(_settings.DashboardActividadesAMostrar)
            //    .Select(a => new LookupItem
            //    {
            //        Id = a.Id,
            //        //DisplayMember1 = LookupItem.ShortenStringForDisplay(a.Titulo,
            //        //    _settings.DashboardActividadesLongitudDeTitulos),
            //        DisplayMember1 = a.Titulo,
            //        Id_Coordinador = a.Coordinador.Id,
            //    }));
            ListaCooperantes = new ObservableCollection<Cooperante>(
                _cooperanteRepository.GetAll()
                .OrderBy(c => c.Id)
                .ToArray());
            ListaParcialCooperantes = new ObservableCollection<Cooperante>(
                _cooperanteRepository.GetAll()
                //.GetRange(_CooperantesMostrados, _CooperantesMostrados + 4)
                //.OrderBy(c => c.Id)
                .Take(4)
                .ToArray());
            _CooperantesMostrados = _CooperantesMostrados + 4;
            
            _eventAggregator.GetEvent<ActividadEliminadaEvent>().Subscribe(OnActividadEliminadaEvent);
            _eventAggregator.GetEvent<ActividadCreadaEvent>().Subscribe(OnNuevaActividadEvent);
            _eventAggregator.GetEvent<ActividadActualizadaEvent>().Subscribe(OnActividadActualizadaEvent);
            _eventAggregator.GetEvent<CooperanteCreadoEvent>().Subscribe(PublicarCooperante);
            _eventAggregator.GetEvent<PublicarNuevaActividad>().Subscribe(OnPublicarEventosActividad);

            SelectActividadCommand = new DelegateCommand<object>(OnSelectActividadCommand);
            SelectCooperanteCommand = new DelegateCommand<Cooperante>(OnSelectCooperanteCommand);
            PruebaTemplateCommand = new DelegateCommand(OnPruebaTemplateCommandExecute);
            NuevaActividadCommand = new DelegateCommand(OnNuevaActividadCommandExecute);
            ListaDeActividadesCommand = new DelegateCommand(OnListaDeActividadesCommandExecute);
            ListaCooperantesCommand = new DelegateCommand(OnListaCooperantesCommandExecute);
            PaginaSiguienteCommand = new DelegateCommand(OnPaginaSiguienteCommandExecute);
            PaginaAnteriorCommand = new DelegateCommand(OnPaginaAnteriorCommandExecute);
            NuevoCooperanteCommand = new DelegateCommand(OnNuevoCooperanteCommandExecute);
            ActividadesPorComenzarCommand = new DelegateCommand(OnActividadesPorComenzarCommandExecute);
            ActividadesProximosVencimientosCommand = new DelegateCommand(OnActividadesProximosVencimientosCommandExecute);
            ActividadesActividadesRetrasadasCommand = new DelegateCommand(OnActividadesActividadesRetrasadasCommandExecute);
            ActividadesActividadesFinalizadasCommand = new DelegateCommand(OnActividadesActividadesFinalizadasCommandExecute);
            ActividadesActividadesActivasCommand = new DelegateCommand(OnActividadesActividadesActivasCommandExecute);
            InfoActividadCommand = new DelegateCommand<object>(OnInfoActividadCommandExecute);
            InfoActividadCommand2 = new DelegateCommand<object>(OnInfoActividadCommand2Execute);
            ListarActividadesCommand = new DelegateCommand(OnListarActividadesCommandExecute);
            BotonListarTodoCommand = new DelegateCommand<string>(OnBotonListarTodoCommandExecute);
            VerFiltroCommand = new DelegateCommand(OnVerFiltroCommandExecute);
            ResetearCheckBoxCommand = new DelegateCommand(OnResetearCheckBoxCommandExecute);
            BotonFiltarEventosCommand = new DelegateCommand(OnBotonFiltarEventosCommandExecute);
            ResetearFechaEventosCommand = new DelegateCommand(OnResetearFechaEventosCommandExecute);
            EditarActividadCommand = new DelegateCommand<object>(OnEditarActividadCommandExecute);
            EventoSelectActividadCommand = new DelegateCommand<object>(OnEventoSelectActividadCommandExecute);
            BorrarActividadCommand = new DelegateCommand<Actividad>(OnBorrarActividadCommandExecute);

            Gama.Common.Debug.Debug.StopWatch("DashboardViewModel");
        }
        private void OnPublicarEventosActividad(Evento GenerarEvento)
        {
            EventoActividad.Insert(0, GenerarEvento);
        }
        private void OnActividadEliminadaEvent(Actividad obj)
        {
            ListaParcialActividades.Remove(obj);
        }

        private void OnBorrarActividadCommandExecute(Actividad actividad)
        {
            _actividadRepository.Delete(actividad);
            _eventAggregator.GetEvent<ActividadEliminadaEvent>().Publish(actividad);

        }

        //    private void OnSelectActividadCommand(object param)
        //{
        //    var lookup = param as LookupItem;
        //    if (lookup != null) _eventAggregator.GetEvent<ActividadSeleccionadaEvent>().Publish(lookup.Id);

        //}

        private void PublicarCooperante(CooperanteWrapper CooperanteInsertado)
        {
            ListaParcialCooperantes.Clear();
            ListaParcialCooperantes.Add(CooperanteInsertado.Model);
            ListaCooperantes.Add(CooperanteInsertado.Model);
            // En la lista de Socios se muestra solo a este socio
            // En la zona de datos de socio selecionado se muestran los datos del nuevo cooperante creada
            // Se muestar zona de botones para poder editar datos del socio creado
        }
        private void OnInfoActividadCommandExecute(object param)
        {
            var lookupItem = param as LookupItem;
            if (lookupItem != null)
            {
                var Act = _actividadRepository.GetById(lookupItem.Id);
                var Coord = _cooperanteRepository.GetById(lookupItem.Id_Coordinador);
                IdActividad = Act.Id;
                FechaInicioActividad = Act.FechaDeInicio;
                FechaFinalActividad = Act.FechaDeFin;
                NombreCoordinador = Coord.Nombre;
                ApellidoCoordinador = Coord.Id;
                VisibleInfoAct = true;
                var FechaHoy = new DateTime();
                FechaHoy = DateTime.Today;
                int CompararFecha = DateTime.Compare(FechaHoy, Act.FechaDeFin);
                if (CompararFecha >= 0)
                {
                    var NumDias = (Act.FechaDeFin - FechaHoy);
                    VisibleAviso = true;
                    MensajeAviso = "Dias de Retraso";
                    //NumeroDias = -1*(NumDias.Days);
                }
            }
        }
        private void OnInfoActividadCommand2Execute(object param)
        {
            VisibleInfoAct = false;
            VisibleAviso = false;
        }
        private void OnListaCooperantesCommandExecute()
        {
            VisibleListaActividades = false;
            VisibleListaCooperantes = true;
            TituloPrincipal = "Listado de Cooperantes";
        }
        private void OnListaDeActividadesCommandExecute()
        {
            TituloPrincipal = "Listado de Actividades Disponibles";
            VisibleListaActividades = true;
            VisibleListaCooperantes = false;
            ListaDeActividades.RemoveAt(1);
        }
        private void OnNuevaActividadCommandExecute()
        {
            var o = new NuevaActividadView();
            o.ShowDialog();
            
        }
        private void OnPruebaTemplateCommandExecute()
        {
            PruebaTemplate = !PruebaTemplate;
        }

        private bool _PruebaTemplate;
        public bool PruebaTemplate
        {
            get { return _PruebaTemplate; }
            set { _PruebaTemplate = value;  OnPropertyChanged(); }
        }
        private string _TituloPrincipal="Listado de Actividades";
        public string TituloPrincipal
        {
            get { return _TituloPrincipal; }
            set { SetProperty(ref _TituloPrincipal, value); }
        }
        public ObservableCollection<Item> Items { get; private set; }
        public class Item
        {
            public string Text { get; private set; }

            public Item(string text)
            {
                this.Text = text;
            }
        }
        public CooperanteWrapper NuevoCooperante
        {
            get { return _NuevoCooperante; }
            set { SetProperty(ref _NuevoCooperante, value); }
        }
        public Cooperante CooperanteSeleccionado
        {
            get { return _CooperanteSeleccionado; }
            set
            {
                SetProperty(ref _CooperanteSeleccionado, value);
            }
        }//Copiado
        public ObservableCollection<Actividad> ListaCompletaActividades { get; private set; }
        public ObservableCollection<Actividad> ListaParcialActividades { get; private set; }
        public ObservableCollection<LookupItem> ListaDeActividades { get; private set; }
        public ObservableCollection<LookupItem> ListaDeActividadesCooperante { get; private set; }
        public ObservableCollection<LookupItem> ListaDeActividadesCoordina { get; private set; }
        public ObservableCollection<Cooperante> ListaCooperantes { get; private set; }
        public ObservableCollection<Cooperante> ListaParcialCooperantes { get; private set; }
        public ObservableCollection<Evento> EventoActividad { get; private set; }
        public ObservableCollection<Evento> EventosFiltrados { get; private set; }
        

        public ChartValues<int> ActividadesNuevasPorMes { get; private set; }
        public ChartValues<int> CooperantesNuevosPorMes { get; private set; }
        public ChartValues<int> IncidenciasNuevasPorMes { get; private set; }

        public string[] ActividadesLabels =>
            _Labels.Skip(_mesInicialActividades).ToArray();

        public string[] CooperantesLabels =>
            _Labels.Skip(_mesInicialCooperantes).ToArray();

        public ICommand SelectActividadCommand { get; set; }
        public ICommand SelectCooperanteCommand { get; set; }
        public ICommand PruebaTemplateCommand { get; set; }
        public ICommand NuevaActividadCommand { get; set; }
        public ICommand PaginaSiguienteCommand { get; set; }
        public ICommand PaginaAnteriorCommand { get; set; }
        public ICommand NuevoCooperanteCommand { get; set; }
        public ICommand ListaDeActividadesCommand { get; set; }
        public ICommand ListaCooperantesCommand { get; set; }
        public ICommand ActividadesPorComenzarCommand { get; set; }
        public ICommand ActividadesProximosVencimientosCommand { get; set; }
        public ICommand ActividadesActividadesRetrasadasCommand { get; set; }
        public ICommand ActividadesActividadesFinalizadasCommand { get; set; }
        public ICommand ActividadesActividadesActivasCommand { get; set; }
        public ICommand InfoActividadCommand { get; set; }
        public ICommand InfoActividadCommand2 { get; set; }
        public ICommand ListarActividadesCommand { get; set; }
        public ICommand BotonListarTodoCommand { get; set; }
        public ICommand VerFiltroCommand { get; set; }
        public ICommand ResetearCheckBoxCommand { get; set; }
        public ICommand BotonFiltarEventosCommand { get; set; }
        public ICommand ResetearFechaEventosCommand { get; set; }
        public ICommand EditarActividadCommand { get; set; }
        public ICommand EventoSelectActividadCommand { get; set; }
        public ICommand BorrarActividadCommand { get; set; }

        private void OnEventoSelectActividadCommandExecute(object param)
        {
            var lookup = param as Evento;

            if (lookup != null) _eventAggregator.GetEvent<ActividadSeleccionadaEvent>().Publish(lookup.Actividad.Id);
        }
        private void OnEditarActividadCommandExecute(object param)
        {
            //var o = new CrearNuevoForo();
            //var vm = (CrearNuevoForoViewModel)o.DataContext;
            //vm.Session = _Session;
            //vm.Load(Actividad);
            //o.ShowDialog();
            var lookup = param as Actividad;
            var o = new NuevaActividadView();
            var vm = (NuevaActividadViewModel)o.DataContext;
            
            vm.Load(lookup);
            o.Title = "Editar Actividad";
             o.ShowDialog();
        }
        private void OnResetearFechaEventosCommandExecute()
        {
            FechaInicioOpcion = null;
            FechaFinOpcion = null;
            EventoActividad.Clear();
            foreach(var Event in EventosFiltrados)
            {
                EventoActividad.Add(Event);
            }
        }

        private void OnBotonFiltarEventosCommandExecute()
        {
            DateTime? Inicio = FechaInicioOpcion;
            DateTime? Final = FechaFinOpcion;
            _Evento.Clear();
            if ((Inicio != null) && (Final == null))
            {
                foreach (var EventoSel in EventoActividad)
                {
                    if (EventoSel.FechaDePublicacion.Date == Inicio)
                    {
                        _Evento.Add(EventoSel);
                    } 
                }
                EventoActividad.Clear();
                foreach(var EventoValido in _Evento)
                {
                    EventoActividad.Add(EventoValido);
                }
            }
            else
            {
                if ((Inicio != null) && (Final != null))
                {
                    foreach (var EventoSel in EventoActividad)
                    {
                        if ((EventoSel.FechaDePublicacion.Date >= Inicio)&& (EventoSel.FechaDePublicacion.Date <= Final))
                        {
                            _Evento.Add(EventoSel);
                        }
                    }
                    EventoActividad.Clear();
                    foreach (var EventoValido in _Evento)
                    {
                        EventoActividad.Add(EventoValido);
                    }
                }
            }
        }
        private void OnResetearCheckBoxCommandExecute()
        {
            Enero = false;
            Febrero = false;
            Marzo = false;
            Abril = false;
            Mayo = false;
            Junio = false;
            Julio = false;
            Agosto = false;
            Septiembre = false;
            Octubre = false;
            Noviembre = false;
            Diciembre = false;
            EnCursoSeleccionado = true;
            PorComenzarSeleccionado = true;
            ProximasFechasSeleccionado = true;
            FueraPlazoSeleccionado = true;
            FinalizadasSeleccionado = true;
            OpcionYear = DateTime.Today.Year;
            ListaParcialActividades.Clear();
            EventoActividad.Clear();
            foreach(var act in ListaCompletaActividades)
            {
                ListaParcialActividades.Add(act);
            }
            foreach (var Event in EventosFiltrados)
            {
                EventoActividad.Add(Event);
            }
        }

        private void OnVerFiltroCommandExecute()
        {
            if (VisibleOpcionesListar == false) VisibleOpcionesListar = true;
            else VisibleOpcionesListar = false;
        }
        private void OnListarActividadesCommandExecute() // Mostrar el GroupBox de Opciones
        {
            if (VisibleOpcionesActividades == false) VisibleOpcionesActividades = true;
            else VisibleOpcionesActividades = false;
        }
        private void OnBotonListarTodoCommandExecute(string AListar)    // Pulsado el boton de listar todo
        {
            //_regionManager.RequestNavigate(RegionNames.ContentRegion, "ListarActividadesDataGridView");
            ActividadesOpciones.CheckBoxSeleccionados.Clear();
            ActividadesOpciones.CheckBoxMeses.Clear();
            ListaParcialActividades.Clear();
            if (EnCursoSeleccionado == true) ActividadesOpciones.CheckBoxSeleccionados.Add(true);
            else ActividadesOpciones.CheckBoxSeleccionados.Add(false);
            if (PorComenzarSeleccionado == true) ActividadesOpciones.CheckBoxSeleccionados.Add(true);
            else ActividadesOpciones.CheckBoxSeleccionados.Add(false);
            if (ProximasFechasSeleccionado == true) ActividadesOpciones.CheckBoxSeleccionados.Add(true);
            else ActividadesOpciones.CheckBoxSeleccionados.Add(false);
            if (FueraPlazoSeleccionado == true) ActividadesOpciones.CheckBoxSeleccionados.Add(true);
            else ActividadesOpciones.CheckBoxSeleccionados.Add(false);
            if (FinalizadasSeleccionado == true) ActividadesOpciones.CheckBoxSeleccionados.Add(true);
            else ActividadesOpciones.CheckBoxSeleccionados.Add(false);
            if (Enero == true) ActividadesOpciones.CheckBoxMeses.Add(true);
            else ActividadesOpciones.CheckBoxMeses.Add(false);
            if (Febrero == true) ActividadesOpciones.CheckBoxMeses.Add(true);
            else ActividadesOpciones.CheckBoxMeses.Add(false);
            if (Marzo == true) ActividadesOpciones.CheckBoxMeses.Add(true);
            else ActividadesOpciones.CheckBoxMeses.Add(false);
            if (Abril == true) ActividadesOpciones.CheckBoxMeses.Add(true);
            else ActividadesOpciones.CheckBoxMeses.Add(false);
            if (Mayo == true) ActividadesOpciones.CheckBoxMeses.Add(true);
            else ActividadesOpciones.CheckBoxMeses.Add(false);
            if (Junio == true) ActividadesOpciones.CheckBoxMeses.Add(true);
            else ActividadesOpciones.CheckBoxMeses.Add(false);
            if (Julio == true) ActividadesOpciones.CheckBoxMeses.Add(true);
            else ActividadesOpciones.CheckBoxMeses.Add(false);
            if (Agosto == true) ActividadesOpciones.CheckBoxMeses.Add(true);
            else ActividadesOpciones.CheckBoxMeses.Add(false);
            if (Septiembre == true) ActividadesOpciones.CheckBoxMeses.Add(true);
            else ActividadesOpciones.CheckBoxMeses.Add(false);
            if (Octubre == true) ActividadesOpciones.CheckBoxMeses.Add(true);
            else ActividadesOpciones.CheckBoxMeses.Add(false);
            if (Noviembre == true) ActividadesOpciones.CheckBoxMeses.Add(true);
            else ActividadesOpciones.CheckBoxMeses.Add(false);
            if (Diciembre == true) ActividadesOpciones.CheckBoxMeses.Add(true);
            else ActividadesOpciones.CheckBoxMeses.Add(false);
            string EstadosSeleccionados = "";
            int MesSelecionado = 0;
            for (int i = 0; i < 5; i++)
            {
                if ((ActividadesOpciones.CheckBoxSeleccionados[i] == true) && i == 0) EstadosSeleccionados = "Comenzado";
                if ((ActividadesOpciones.CheckBoxSeleccionados[i] == true) && i == 1) EstadosSeleccionados = "NoComenzado";
                if ((ActividadesOpciones.CheckBoxSeleccionados[i] == true) && i == 2) EstadosSeleccionados = "ProximasFinalizaciones";
                if ((ActividadesOpciones.CheckBoxSeleccionados[i] == true) && i == 3) EstadosSeleccionados = "FueraPlazo";
                if ((ActividadesOpciones.CheckBoxSeleccionados[i] == true) && i == 4) EstadosSeleccionados = "Finalizado";
                if (EstadosSeleccionados != "")
                {
                    foreach (var ActividadSeleccionada in ListaCompletaActividades)
                    {
                        if (ActividadSeleccionada.Estado.ToString() == EstadosSeleccionados)  // Si  esta seleccionadose anade de ListaParcialActividades
                        {
                            // Antes de añadirlo se comprueba si se filtra por algun mes
                            for (int j = 0; j < 11; j++)
                            {
                                if ((ActividadesOpciones.CheckBoxMeses[j] == true) && j == 0) MesSelecionado = 1;
                                if ((ActividadesOpciones.CheckBoxMeses[j] == true) && j == 1) MesSelecionado = 2;
                                if ((ActividadesOpciones.CheckBoxMeses[j] == true) && j == 2) MesSelecionado = 3;
                                if ((ActividadesOpciones.CheckBoxMeses[j] == true) && j == 3) MesSelecionado = 4;
                                if ((ActividadesOpciones.CheckBoxMeses[j] == true) && j == 4) MesSelecionado = 5;
                                if ((ActividadesOpciones.CheckBoxMeses[j] == true) && j == 5) MesSelecionado = 6;
                                if ((ActividadesOpciones.CheckBoxMeses[j] == true) && j == 6) MesSelecionado = 7;
                                if ((ActividadesOpciones.CheckBoxMeses[j] == true) && j == 7) MesSelecionado = 8;
                                if ((ActividadesOpciones.CheckBoxMeses[j] == true) && j == 8) MesSelecionado = 9;
                                if ((ActividadesOpciones.CheckBoxMeses[j] == true) && j == 9) MesSelecionado = 10;
                                if ((ActividadesOpciones.CheckBoxMeses[j] == true) && j == 10) MesSelecionado = 11;
                                if ((ActividadesOpciones.CheckBoxMeses[j] == true) && j == 11) MesSelecionado = 12;
                                if (ActividadSeleccionada.FechaDeFin.Month == MesSelecionado)
                                {
                                    ListaParcialActividades.Add(ActividadSeleccionada);
                                    var Coord = _cooperanteRepository.GetById(ActividadSeleccionada.Coordinador.Id);
                                    string NombreCompleto = Coord.Nombre + Coord.Apellido;
                                    j = 0;
                                    break;
                                }
                            }
                            if (MesSelecionado == 0) // AL salirdel for no hay mes seleccionado se muestra la actividad
                            {
                                ListaParcialActividades.Add(ActividadSeleccionada);
                                var Coord = _cooperanteRepository.GetById(ActividadSeleccionada.Coordinador.Id);
                                string NombreCompleto = Coord.Nombre + Coord.Apellido;
                            }
                           
                        }
                    }
                    EstadosSeleccionados = "";
                    MesSelecionado = 0;
                }
            }
            // En los eventos tendria que mostrar solo los eventos de las actividades mostradas.
            EventoActividad.Clear();
            foreach (var Act in ListaParcialActividades)
            {
                foreach (var Event in Act.Eventos)
                {
                    EventoActividad.Add(Event);
                }
            }
        }
        private void OnActividadesActividadesActivasCommandExecute()
        {
            ListaDeActividades.Clear();
            foreach (var act in ListaCompletaActividades)
            {
                if (act.Estado.ToString() == "Comenzado")
                {
                    var ActividadComenzada = new LookupItem()
                    {
                        Id = act.Id,

                        DisplayMember1 = act.Titulo,
                        Id_Coordinador = act.Coordinador.Id,
                        FechaDeInicioActividad = act.FechaDeInicio,
                    };
                    ListaDeActividades.Add(ActividadComenzada);
                }
            }
        }
        private void OnActividadesPorComenzarCommandExecute()
        {
            ListaDeActividades.Clear();
            foreach(var act in ListaCompletaActividades)
            {
                if (act.Estado.ToString() == "NoComenzado")
                {
                    var ActividadNoComenzada = new LookupItem()
                    {
                        Id = act.Id,

                        DisplayMember1 = act.Titulo,
                        Id_Coordinador = act.Coordinador.Id,
                        FechaDeInicioActividad = act.FechaDeInicio,
                    };
                    ListaDeActividades.Add(ActividadNoComenzada);
                }
            }
        }
        private void OnActividadesProximosVencimientosCommandExecute()
        {
            ListaDeActividades.Clear();
            foreach (var act in ListaCompletaActividades)
            {
                if (act.Estado.ToString() == "ProximasFinalizaciones")
                {
                    var ActividadProximaVencimiento = new LookupItem()
                    {
                        Id = act.Id,

                        DisplayMember1 = act.Titulo,
                        Id_Coordinador = act.Coordinador.Id,
                        FechaDeInicioActividad = act.FechaDeInicio,
                    };
                    ListaDeActividades.Add(ActividadProximaVencimiento);
                }
            }
        }
        private void OnActividadesActividadesRetrasadasCommandExecute()
        {
            ListaDeActividades.Clear();
            foreach (var act in ListaCompletaActividades)
            {
                if (act.Estado.ToString() == "FueraPlazo")
                {
                    var ActividadFueraPlazo = new LookupItem()
                    {
                        Id = act.Id,

                        DisplayMember1 = act.Titulo,
                        Id_Coordinador = act.Coordinador.Id,
                        FechaDeInicioActividad = act.FechaDeInicio,
                    };
                    ListaDeActividades.Add(ActividadFueraPlazo);
                }
            }
        }
        private void OnActividadesActividadesFinalizadasCommandExecute()
        {
            ListaDeActividades.Clear();
            foreach (var act in ListaCompletaActividades)
            {
                if (act.Estado.ToString() == "Finalizado")
                {
                    var ActividadFinalizada = new LookupItem()
                    {
                        Id = act.Id,

                        DisplayMember1 = act.Titulo,
                        Id_Coordinador = act.Coordinador.Id,
                        FechaDeInicioActividad = act.FechaDeInicio,
                    };
                    ListaDeActividades.Add(ActividadFinalizada);
                }
            }
        }
        private void OnNuevoCooperanteCommandExecute()
        {
            var o = new AgregarCooperanteView();    // Esta es la vista

            o.ShowDialog();
        }

        private void OnPaginaSiguienteCommandExecute()
        {
            OpcionYear = OpcionYear + 1;
        }
        private void OnPaginaAnteriorCommandExecute()
        {
            OpcionYear = OpcionYear - 1;
        }

        private void OnSelectCooperanteCommand(Cooperante obj)
        {
            //ListaCooperantes.Clear();
            //ListaCooperantes.Add(obj);
            //ListaCooperantes = ListaParcialCooperantes;
            VisibleListaActividadesCooperante = true;
            VisibleDatosCooperanteSeleccionado = true;
            VisibleImagenSeleccionCooperante = false;
            VisibleListaTodosCooperantes = false;
            VisibleCooperanteSeleccionado = true;
            CooperanteSeleccionado = ListaCooperantes.Where(x => x.Id == obj.Id).FirstOrDefault();
            ListaParcialCooperantes.Clear();
            ListaParcialCooperantes.Add(CooperanteSeleccionado);
            ListaDeActividadesCoordina.Clear();
            ListaDeActividadesCooperante.Clear();
            foreach (var actividadCoordina in ListaDeActividades)
            {
                if (actividadCoordina.Id_Coordinador == obj.Id)
                {
                    ListaDeActividadesCoordina.Add(actividadCoordina);
                }
            }
           foreach (var actividadCoopera in ListaCompletaActividades)
            {
                foreach (var CooperanteActividadCoopera in actividadCoopera.Cooperantes)
                {
                    if (CooperanteActividadCoopera.Id == obj.Id)
                    {
                        var ItemCooperante = new LookupItem()
                        {
                            Id = actividadCoopera.Id,
                            DisplayMember1 = actividadCoopera.Titulo,
                            Id_Coordinador = actividadCoopera.Coordinador.Id,
                        };
                        ListaDeActividadesCooperante.Add(ItemCooperante);
                    }
                }
            }
        }

        private void OnSelectActividadCommand(object param)
        {
            var lookup = param as Actividad;
            if (lookup != null)  _eventAggregator.GetEvent<ActividadSeleccionadaEvent>().Publish(lookup.Id);
        }

        private void OnNuevaActividadEvent(int id)
        {
            var actividad = _actividadRepository.GetById(id);
            ListaParcialActividades.Add(actividad);
        }
        

        private void OnActividadActualizadaEvent(int id)
        {
            var actividadActualizada = _actividadRepository.GetById(id);
            if (ListaParcialActividades.Any(a => a.Id == id))
            {
                var indice = ListaParcialActividades.IndexOf(ListaParcialActividades.Single(a => a.Id == id));
                ListaParcialActividades.RemoveAt(indice);
                ListaParcialActividades.Insert(0,actividadActualizada);
                
            }
        }
        public bool VisibleInfoAct
        {
            get { return _VisibleInfoAct; }
            set { SetProperty(ref _VisibleInfoAct, value); }
        }
        public bool VisibleListaActividades
        {
            get { return _VisibleListaActividades; }
            set { SetProperty(ref _VisibleListaActividades, value); }
        }
        public bool VisibleListaActividadesCooperante
        {
            get { return _VisibleListaActividadesCooperante; }
            set { SetProperty(ref _VisibleListaActividadesCooperante, value); }
        }//Copiado
        public bool VisibleListaCooperantes
        {
            get { return _VisibleListaCooperantes; }
            set { SetProperty(ref _VisibleListaCooperantes, value); }
        }
        public bool VisibleDatosDNI
        {
            get { return _VisibleDatosDNI; }
            set { SetProperty(ref _VisibleDatosDNI, value); }
        }
        public bool VisibleOpcionesListar
        {
            get { return _VisibleOpcionesListar; }
            set { SetProperty(ref _VisibleOpcionesListar, value); }
        }
        public bool VisibleContacto
        {
            get { return _VisibleContacto; }
            set { SetProperty(ref _VisibleContacto, value); }
        }
        public bool VisibleDireccion
        {
            get { return _VisibleDireccion; }
            set { SetProperty(ref _VisibleDireccion, value); }
        }
        public bool VisibleCooperanteSeleccionado
        {
            get { return _VisibleCooperanteSeleccionado; }
            set { SetProperty(ref _VisibleCooperanteSeleccionado, value); }
        } // Copiado
        public bool VisibleAviso
        {
            get { return _VisibleAviso; }
            set { SetProperty(ref _VisibleAviso, value); }
        }
        public bool EnCursoSeleccionado
        {
            get { return _EnCursoSeleccionado; }
            set { SetProperty(ref _EnCursoSeleccionado, value); }
        }
        public bool PorComenzarSeleccionado
        {
            get { return _PorComenzarSeleccionado; }
            set { SetProperty(ref _PorComenzarSeleccionado, value); }
        }
        public bool ProximasFechasSeleccionado
        {
            get { return _ProximasFechasSeleccionado; }
            set { SetProperty(ref _ProximasFechasSeleccionado, value); }
        }
        public bool FueraPlazoSeleccionado
        {
            get { return _FueraPlazoSeleccionado; }
            set { SetProperty(ref _FueraPlazoSeleccionado, value); }
        }
        public bool FinalizadasSeleccionado
        {
            get { return _FinalizadasSeleccionadas; }
            set { SetProperty(ref _FinalizadasSeleccionadas, value); }
        }
        public bool VisibleListaTodosCooperantes
        {
            get { return _VisibleListaTodosCooperantes; }
            set { SetProperty(ref _VisibleListaTodosCooperantes, value); }
        }// copiado
        public bool VisibleImagenSeleccionCooperante
        {
            get { return _VisibleImagenSeleccionCooperante; }
            set { SetProperty(ref _VisibleImagenSeleccionCooperante, value); }
        }// Copiado
        public bool VisibleDatosCooperanteSeleccionado
        {
            get { return _VisibleDatosCooperanteSeleccionado; }
            set { SetProperty(ref _VisibleDatosCooperanteSeleccionado, value); }
        }
            public bool VisibleOpcionesActividades
        {
            get { return _VisibleOpcionesActividades; }
            set { SetProperty(ref _VisibleOpcionesActividades, value); }
        }
        public int EnCursoCount
        {
            get { return _EnCursoCount; }
            set { SetProperty(ref _EnCursoCount, value); }
        }
        public int NoComenzado
        {
            get { return _NoComenzado; }
            set { SetProperty(ref _NoComenzado, value); }
        }
        public int Finalizado
        {
            get { return _Finalizado; }
            set { SetProperty(ref _Finalizado, value); }
        }
        public int ProximasFinalizaciones
        {
            get { return _ProximasFinalizaciones; }
            set { SetProperty(ref _ProximasFinalizaciones, value); }
        }
        public int FueraPlazo
        {
            get { return _FueraPlazo; }
            set { SetProperty(ref _FueraPlazo, value); }
        }
        public int IdActividad
        {
            get { return _IdActividad; }
            set { SetProperty(ref _IdActividad, value); }
        }
        public DateTime FechaInicioActividad
        {
            get { return _FechaInicioActividad; }
            set { SetProperty(ref _FechaInicioActividad, value); }
        }
        public DateTime FechaFinalActividad
        {
            get { return _FechaFinalActividad; }
            set { SetProperty(ref _FechaFinalActividad, value); }
        }
        public string NombreCoordinador
        {
            get { return _NombreCoordinador; }
            set { SetProperty(ref _NombreCoordinador, value); }
        }
        public int ApellidoCoordinador
        {
            get { return _ApellidoCoordinador; }
            set { SetProperty(ref _ApellidoCoordinador, value); }
        }
        public string MensajeAviso
        {
            get { return _MensajeAviso; }
            set { SetProperty(ref _MensajeAviso, value); }
        }
        public int OpcionYear
        {
            get { return _OpcionYear; }
            set { SetProperty(ref _OpcionYear, value); }
        }
        public bool Enero
        {
            get { return _Enero; }
            set { SetProperty(ref _Enero, value); }
        }
        public bool Febrero
        {
            get { return _Febrero; }
            set { SetProperty(ref _Febrero, value); }
        }
        public bool Marzo
        {
            get { return _Marzo; }
            set { SetProperty(ref _Marzo, value); }
        }
        public bool Abril
        {
            get { return _Abril; }
            set { SetProperty(ref _Abril, value); }
        }
        public bool Mayo
        {
            get { return _Mayo; }
            set { SetProperty(ref _Mayo, value); }
        }
        public bool Junio
        {
            get { return _Junio; }
            set { SetProperty(ref _Junio, value); }
        }
        public bool Julio
        {
            get { return _Julio; }
            set { SetProperty(ref _Julio, value); }
        }
        public bool Agosto
        {
            get { return _Agosto; }
            set { SetProperty(ref _Agosto, value); }
        }
        public bool Septiembre
        {
            get { return _Septiembre; }
            set { SetProperty(ref _Septiembre, value); }
        }
        public bool Octubre
        {
            get { return _Octubre; }
            set { SetProperty(ref _Octubre, value); }
        }
        public bool Noviembre
        {
            get { return _Noviembre; }
            set { SetProperty(ref _Noviembre, value); }
        }
        public bool Diciembre
        {
            get { return _Diciembre; }
            set { SetProperty(ref _Diciembre, value); }
        }
        private DateTime? _FechaInicioOpcion;
        public DateTime? FechaInicioOpcion
        {
            get { return _FechaInicioOpcion; }
            set { SetProperty(ref _FechaInicioOpcion, value);}
        }

        private DateTime? _FechaFinOpcion;
        public DateTime? FechaFinOpcion
        {
            get { return _FechaFinOpcion; }
            set { SetProperty(ref _FechaFinOpcion, value);}
        }
        private void FiltrarPorFecha()
        {
            var fechaDeInicio = FechaInicioOpcion;
            var fechaDeFin = FechaFinOpcion;
            if ((fechaDeInicio != null) && (fechaDeFin == null)) // Listar solo lo del dia seleccionado
            {
                EventoActividad.Clear();
                foreach (var EventoSel in EventosFiltrados)
                {
                    //int CompararFecha = DateTime.Compare((DateTime)fechaDeInicio, EventoSel.FechaDePublicacion); // Comprabando si comenzo el proyecto
                    //if (CompararFecha == 0) EventoActividad.Add(EventoSel);
                   
                }
            }
            //EventoActividad.Clear();
            //OnPropertyChanged(nameof(EventoActividad));
        }
    }
}



//private int _FueraPlazo;