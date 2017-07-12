using Core;
using FluentNHibernate.Infrastructure; // Añadido prueba foro
using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Views;
using Gama.Cooperacion.Wpf.Wrappers;
using Microsoft.Practices.Unity;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class TareasDeActividadViewModel : ViewModelBase
    {
        private IActividadRepository _actividadRepository;

        private ActividadWrapper _Actividad;
        private CooperanteWrapper _ResponsableTarea;
        private ISession _Session;
        private ITareaRepository _TareaRepository;
        private IForoRepository _ForoRepository;
        private IEventAggregator _EventAggregator;
        private bool _VisibleTareasFinalizadas;
        private bool _VisibleFiltroEventoTarea;
        private bool _VisibleEventosTarea;
        private DateTime? _FechaInicioOpcion;
        private DateTime? _FechaFinOpcion;
        private List<Evento> _Evento;
        public TareasDeActividadViewModel(
            IActividadRepository actividadRepository,
            ICooperanteRepository cooperabteRepository,
            IForoRepository foroRepository,
            ITareaRepository tareaRepository,
            IEventAggregator eventAggregator) 
        {
            Gama.Common.Debug.Debug.StartWatch();
            _VisibleTareasFinalizadas = false;
            _VisibleFiltroEventoTarea = false;
            _VisibleEventosTarea = true;
            _actividadRepository = actividadRepository;
            _ForoRepository = foroRepository;
            _TareaRepository = tareaRepository;
            _EventAggregator = eventAggregator;
            
            ForosDisponibles = new ObservableCollection<ForoWrapper>();
            TareasDisponibles = new ObservableCollection<TareaWrapper>();
            TareasDisponiblesAux = new ObservableCollection<TareaWrapper>();
            EventoActividad = new ObservableCollection<Evento>();
            EventoActividadAux = new ObservableCollection<Evento>();
            TareasFinalizadas = new ObservableCollection<TareaWrapper>();
            CooperantesSeleccionados = new ObservableCollection<CooperanteWrapper>();
            _Evento = new List<Evento>();

            _EventAggregator.GetEvent<CargarNuevaActividadEvent>().Subscribe(OnCargarNuevaActividadEvent);
            _EventAggregator.GetEvent<NuevoForoCreadoEvent>().Subscribe(OnNuevoForoCreadoEvent);
            _EventAggregator.GetEvent<NuevaTareaCreadaEvent>().Subscribe(OnNuevaTareaCreadaEvent);
            _EventAggregator.GetEvent<PublicarEventosActividad>().Subscribe(OnPublicarEventosActividad);
            _EventAggregator.GetEvent<TareaModificadaEvent>().Subscribe(OnTareaModificadaEvent);

            BorrarTareaCommand = new DelegateCommand<object>(OnBorrarTareaCommand, OnBorrarTareaCommand_CanExecute);
            BotonFiltarEventosCommand = new DelegateCommand(OnBotonFiltarEventosCommandExecute);
            CargarTareasDisponibles = new DelegateCommand(OnCargarTareasDisponibles);
            CargarTareasFinalizadas = new DelegateCommand(OnCargarTareasFinalizadas);
            CrearForoCommand = new DelegateCommand(OnCrearForoCommand, OnCrearForoCommand_CanExecute);
            CrearTareaCommand = new DelegateCommand(OnCrearTareaCommand, OnCrearTareaCommand_CanExecute);
            EditarTareaCommand = new DelegateCommand<object>(OnEditarTareaCommand, OnEditarTareaCommand_CanExecute);
            FinalizarTareaTDCommand = new DelegateCommand<TareaWrapper>(OnFinalizarTareaTDCommand, OnFinalizarTareaTDCommand_CanExecute);
            InfoTareaCommand = new DelegateCommand<object>(OnInfoTareaCommand, OnInfoTareaCommand_CanExecute);
            MensajesForoCommand = new DelegateCommand<object>(OnMensajeForoCommand, OnMensajeForoCommand_CanExecute);
            MostarFiltroEventoTarea = new DelegateCommand(OnMostarFiltroEventoTarea);
            RecuperarTareaCommand = new DelegateCommand<TareaWrapper>(OnRecuperarTareaCommand);
            ResetearFechaEventosCommand = new DelegateCommand(OnResetearFechaEventosCommandExecute);
            SeguimientoTareaCommand = new DelegateCommand<object>(OnSeguimientoTareaCommand, OnSeguimientoTareaCommand_CanExecute);
            VerEventosTareaCommand = new DelegateCommand(OnVerEventosTareaCommand);
            AñadirCooperantesComboBox = new DelegateCommand (OnAñadirCooperantesComboBox, OnAñadirCooperantesComboBox_CanExecute);
          
            
            Gama.Common.Debug.Debug.StopWatch("TareasDeActividadViewModel");
        }
        //--------------------------------
        //Contenedores (ObservableCollections, lists,...
        //--------------------------------
        public ObservableCollection<ForoWrapper> ForosDisponibles { get; private set; }
        public ObservableCollection<Evento> EventoActividad { get; private set; }
        public ObservableCollection<Evento> EventoActividadAux { get; private set; }
        public ObservableCollection<TareaWrapper> TareasDisponibles { get; private set; }
        public ObservableCollection<TareaWrapper> TareasDisponiblesAux { get; private set; }
        public ObservableCollection<TareaWrapper> TareasFinalizadas { get; private set; }
        public ObservableCollection<CooperanteWrapper> CooperantesSeleccionados { get; private set; }
        //--------------------------------
        // ICommands
        //--------------------------------
        public ICommand FinalizarTareaTDCommand { get; set; }
        public ICommand MensajesForoCommand { get; private set; }
        public ICommand InfoTareaCommand { get; private set; }
        public ICommand SeguimientoTareaCommand { get; private set; }
        public ICommand EditarTareaCommand { get; private set; }
        public ICommand BorrarTareaCommand { get; private set; }
        public ICommand CrearForoCommand { get; private set; }
        public ICommand CrearTareaCommand { get; private set; }
        public ICommand AñadirCooperantesComboBox { get; private set; }
        public ICommand CargarTareasFinalizadas { get; set; }
        public ICommand CargarTareasDisponibles { get; set; }
        public ICommand RecuperarTareaCommand { get; set; }
        public ICommand MostarFiltroEventoTarea { get; set; }
        public ICommand VerEventosTareaCommand { get; set; }
        public ICommand BotonFiltarEventosCommand { get; set; }
        public ICommand ResetearFechaEventosCommand { get; set; }

        //-----------------------------------
        // ICommands Implementaciones
        //-----------------------------------
        private void OnBorrarTareaCommand(object wrapper)
        {
            var tareaBorrada = Actividad.Tareas.Where(x => x.Id == ((TareaWrapper)wrapper).Id).First();
            Actividad.Tareas.Remove(tareaBorrada);
            _TareaRepository.Delete(tareaBorrada.Model);
            Actividad.AcceptChanges();
            var todasLasTareas = Actividad.Tareas.ToList();
            todasLasTareas.Remove(tareaBorrada);
            TareasDisponibles.Clear();
            TareasDisponibles.AddRange(todasLasTareas);
            OnPropertyChanged(nameof(TareasDisponibles)); // No se si hace falta
        }
        private void OnResetearFechaEventosCommandExecute()
        {
            FechaInicioOpcion = null;
            FechaFinOpcion = null;
            EventoActividad.Clear();
            foreach (var Event in EventoActividadAux)
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
                        _Evento.Add(EventoSel);
                }
                EventoActividad.Clear();
                EventoActividad.AddRange(_Evento);
            }
            else
            {
                if ((Inicio != null) && (Final != null))
                {
                    foreach (var EventoSel in EventoActividad)
                    {
                        if ((EventoSel.FechaDePublicacion.Date >= Inicio) && (EventoSel.FechaDePublicacion.Date <= Final))
                            _Evento.Add(EventoSel);
                    }
                    EventoActividad.Clear();
                    EventoActividad.AddRange(_Evento);
                }
            }
        }
        private void OnVerEventosTareaCommand()
        {
            if (VisibleEventosTarea == false) VisibleEventosTarea = true;
            else VisibleEventosTarea = false;
        }
        private void OnMostarFiltroEventoTarea()
        {
            if (VisibleFiltroEventoTarea == false) VisibleFiltroEventoTarea = true;
            else VisibleFiltroEventoTarea = false;
        }
        private void OnCargarTareasDisponibles()
        {
            if (VisibleTareasFinalizadas == false) VisibleTareasFinalizadas = true;
            else VisibleTareasFinalizadas = false;
        }
        private void OnCargarTareasFinalizadas()
        {
            if (VisibleTareasFinalizadas == false) VisibleTareasFinalizadas = true;
            else VisibleTareasFinalizadas = false;
        }
        private void OnAñadirCooperantesComboBox()
        {
            foreach (var cooperante in Actividad.Cooperantes)
            {
                var cooperNuevo = CooperantesSeleccionados.Where(c=>c.Id==cooperante.Id).First();
                if (cooperNuevo.Nombre != null)
                {
                    CooperantesSeleccionados.Add(new CooperanteWrapper(
                    new Cooperante()
                    {
                        Id = cooperNuevo.Id,
                        Dni = cooperNuevo.Dni,
                        Nombre = cooperNuevo.Nombre,
                        Apellido = cooperNuevo.Apellido
                    }));
                }
            }
        }
        private void OnMensajeForoCommand(object wrapper)
        {
            var o = new MensajesForoView();
            var vm = (MensajesForoViewModel)o.DataContext;
            vm.Session = _Session;         
            vm.LoadActividad(Actividad);   
            vm.LoadForo(((ForoWrapper)wrapper));
            o.ShowDialog();
        }
        private void OnEditarTareaCommand(object wrapper)
        {
            var o = new CrearNuevaTareaView();  // Llama al constructor de CrearNuevaTareaVM y ejecuta la instruccion
                                                // Tarea = new TareaWrapper(new Tarea()); Para que en el TareaWrapper
                                                // Responsable no puede ser nulo con lo que este llamada dara error porque
                                                // El responsable es nulo
            var vm = (CrearNuevaTareaViewModel)o.DataContext;
            vm.Session = _Session;
            vm.LoadActividad(Actividad);
            vm.LoadTarea(((TareaWrapper)wrapper));
            o.ShowDialog();
        }
        private void OnInfoTareaCommand(object wrapper)
        {
            var o = new InformacionTareaView();
            var vm = (InformacionTareaViewModel)o.DataContext;
            vm.Session = _Session;          // Da error si en InformacionTareaViewModel no tenemos definido esta variable
            vm.LoadActividad(Actividad);    //Da error si no tenemos implemetada esta funcion en InformacionTareaViewModel
            vm.LoadTarea(((TareaWrapper)wrapper));  // necesito los detalles de la tarea concreta seleccionada
            o.ShowDialog();
        }
        private void OnSeguimientoTareaCommand(object wrapper)
        {
            var o = new SeguimientoTareaView();
            var vm = (SeguimientoTareaViewModel)o.DataContext;
            vm.Session = _Session;
            vm.LoadActividad(Actividad);
            vm.LoadTarea(((TareaWrapper)wrapper)); 
            o.ShowDialog();
        }
        private void OnFinalizarTareaTDCommand(TareaWrapper wrapper)
        {
            TareasFinalizadas.Insert(0,wrapper);
            TareasDisponibles.Remove(wrapper);
            TareasDisponiblesAux.Remove(wrapper);
            Actividad.Tareas.Where(ident => ident.Id == wrapper.Id).First().HaFinalizado = true;
            Actividad.UpdatedAt = DateTime.Now;
            _actividadRepository.Update(Actividad.Model);
            Actividad.AcceptChanges();
            _EventAggregator.GetEvent<TareaModificadaEvent>().Publish(wrapper.Id);
            AuxiliarOnPublicarEventosActividad(Ocurrencia.TAREA_FINALIZADA, ((TareaWrapper)wrapper).Descripcion);
        }
        private void OnRecuperarTareaCommand(TareaWrapper wrapper)
        {
            TareasDisponibles.Insert(0, wrapper);
            TareasFinalizadas.Remove(wrapper);
            TareasDisponiblesAux.Add(wrapper);
            Actividad.Tareas.Where(ident => ident.Id == wrapper.Id).First().HaFinalizado = false;
            Actividad.UpdatedAt = DateTime.Now;
            _actividadRepository.Update(Actividad.Model);
            Actividad.AcceptChanges();
            _EventAggregator.GetEvent<TareaModificadaEvent>().Publish(wrapper.Id);
            AuxiliarOnPublicarEventosActividad(Ocurrencia.TAREA_RECUPERADA, ((TareaWrapper)wrapper).Descripcion);
        }
        private void OnCrearForoCommand()
        {
            var o = new CrearNuevoForo();
            var vm = (CrearNuevoForoViewModel)o.DataContext;
            vm.Session = _Session;
            vm.Load(Actividad);
            o.ShowDialog();
        }
        private void OnCrearTareaCommand()
        {
            var o = new CrearNuevaTareaView();
            var vm = (CrearNuevaTareaViewModel)o.DataContext;
            vm.Session = _Session;
            vm.LoadActividad(Actividad);
            o.ShowDialog();
        }

        //-------------------------------
        // ICommand_CanExecute
        //-------------------------------
        private bool OnFinalizarTareaTDCommand_CanExecute(object wrapper)
        {
            return true;
        }
        private bool OnMensajeForoCommand_CanExecute(object wrapper)
        {
            return true;
        }
        private bool OnInfoTareaCommand_CanExecute(object wrapper)
        {
            return true;
        }
        private bool OnSeguimientoTareaCommand_CanExecute(object wrapper)
        {
            return true;
        }
        private bool OnEditarTareaCommand_CanExecute(object wrapper)
        {
            return true;
        }
        private bool OnBorrarTareaCommand_CanExecute(object wrapper)
        {
            return true;
        }
        private bool OnAñadirCooperantesComboBox_CanExecute()
        {
            return true;
        }
        private bool OnCrearTareaCommand_CanExecute()
        {
            return true;
        }
        private bool OnCrearForoCommand_CanExecute()
        {
            return true;
        }
        //-----------------------------------
        // Events
        //-----------------------------------
        private void OnCargarNuevaActividadEvent(int id)
        {
            ForosDisponibles.Clear();
            TareasDisponibles.Clear();
            TareasFinalizadas.Clear();
            EventoActividad.Clear();
            EventoActividadAux.Clear();
            var actividad = _actividadRepository.GetById(id); // actividad contiene la información de la base de datos
            foreach (var forito in actividad.Foros)
            {
                ForosDisponibles.Add(new ForoWrapper(
                    new Foro()
                    {
                        Id = forito.Id,
                        Titulo = forito.Titulo,
                        FechaDePublicacion = forito.FechaDePublicacion,
                        Mensajes = forito.Mensajes
                    })
                { ForoVisible = false });
            }
            // Recorrido para obtener las Tareas creadas en esta actividad
            foreach (var tarea in actividad.Tareas)
            {
                if (tarea.HaFinalizado == false)
                {
                    TareasDisponibles.Add(new TareaWrapper(
                    new Tarea()
                    {
                        Id = tarea.Id,
                        Descripcion = LookupItem.ShortenStringForDisplay(tarea.Descripcion, 100),
                        //Descripcion = tarea.Descripcion,
                        FechaDeFinalizacion = tarea.FechaDeFinalizacion,
                        HaFinalizado = tarea.HaFinalizado,
                        Seguimiento = tarea.Seguimiento,
                        Incidencias = tarea.Incidencias,
                        Responsable = tarea.Responsable,
                    })
                    { SeguimientoVisible = false });
                    TareasDisponiblesAux.Add(new TareaWrapper(
                    new Tarea()
                    {
                        Id = tarea.Id,
                        Descripcion = LookupItem.ShortenStringForDisplay(tarea.Descripcion, 100),
                        //Descripcion = tarea.Descripcion,
                        FechaDeFinalizacion = tarea.FechaDeFinalizacion,
                        HaFinalizado = tarea.HaFinalizado,
                        Seguimiento = tarea.Seguimiento,
                        Incidencias = tarea.Incidencias,
                        Responsable = tarea.Responsable,
                    })
                    { SeguimientoVisible = false });
                }
                else
                {
                    TareasFinalizadas.Add(new TareaWrapper(
                    new Tarea()
                    {
                        Id = tarea.Id,
                        Descripcion = LookupItem.ShortenStringForDisplay(tarea.Descripcion, 100),
                        FechaDeFinalizacion = tarea.FechaDeFinalizacion,
                        HaFinalizado = tarea.HaFinalizado,
                        Seguimiento = tarea.Seguimiento,
                        Incidencias = tarea.Incidencias,
                        Responsable = tarea.Responsable,
                    })
                    { SeguimientoVisible = false });
                }

            }
            // Recorrido para obtener los eventos publicados en la Actividad cargada
            foreach (var evento in actividad.Eventos)
            {
                EventoActividad.Add(evento);
                EventoActividadAux.Add(evento);
            }
        }
        private void OnNuevoForoCreadoEvent(ForoWrapper forito)
        {
            ForosDisponibles.Insert(0, (new ForoWrapper(new Foro()
            { Id = forito.Id, Titulo = forito.Titulo, FechaDePublicacion = forito.FechaDePublicacion, Mensajes = forito.Model.Mensajes })
            { ForoVisible = false }));
        }
        private void OnNuevaTareaCreadaEvent(TareaWrapper NuevaTarea)
        {
            TareasDisponibles.Insert(0, (new TareaWrapper(new Tarea()
            {
                Id = NuevaTarea.Id,
                Descripcion = NuevaTarea.Descripcion,
                FechaDeFinalizacion = NuevaTarea.FechaDeFinalizacion,
                HaFinalizado = NuevaTarea.HaFinalizado,
                Responsable = NuevaTarea.Responsable.Model,
                Seguimiento = NuevaTarea.Model.Seguimiento,
                Incidencias = NuevaTarea.Model.Incidencias
            })
            { SeguimientoVisible = false }
            ));
            TareasDisponiblesAux.Insert(0, (new TareaWrapper(new Tarea()
            {
                Id = NuevaTarea.Id,
                Descripcion = NuevaTarea.Descripcion,
                FechaDeFinalizacion = NuevaTarea.FechaDeFinalizacion,
                HaFinalizado = NuevaTarea.HaFinalizado,
                Responsable = NuevaTarea.Responsable.Model,
                Seguimiento = NuevaTarea.Model.Seguimiento,
                Incidencias = NuevaTarea.Model.Incidencias
            })
            { SeguimientoVisible = false }
            ));
        }
        private void AuxiliarOnPublicarEventosActividad(Ocurrencia tipoEvento, string tituloEvento)
        {
            var eventoDeActividad = new Evento()
            {
                FechaDePublicacion = DateTime.Now,
                Titulo = tituloEvento,
                Ocurrencia = tipoEvento.ToString(),
            };
            _EventAggregator.GetEvent<PublicarEventosActividad>().Publish(eventoDeActividad);
        }
        private void OnTareaModificadaEvent(int idTarea)
        {
            var tarea = _TareaRepository.GetById(idTarea);
            if (TareasDisponibles.Any(a => a.Id == idTarea))
            {
                var indice = TareasDisponibles.IndexOf(TareasDisponibles.Single(a => a.Id == idTarea));
                TareasDisponibles.Insert(indice, (new TareaWrapper(new Tarea()
                {
                    Id = tarea.Id,
                    Descripcion = tarea.Descripcion,
                    FechaDeFinalizacion = tarea.FechaDeFinalizacion,
                    HaFinalizado = tarea.HaFinalizado,
                    Responsable = tarea.Responsable,
                    Seguimiento = tarea.Seguimiento,
                    Incidencias = tarea.Incidencias
                })
                { SeguimientoVisible = false }
            ));
                TareasDisponibles.RemoveAt(indice + 1);
            }
        }
        private void OnPublicarEventosActividad(Evento GenerarEvento)
        {
            EventoActividad.Insert(0, GenerarEvento);
            Actividad.Model.AddEvento(GenerarEvento);
            _actividadRepository.Update(Actividad.Model);
        }

        //-----------------------------------
        // Bindings - Public
        //-----------------------------------
        public ActividadWrapper Actividad
        {
            get { return _Actividad; }
            set { SetProperty(ref _Actividad, value); }
        }
        public ISession Session
        {
            get { return _Session; }
            set
            {
                _Session = value;
                _actividadRepository.Session = _Session;
                _ForoRepository.Session = _Session;
                _TareaRepository.Session = _Session;
            }
        }
        public CooperanteWrapper ResponsableTarea
        {
            get { return _ResponsableTarea; }
            set
            {
                ((DelegateCommand)AñadirCooperantesComboBox).RaiseCanExecuteChanged(); // Para que es
                SetProperty(ref _ResponsableTarea, value);
            }
        }
        public DateTime? FechaFinOpcion
        {
            get { return _FechaFinOpcion; }
            set { SetProperty(ref _FechaFinOpcion, value); }
        }
        public DateTime? FechaInicioOpcion
        {
            get { return _FechaInicioOpcion; }
            set { SetProperty(ref _FechaInicioOpcion, value); }
        }

        //------------------------------------
        // Visibility
        //------------------------------------
        public bool VisibleEventosTarea
        {
            get { return _VisibleEventosTarea; }
            set { SetProperty(ref _VisibleEventosTarea, value); }
        }
        public bool VisibleFiltroEventoTarea
        {
            get { return _VisibleFiltroEventoTarea; }
            set { SetProperty(ref _VisibleFiltroEventoTarea, value); }
        }
        public bool VisibleTareasFinalizadas
        {
            get { return _VisibleTareasFinalizadas; }
            set { SetProperty(ref _VisibleTareasFinalizadas, value); }
        }

        //------------------------------------
        // Procedimientos y Funciones Auxiliares
        //------------------------------------
        public void LoadActividad(ActividadWrapper wrapper)
        {
            Actividad = wrapper;
            Actividad.AcceptChanges();
            OnCargarNuevaActividadEvent(Actividad.Id);
        }

    }   // Clase TareasDeActividadVM
}       // Fin namespace
