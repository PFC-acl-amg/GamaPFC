using Core;
using Gama.Common.Eventos;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using Remotion.Linq.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class MensajesForoViewModel : ViewModelBase
    {
        // Zona de Declaracion variables privadas
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser
        private IActividadRepository _ActividadRepository;
        private IEventAggregator _EventAggregator;
        private string _DescripcionNuevaTarea;
        private string _NuevoMensaje;
        private string _NuevoSeguimiento;
        private CooperanteWrapper _ResponsableTarea;
        private DateTime _FechaFinTarea;
        private int _ModificarTarea;
        private int _TareaID;
        private ForoWrapper _ForoSeleccionado;
        // Fin Zona
        // Constructor de la clase
        public MensajesForoViewModel(IActividadRepository ActividadRepository, IEventAggregator EventAggregator)
        {
            _ActividadRepository = ActividadRepository;
            _EventAggregator = EventAggregator;
            _ModificarTarea = 0;
            _TareaID = 0;

            TareasDisponibles = new ObservableCollection<TareaWrapper>();
            TareasFinalizadas = new ObservableCollection<TareaWrapper>();


            AceptarMensajeCommand = new DelegateCommand(OnAceptarMensajeCommand_Execute,
                OnAceptarMensajeCommand_CanExecute);
            CancelarIncidenciaCommand = new DelegateCommand(OnCancelarIncidenciaCommand_Execute);
            //AceptarSeguimientoCommand = new DelegateCommand(OnAceptarSeguimientoCommand_Execute,OnAceptarSeguimientoCommand_CanExecute);
        }

        public override void OnActualizarServidor()
        {
            var actividadActualizada = _ActividadRepository.GetById(Actividad.Id);
            int idSeleccionado = ForoSeleccionado.Id;
            var foro = actividadActualizada.Foros.Where(x => x.Id == idSeleccionado).First();
            ForoSeleccionado = new ForoWrapper(foro);
        }

        /// <summary>
        /// Zona Entidades y Contenedores
        /// </summary>
        public ActividadWrapper Actividad { get; private set; }
        //public TareaWrapper TareaSeleccionada { get; private set; }
        public ObservableCollection<TareaWrapper> TareasDisponibles { get; private set; }
        public ObservableCollection<TareaWrapper> TareasFinalizadas { get; private set; }
        /// <summary>
        /// Zona Propertychanges => Variables que contralan actualizacion en la interfaz de usuario
        /// </summary>
        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }
        public ForoWrapper ForoSeleccionado
        {
            get { return _ForoSeleccionado; }
            set { SetProperty(ref _ForoSeleccionado, value); }
        }
        public ISession Session
        {
            get { return null; }
            set
            {
                _ActividadRepository.Session = value;
            }
        }
        public string NuevoMensaje
        {
            get { return _NuevoMensaje; }
            set { SetProperty(ref _NuevoMensaje, value); }
        }
        public string NuevoSeguimiento
        {
            get { return _NuevoSeguimiento; }
            set { SetProperty(ref _NuevoSeguimiento, value); }
        }
        /// <summary>
        /// Zona Icommand => Funciones invocadas al pulsar un botón.
        /// Uso de la clase DelegateCommand de la librería Prims.Commands
        /// </summary>
        public ICommand AceptarMensajeCommand { get; private set; }
        public ICommand CancelarIncidenciaCommand { get; private set; }
        public ICommand AceptarSeguimientoCommand { get; private set; }

        /// <summary>
        /// Zona DelegateCommand _CanExecute => Comprueba que se cumplen las condicines para ejecutar la acción del botón
        /// </summary>
        private bool OnAceptarMensajeCommand_CanExecute()
        {
            return true;
        }
        private bool OnAceptarSeguimientoCommand_CanExecute()
        {
            return true;
        }
        /// <summary>
        /// Zona DelegateCommand _Execute => Funciones que implementan la acción al pulsar un botón
        /// </summary>
        private void OnAceptarMensajeCommand_Execute()
        {
            //var nuevoMensaje = new MensajeWrapper(new Mensaje()
            //{
            //    Titulo = NuevoMensajeForo,
            //    FechaDePublicacion = DateTime.Now,
            //});
            //((ForoWrapper)wrapper).Mensajes.Insert(0, nuevoMensaje); // Inserta en la variable Mensajes del wrapper y en .Model en la posicion 0
            //((ForoWrapper)wrapper).Model.AddMensaje(nuevoMensaje.Model); // Inserta en la variable Mensaje de wrapper.Model.Mensajes
            //_actividadRepository.Update(Actividad.Model);
            //AuxiliarOnPublicarEventosActividad(Ocurrencia.MENSAJE_PUBLICADO_EN_FORO, ((ForoWrapper)wrapper).Titulo);
            //NuevoMensajeForo = null;
            var foroSeleccionado = Actividad.Foros.Where(x => x.Id == ForoSeleccionado.Id).First();
            var nuevoMensaje = new MensajeWrapper(new Mensaje()
            {
                Titulo = NuevoMensaje,
                FechaDePublicacion = DateTime.Now,
                Foro = foroSeleccionado.Model
            });
            foroSeleccionado.Mensajes.Add(nuevoMensaje);//Con esta instruccion se añade la incidencia al wrapper
                                                        // y se anade la informacion al .Model menos a que tarea pertenece la incidencia
                                                        // con lo que el mapeo en la bade de datos falla porque a la incidenia deja a nulo
                                                        // la tarea-id a la que petenece => para que tome el id del foro al que pertenece al crear 
                                                        // el mensaje se añadela instruccion Foro = foroSeleccionado.Model

            //tareaSeleccionada.Model.AddIncidencia(nuevaIncidencia.Model);
            // TareaSeleccionada.Model.AddIncidencia(nuevaIncidencia.Model);

            ForoSeleccionado.Mensajes.Insert(0, nuevoMensaje);
            var eventoDeActividad = new Evento()
            {
                FechaDePublicacion = DateTime.Now,
                Titulo = ForoSeleccionado.Titulo,
                Ocurrencia = Ocurrencia.MENSAJE_PUBLICADO_EN_FORO.ToString(),
            };
            Actividad.Model.AddEvento(eventoDeActividad);
            _ActividadRepository.Update(Actividad.Model);
            _EventAggregator.GetEvent<PublicarEventosActividad>().Publish(eventoDeActividad);
            NuevoMensaje = null;


            //Actividad.Tareas.Where(ident => ident.Id == _TareaID).First().Descripcion = DescripcionNuevaTarea;
            //Actividad.Tareas.Where(ident => ident.Id == _TareaID).First().FechaDeFinalizacion = FechaFinTarea;
            //Actividad.Tareas.Where(ident => ident.Id == _TareaID).First().Responsable = ResponsableTarea;
            ////CooperantesDisponibles.Remove(CooperantesDisponibles.Where(c => c.Id == cooperante.Id).First());
            ////Actividad.Tareas[0].Id ==
            //Actividad.UpdatedAt = DateTime.Now;
            //_ActividadRepository.Update(Actividad.Model);
            Actividad.AcceptChanges();
            //_EventAggregator.GetEvent<TareaModificadaEvent>().Publish(_TareaID);


            //if (_ModificarTarea == 0)
            //{
            //    var nuevoSeguimiento = new SeguimientoWrapper(new Seguimiento()
            //    {
            //        Descripcion = "Historial del desarrollo de las tareas",
            //        FechaDePublicacion = DateTime.Now,
            //    });
            //    var incidenciaTarea = (new IncidenciaWrapper(new Incidencia()
            //    {
            //        Descripcion = "Historial de incidencias durante desarrollo de la tarea",
            //        FechaDePublicacion = DateTime.Now,
            //    }));
            //    var NuevaTarea = (new TareaWrapper(new Tarea()
            //    {
            //        Descripcion = DescripcionNuevaTarea,
            //        FechaDeFinalizacion = FechaFinTarea,
            //        Responsable = ResponsableTarea.Model,
            //        HaFinalizado = false
            //    })
            //    { SeguimientoVisible = false });
            //    NuevaTarea.Model.AddIncidencia(incidenciaTarea.Model);
            //    NuevaTarea.Model.AddSeguimiento(nuevoSeguimiento.Model);
            //    Actividad.Model.AddTarea(NuevaTarea.Model);
            //    _ActividadRepository.Update(Actividad.Model);
            //    _EventAggregator.GetEvent<NuevaTareaCreadaEvent>().Publish(NuevaTarea);
            //    var eventoDeActividad = new Evento()
            //    {
            //        FechaDePublicacion = DateTime.Now,
            //        Titulo = DescripcionNuevaTarea,
            //        Ocurrencia = Ocurrencia.NUEVA_TAREA_PUBLICADA,
            //    };
            //    _EventAggregator.GetEvent<PublicarEventosActividad>().Publish(eventoDeActividad);
            //    _ModificarTarea = 0;
            //}
            //else
            //{
            //    Actividad.Tareas.Where(ident => ident.Id == _TareaID).First().Descripcion = DescripcionNuevaTarea;
            //    Actividad.Tareas.Where(ident => ident.Id == _TareaID).First().FechaDeFinalizacion = FechaFinTarea;
            //    Actividad.Tareas.Where(ident => ident.Id == _TareaID).First().Responsable = ResponsableTarea;
            //    //CooperantesDisponibles.Remove(CooperantesDisponibles.Where(c => c.Id == cooperante.Id).First());
            //    //Actividad.Tareas[0].Id ==
            //    Actividad.UpdatedAt = DateTime.Now;
            //    _ActividadRepository.Update(Actividad.Model);
            //    Actividad.AcceptChanges();
            //    _EventAggregator.GetEvent<TareaModificadaEvent>().Publish(_TareaID);
            //}

            //Cerrar = true;
        }
        //private void OnAceptarSeguimientoCommand_Execute()
        //{
        //    var nuevoSeguimiento = new SeguimientoWrapper(new Seguimiento()
        //    {
        //        Descripcion = NuevoSeguimiento,
        //        FechaDePublicacion = DateTime.Now,
        //    });
        //    ForoSeleccionado.Model.AddMensaje(nuevoSeguimiento.Model);
        //    _ActividadRepository.Update(Actividad.Model);
            
        //    var eventoDeActividad = new Evento()
        //    {
        //        FechaDePublicacion = DateTime.Now,
        //        Titulo = ForoSeleccionado.Titulo,
        //        Ocurrencia = Ocurrencia.SEGUIMIENGO_EN_TAREA,
        //    };
        //    _EventAggregator.GetEvent<PublicarEventosActividad>().Publish(eventoDeActividad);
        //    NuevoSeguimiento = null;
        //    Actividad.AcceptChanges();
        //}
        private void OnCancelarIncidenciaCommand_Execute()
        {

        }
        /// <summary>
        /// Zona Funciones Mienbro => Funciones necesirarias en el ViewModel para realizar su trabajo
        /// </summary>
        public void LoadActividad(ActividadWrapper actividad)
        {
            Actividad = actividad;
            Actividad.AcceptChanges();
        }
        public void LoadForo(ForoWrapper foro) // Para Foro no se si es necesario
        {
            ForoSeleccionado = foro;
        }
    }
}
