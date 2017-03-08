using Core;
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
    public class SeguimientoTareaViewModel : ViewModelBase
    {
        // Zona de Declaracion variables privadas
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser
        private IActividadRepository _ActividadRepository;
        private IEventAggregator _EventAggregator;
        private string _DescripcionNuevaTarea;
        private string _NuevaIncidencia;
        private string _NuevoSeguimiento;
        private CooperanteWrapper _ResponsableTarea;
        private DateTime _FechaFinTarea;
        private int _ModificarTarea;
        private int _TareaID;
        private TareaWrapper _TareaSeleccionada;
        // Fin Zona
        // Constructor de la clase
        public SeguimientoTareaViewModel(IActividadRepository ActividadRepository, IEventAggregator EventAggregator)
        {
            _ActividadRepository = ActividadRepository;
            _EventAggregator = EventAggregator;
            _ModificarTarea = 0;
            _TareaID = 0;

            TareasDisponibles = new ObservableCollection<TareaWrapper>();
            TareasFinalizadas = new ObservableCollection<TareaWrapper>();

            AceptarIncidenciaCommand = new DelegateCommand(OnAceptarIncidenciaCommand_Execute,
                OnAceptarIncidenciaCommand_CanExecute);
            CancelarIncidenciaCommand = new DelegateCommand(OnCancelarIncidenciaCommand_Execute);
            AceptarSeguimientoCommand = new DelegateCommand(OnAceptarSeguimientoCommand_Execute,
                OnAceptarSeguimientoCommand_CanExecute);
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
        public TareaWrapper TareaSeleccionada
        {
            get { return _TareaSeleccionada; }
            set { SetProperty(ref _TareaSeleccionada, value); }
        }
        public ISession Session
        {
            get { return null; }
            set
            {
                _ActividadRepository.Session = value;
            }
        }
        public string NuevaIncidencia
        {
            get { return _NuevaIncidencia; }
            set { SetProperty(ref _NuevaIncidencia, value); }
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
        public ICommand AceptarIncidenciaCommand { get; private set; }
        public ICommand CancelarIncidenciaCommand { get; private set; }
        public ICommand AceptarSeguimientoCommand { get; private set; }

        /// <summary>
        /// Zona DelegateCommand _CanExecute => Comprueba que se cumplen las condicines para ejecutar la acción del botón
        /// </summary>
        private bool OnAceptarIncidenciaCommand_CanExecute()
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
        private void OnAceptarIncidenciaCommand_Execute()
        {
            var tareaSeleccionada = Actividad.Tareas.Where(x => x.Id == TareaSeleccionada.Id).First();
            var nuevoSeguimiento = new SeguimientoWrapper(new Seguimiento()
            {
                Descripcion = NuevoSeguimiento,
                FechaDePublicacion = DateTime.Now,
                Tarea = tareaSeleccionada.Model
            });
            
            tareaSeleccionada.Seguimiento.Add(nuevoSeguimiento);//Con esta instruccion se añade la incidencia al wrapper
                                                               // y se anade la informacion al .Model menos a que tarea pertenece la incidencia
                                                               // con lo que el mapeo en la bade de datos falla porque a la incidenia deja a nulo
                                                               // la tarea-id a la que petenece
            _ActividadRepository.Update(Actividad.Model);
            TareaSeleccionada.Seguimiento.Insert(0, nuevoSeguimiento);
            var eventoDeActividad = new Evento()
            {
                FechaDePublicacion = DateTime.Now,
                Titulo = TareaSeleccionada.Descripcion,
                Ocurrencia = Ocurrencia.INCIDENCIA_EN_TAREA,
            };
            _EventAggregator.GetEvent<PublicarEventosActividad>().Publish(eventoDeActividad);
            NuevoSeguimiento = null;
            Actividad.AcceptChanges();
        }
        private void OnAceptarSeguimientoCommand_Execute()
        {
            var tareaSeleccionada = Actividad.Tareas.Where(x => x.Id == TareaSeleccionada.Id).First();
            var nuevoSeguimiento = new SeguimientoWrapper(new Seguimiento()
            {
                Descripcion = NuevoSeguimiento,
                FechaDePublicacion = DateTime.Now,
                Tarea = tareaSeleccionada.Model
            });
            
            tareaSeleccionada.Seguimiento.Add(nuevoSeguimiento);
            _ActividadRepository.Update(Actividad.Model);
            TareaSeleccionada.Seguimiento.Insert(0, nuevoSeguimiento);
            var eventoDeActividad = new Evento()
            {
                FechaDePublicacion = DateTime.Now,
                Titulo = TareaSeleccionada.Descripcion,
                Ocurrencia = Ocurrencia.SEGUIMIENGO_EN_TAREA,
            };
            _EventAggregator.GetEvent<PublicarEventosActividad>().Publish(eventoDeActividad);
            NuevoSeguimiento = null;
            Actividad.AcceptChanges();
        }
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
        public void LoadTarea(TareaWrapper tarea) // Esto es necesario para cuando edito una tarea que tengo que pasar la informacion de la tarea
        {
            TareaSeleccionada = tarea;
        }
    }
}
