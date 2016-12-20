using Core;
using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
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
        private int _CantidadMensajes;
        private string _tituloForo;
        private string _tituloForoMensaje;
        private bool _VisibleCrearForo;
        private bool _OcultarCrearForo;
        private bool _VisibleCrearTarea;
        private bool _VisibleMensajeForo;
        private bool _VisibleCrearSeguimiento;
        private string _NuevoMensajeForo;
        private string _DescripcionNuevaTarea;
        private string _NuevoIncidenciaTareasDisponibles;
        private string _NuevoSeguimientoTD;
        private ActividadWrapper _Actividad;
        private CooperanteWrapper _ResponsableTarea;
        private DateTime _FechaFinTarea;
        private ISession _session;
        private IEventoRepository _EventoRepository;
        private IIncidenciaRepository _IncidenciaRepository;
        private ITareaRepository _TareaRepository;
        private ISeguimientoRepository _SeguimientoRepository;
        private IEventAggregator _EventAggregator;
        private bool _PopupEstaAbierto = false;
        private ForoWrapper _ForoSeleccionado;

        public TareasDeActividadViewModel(
            IActividadRepository actividadRepository,
            IEventoRepository eventoRepository,
            ICooperanteRepository cooperabteRepository,
            ITareaRepository tareaRepository,
            ISeguimientoRepository seguimientoRepository,
            IIncidenciaRepository incidenciaRepository,
            IEventAggregator eventAggregator, ISession session)     // Constructor de la clase
        {
            _VisibleCrearForo = false;
            _VisibleCrearTarea = false;
            _OcultarCrearForo = true;
            _VisibleMensajeForo = false;
            _EventoRepository = eventoRepository;
            _actividadRepository = actividadRepository;
            _IncidenciaRepository = incidenciaRepository;
            _TareaRepository = tareaRepository;
            _SeguimientoRepository = seguimientoRepository;
            _session = session;
            _actividadRepository.Session = session;
            _EventoRepository.Session = session;
            _IncidenciaRepository.Session = session;
            _TareaRepository.Session = session;
            _SeguimientoRepository.Session = session;
            _EventAggregator = eventAggregator;

            MensajesDisponibleEnForo = new ObservableCollection<Mensaje>();
            ForosDisponibles = new ObservableCollection<ForoWrapper>();
            TareasDisponibles = new ObservableCollection<TareaWrapper>();
            EventoActividad = new ObservableCollection<Evento>();
            TareasFinalizadas = new ObservableCollection<TareaWrapper>();
            CooperantesSeleccionados = new ObservableCollection<CooperanteWrapper>();

            _EventAggregator.GetEvent<CargarNuevaActividadEvent>().Subscribe(OnCargarNuevaActividadEvent);
            _EventAggregator.GetEvent<NuevoForoCreadoEvent>().Subscribe(OnNuevoForoCreadoEvent);
            _EventAggregator.GetEvent<NuevaTareaCreadaEvent>().Subscribe(OnNuevaTareaCreadaEvent);
            _EventAggregator.GetEvent<PublicarEventosActividad>().Subscribe(OnPublicarEventosActividad);
            CrearForoCommand = new DelegateCommand(OnCrearForoCommand, OnCrearForoCommand_CanExecute);
            CrearTareaCommand = new DelegateCommand(OnCrearTareaCommand, OnCrearTareaCommand_CanExecute);
            MensajesForoCommand = new DelegateCommand<object>(OnMensajeForoCommand, OnMensajeForoCommand_CanExecute);
            InfoTareaCommand = new DelegateCommand<object>(OnInfoTareaCommand, OnInfoTareaCommand_CanExecute);
            AceptarCrearForoCommand = new DelegateCommand(OnAceptarCrearForoCommand, OnAceptarCrearForoCommand_CanExecute);
            AceptarNuevoMensajeCommand = new DelegateCommand<object>(OnAceptarNuevoMensajeCommand, OnAceptarNuevoMensajeCommand_CanExecute);
            AceptarIncidenciaTDCommand = new DelegateCommand<object>(OnAceptarIncidenciaTDCommand, OnAceptarIncidenciaTDCommand_CanExecute);
            AceptarSeguimientoTDCommand = new DelegateCommand<object>(OnAceptarSeguimientoTDCommand, OnAceptarSeguimientoTDCommand_CanExecute);
            FinalizarTareaTDCommand = new DelegateCommand<object>(OnFinalizarTareaTDCommand, OnFinalizarTareaTDCommand_CanExecute);
            AceptarCrearTareaCommand = new DelegateCommand(OnAceptarCrearTareaCommand, OnAceptarCrearTareaCommand_CanExecute);
            AñadirCooperantesComboBox = new DelegateCommand (OnAñadirCooperantesComboBox, OnAñadirCooperantesComboBox_CanExecute);
        }
        public string TituloForo
        {
            get { return _tituloForo; }
            set { SetProperty(ref _tituloForo, value); }
        }
        public string TituloForoMensaje
        {
            get { return _tituloForoMensaje; }
            set { SetProperty(ref _tituloForoMensaje, value); }
        }
        public string NuevoMensajeForo
        {
            get { return _NuevoMensajeForo; }
            set { SetProperty(ref _NuevoMensajeForo, value); }
        }
        public string DescripcionNuevaTarea
        {
            get { return _DescripcionNuevaTarea; }
            set { SetProperty(ref _DescripcionNuevaTarea, value); }
        }
        public string NuevoIncidenciaTareasDisponibles
        {
            get { return _NuevoIncidenciaTareasDisponibles; }
            set { SetProperty(ref _NuevoIncidenciaTareasDisponibles, value); }
        }
        public string NuevoSeguimientoTD
        {
            get { return _NuevoSeguimientoTD; }
            set { SetProperty(ref _NuevoSeguimientoTD, value); }
        }
        public ICommand AceptarCrearForoCommand { get; private set; }
        public ICommand AceptarCrearTareaCommand { get; set; }
        public ICommand AceptarNuevoMensajeCommand { get; private set; }
        public ICommand AceptarIncidenciaTDCommand { get; private set; }
        public ICommand AceptarSeguimientoTDCommand { get; set; }
        public ICommand FinalizarTareaTDCommand { get; set; }
        public ICommand MensajesForoCommand { get; private set; }
        public ICommand InfoTareaCommand { get; private set; }
        public ICommand CrearForoCommand { get; private set; }
        public ICommand CrearTareaCommand { get; private set; }
        public ICommand AñadirCooperantesComboBox { get; private set; }

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
            ((ForoWrapper)wrapper).ForoVisible = !((ForoWrapper)wrapper).ForoVisible;
        }
        private void OnInfoTareaCommand(object wrapper)
        {
            ((TareaWrapper)wrapper).SeguimientoVisible = !((TareaWrapper)wrapper).SeguimientoVisible;
        }
        private void OnAceptarCrearForoCommand()
        {
            var mensajeForo = new MensajeWrapper(new Mensaje() {
                Titulo = TituloForoMensaje,
                FechaDePublicacion = DateTime.Now,
            });
            var foroW= (new ForoWrapper(new Foro()
                                        { Titulo = TituloForo, FechaDePublicacion = DateTime.Now})
                                        { ForoVisible = true });
            foroW.Mensajes.Add(mensajeForo);
            Actividad.Model.AddForo(foroW.Model);
            _EventAggregator.GetEvent<NuevoForoCreadoEvent>().Publish(foroW);
            AuxiliarOnPublicarEventosActividad(Ocurrencia.FORO_CREADO, TituloForo);
            CrearForoVisible = false;
            OcultarForoVisible = true;
        }
        private void OnAceptarCrearTareaCommand()
        {
            var NuevaTarea = (new TareaWrapper(new Tarea()
            {
                Descripcion = DescripcionNuevaTarea,
                FechaDeFinalizacion = FechaFinTarea,
                Responsable = ResponsableTarea.Model,
                HaFinalizado = false
            }));
            Actividad.Model.AddTarea(NuevaTarea.Model);
            //_session.Save(NuevaTarea.Model); 
            //_actividadRepository.Update(Actividad.Model);
            //_TareaRepository.Create(NuevaTarea.Model);
            _EventAggregator.GetEvent<NuevaTareaCreadaEvent>().Publish(NuevaTarea);
            AuxiliarOnPublicarEventosActividad(Ocurrencia.NUEVA_TAREA_PUBLICADA, ((TareaWrapper)NuevaTarea).Descripcion);
            CrearTareaVisible = false;
        }
        private void OnAceptarNuevoMensajeCommand(object wrapper)
        {
            var nuevoMensaje = new MensajeWrapper(new Mensaje()
            {
                Titulo = NuevoMensajeForo,
                FechaDePublicacion = DateTime.Now,
            });
            //((ForoWrapper)wrapper).Mensajes.Add(nuevoMensaje);
            ((ForoWrapper)wrapper).Mensajes.Insert(0,nuevoMensaje);
            AuxiliarOnPublicarEventosActividad(Ocurrencia.MENSAJE_PUBLICADO_EN_FORO,((ForoWrapper)wrapper).Titulo);
            NuevoMensajeForo = null;
        }
        private void OnAceptarIncidenciaTDCommand(object wrapper)
        {
            var nuevaIncidencia = new IncidenciaWrapper(new Incidencia()
            {
                Descripcion = NuevoIncidenciaTareasDisponibles,
                FechaDePublicacion = DateTime.Now,
            });
            ((TareaWrapper)wrapper).Incidencia.Insert(0, nuevaIncidencia);
            var tar = ((TareaWrapper)wrapper);
            
            ((TareaWrapper)wrapper).Model.AddIncidencia(nuevaIncidencia.Model);
            //_session.Update(((TareaWrapper)wrapper).Model); // Creola actividad con un id nuevo con la incidencia => tendria que abrir otra sesion con tarea
            //_IncidenciaRepository.Create(nuevaIncidencia.Model);
            //_actividadRepository.Update(Actividad.Model);
            //_TareaRepository.Create(((TareaWrapper)wrapper).Model); // Errorque da Found shared references to a collection: Gama.Cooperacion.Business.Tarea.Seguimiento
            AuxiliarOnPublicarEventosActividad(Ocurrencia.INCIDENCIA_EN_TAREA, ((TareaWrapper)wrapper).Descripcion);
            
            NuevoIncidenciaTareasDisponibles = null;
        }
        private void OnAceptarSeguimientoTDCommand(object wrapper)
        {
            var nuevoSeguimiento = new SeguimientoWrapper(new Seguimiento()
            {
                Descripcion = NuevoSeguimientoTD,
                FechaDePublicacion = DateTime.Now,
            });
            ((TareaWrapper)wrapper).Seguimiento.Insert(0, nuevoSeguimiento);
            AuxiliarOnPublicarEventosActividad(Ocurrencia.SEGUIMIENGO_EN_TAREA, ((TareaWrapper)wrapper).Descripcion);
            NuevoSeguimientoTD = null;
        }
        private void OnFinalizarTareaTDCommand(object wrapper)
        {
            //TareasFinalizadas.Add((TareaWrapper)wrapper);
            TareasFinalizadas.Insert(0, (TareaWrapper)wrapper);
            TareasDisponibles.Remove((TareaWrapper)wrapper);
            AuxiliarOnPublicarEventosActividad(Ocurrencia.TAREA_FINALIZADA, ((TareaWrapper)wrapper).Descripcion);
        }
        private bool OnFinalizarTareaTDCommand_CanExecute(object wrapper)
        {
            return true;
        }
        private bool OnAceptarSeguimientoTDCommand_CanExecute(object wrapper)
        {
            return true;
        }
        private bool OnAceptarIncidenciaTDCommand_CanExecute(object wrapper)
        {
            return true;
        }
        private bool OnAceptarNuevoMensajeCommand_CanExecute(object wrapper)
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
        private bool OnAceptarCrearForoCommand_CanExecute()
        {
            return true;
        }
        private bool OnAceptarCrearTareaCommand_CanExecute()
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
        public ActividadWrapper Actividad
        {
            get { return _Actividad; }
            set { SetProperty(ref _Actividad, value); }
        }
        public CooperanteWrapper ResponsableTarea
        {
            get { return _ResponsableTarea; }
            set {
                ((DelegateCommand)AñadirCooperantesComboBox).RaiseCanExecuteChanged(); // Para que es
                SetProperty(ref _ResponsableTarea, value); }
        }
        public DateTime FechaFinTarea
        {
            get { return _FechaFinTarea; }
            set { SetProperty(ref _FechaFinTarea, value); }
        }
        public void LoadActividad(ActividadWrapper wrapper)
        {
            Actividad = wrapper;
            Actividad.AcceptChanges();
            _EventAggregator.GetEvent<CargarNuevaActividadEvent>().Publish(Actividad.Id);    
        }
        public ObservableCollection<ForoWrapper> ForosDisponibles { get; private set; }
        public ObservableCollection<Mensaje> MensajesDisponibleEnForo { get; private set; }
        public ObservableCollection<Evento> EventoActividad { get; private set; }
        public ObservableCollection<TareaWrapper> TareasDisponibles { get; private set; }
        public ObservableCollection<TareaWrapper> TareasFinalizadas { get; private set; }
        public ObservableCollection<CooperanteWrapper> CooperantesSeleccionados { get; private set; }
        public ForoWrapper ForoSelecionado { get; set; }
        public bool CrearForoVisible
        {
            get { return _VisibleCrearForo; }
            set { SetProperty(ref _VisibleCrearForo, value); }
        }
        public bool CrearTareaVisible
        {
            get { return _VisibleCrearTarea; }
            set { SetProperty(ref _VisibleCrearTarea, value); }
        }
        public bool MensajeForoVisible
        {
            get { return _VisibleMensajeForo; }
            set { SetProperty(ref _VisibleMensajeForo, value); }
        }
        public bool OcultarForoVisible
        {
            get { return _OcultarCrearForo; }
            set { SetProperty(ref _OcultarCrearForo, value); }
        }
        private bool OnCrearForoCommand_CanExecute()
        {
            return true;
        }
        private void OnCargarNuevaActividadEvent(int id)
        {
            var actividad = _actividadRepository.GetById(id); // actividad contiene la información de la base de datos
            foreach (var forito in actividad.Foros)
            {
                ForosDisponibles.Add(new ForoWrapper(
                    new Foro() {
                        Titulo = forito.Titulo,
                        FechaDePublicacion = forito.FechaDePublicacion,
                        Mensajes = forito.Mensajes })
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
            }

        }
        private void OnNuevoForoCreadoEvent(ForoWrapper forito)
        {
            ForosDisponibles.Insert(0, (new ForoWrapper(new Foro()
            { Titulo = forito.Titulo, FechaDePublicacion = forito.FechaDePublicacion, Mensajes=forito.Model.Mensajes })
            { ForoVisible = true }));
        }
        private void OnNuevaTareaCreadaEvent(TareaWrapper NuevaTarea)
        {
            TareasDisponibles.Insert(0, (new TareaWrapper(new Tarea()
            {
                Descripcion = NuevaTarea.Descripcion,
                FechaDeFinalizacion = NuevaTarea.FechaDeFinalizacion,
                HaFinalizado = NuevaTarea.HaFinalizado,
                Responsable = NuevaTarea.Responsable.Model
            })
            { SeguimientoVisible = false }
            ));
        }
        private void AuxiliarOnPublicarEventosActividad(Ocurrencia tipoEvento,string tituloEvento)
        {
            var eventoDeActividad = new Evento()
            {
                FechaDePublicacion = DateTime.Now,
                Titulo = tituloEvento,
                Ocurrencia = tipoEvento,
            };
            _EventAggregator.GetEvent<PublicarEventosActividad>().Publish(eventoDeActividad);
        }
        private void OnPublicarEventosActividad(Evento GenerarEvento)
        {
            EventoActividad.Insert(0, GenerarEvento);
            
            // Falta código para BBDD
            Actividad.Model.AddEvento(GenerarEvento);
            //_EventoRepository.Create(GenerarEvento);
            _actividadRepository.Update(Actividad.Model);
        }
        public List<EventoWrapper> EventosDisponibles { get; private set; }

        private void OnCrearForoCommand()
        {
            if (CrearForoVisible == false)
            {
                CrearForoVisible = true;
                OcultarForoVisible = false;
            }
            else
            {
                CrearForoVisible = false;
                OcultarForoVisible = true;
            }
        }

        private void OnCrearTareaCommand()
        {
            if (CrearTareaVisible == false)
            {
                var actividad = _actividadRepository.GetById(Actividad.Id); // actividad contiene la información de la base de datos
                var cooper = new Cooperante()
                {
                    Id = 1,
                    Dni = "100",
                    Nombre = "Jose",
                    Apellido = "Martin",
                };
                actividad.Cooperantes.Add(cooper);
                var cooper2 = new Cooperante()
                {
                    Id = 2,
                    Dni = "200",
                    Nombre = "Gabriel",
                    Apellido = "Martin",
                };
                actividad.Cooperantes.Add(cooper2);
                foreach (var cooper3 in actividad.Cooperantes)
                {
                    CooperantesSeleccionados.Add(new CooperanteWrapper(
                        new Cooperante()
                        {
                            Id = cooper3.Id,
                            Dni = cooper3.Dni,
                            Nombre = cooper3.Nombre,
                            Apellido = cooper3.Apellido
                        }));
                }
                CrearTareaVisible = true;
            }
            else
            {
                CrearTareaVisible = false;
            }
        }


    }   // Clase TareasDeActividadVM
}       // Fin namespace
