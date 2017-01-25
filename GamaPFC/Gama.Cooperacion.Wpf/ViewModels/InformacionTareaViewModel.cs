using Core;
using Gama.Common.CustomControls;
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
    public class InformacionTareaViewModel : ViewModelBase
    {
        // Zona de Declaracion variables privadas
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser
        private IActividadRepository _ActividadRepository;
        private IEventAggregator _EventAggregator;
        private string _DescripcionNuevaTarea;
        private string _NuevaIncidencia;
        private CooperanteWrapper _ResponsableTarea;
        private DateTime _FechaFinTarea;
        private int _ModificarTarea;
        private int _TareaID;
        private TareaWrapper _TareaSeleccionada;
        // Fin Zona
        // Constructor de la clase
        public InformacionTareaViewModel(IActividadRepository ActividadRepository, IEventAggregator EventAggregator)
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
        /// <summary>
        /// Zona Icommand => Funciones invocadas al pulsar un botón.
        /// Uso de la clase DelegateCommand de la librería Prims.Commands
        /// </summary>
        public ICommand AceptarIncidenciaCommand { get; private set; }
        public ICommand CancelarIncidenciaCommand { get; private set; }

        /// <summary>
        /// Zona DelegateCommand _CanExecute => Comprueba que se cumplen las condicines para ejecutar la acción del botón
        /// </summary>
        private bool OnAceptarIncidenciaCommand_CanExecute()
        {
            return true;
        }
        /// <summary>
        /// Zona DelegateCommand _Execute => Funciones que implementan la acción al pulsar un botón
        /// </summary>
        private void OnAceptarIncidenciaCommand_Execute()
        {
            var nuevaIncidencia = new IncidenciaWrapper(new Incidencia()
            {
                Descripcion = NuevaIncidencia,
                FechaDePublicacion = DateTime.Now,
            });
            TareaSeleccionada.Model.AddIncidencia(nuevaIncidencia.Model);
            _ActividadRepository.Update(Actividad.Model);
            TareaSeleccionada.Incidencias.Insert(0, nuevaIncidencia);
            var eventoDeActividad = new Evento()
            {
                FechaDePublicacion = DateTime.Now,
                Titulo = TareaSeleccionada.Descripcion,
                Ocurrencia = Ocurrencia.INCIDENCIA_EN_TAREA,
            };
            _EventAggregator.GetEvent<PublicarEventosActividad>().Publish(eventoDeActividad);
            NuevaIncidencia = null;
            

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
        private void OnCancelarIncidenciaCommand_Execute()
        {

        }
        /// <summary>
        /// Zona Funciones Mienbro => Funciones necesirarias en el ViewModel para realizar su trabajo
        /// </summary>
        public void LoadActividad(ActividadWrapper actividad)
        {
            Actividad = actividad;
            var act = _ActividadRepository.GetById(Actividad.Id); // actividad contiene la información de la base de datos
            //foreach (var tarea in act.Tareas)
            //{
            //    if (tarea.HaFinalizado == false)
            //    {
            //        TareasDisponibles.Add(new TareaWrapper(
            //        new Tarea()
            //        {
            //            Id = tarea.Id,
            //            Descripcion = tarea.Descripcion,
            //            FechaDeFinalizacion = tarea.FechaDeFinalizacion,
            //            HaFinalizado = tarea.HaFinalizado,
            //            Seguimiento = tarea.Seguimiento,
            //            Incidencias = tarea.Incidencias,
            //            Responsable = tarea.Responsable,
            //        })
            //        { SeguimientoVisible = false });
            //    }
            //    else
            //    {
            //        TareasFinalizadas.Add(new TareaWrapper(
            //        new Tarea()
            //        {
            //            Id = tarea.Id,
            //            Descripcion = tarea.Descripcion,
            //            FechaDeFinalizacion = tarea.FechaDeFinalizacion,
            //            HaFinalizado = tarea.HaFinalizado,
            //            Seguimiento = tarea.Seguimiento,
            //            Incidencias = tarea.Incidencias,
            //            Responsable = tarea.Responsable,
            //        })
            //        { SeguimientoVisible = false });
            //    }

            //}
            Actividad.AcceptChanges();
            // Por lo pronto probamos con estos dos lineas porque creo que no las necesito
            // Con tener Session creo que es suficiente para poder guardar el foro => ERROR si no necesito porque tengo que
            // usar Actividad.Foros.Add(Foro); antes de llamar a actividadRepository
            // Foro.Actividad = actividad; //Para que no de error en ForoWrapper Actividad tiene que tener set sin private
        }
        public void LoadTarea(TareaWrapper tarea) // Esto es necesario para cuando edito una tarea que tengo que pasar la informacion de la tarea
        {
            TareaSeleccionada = tarea;
            //TareasDisponibles.Add(new TareaWrapper(
            //        new Tarea()
            //        {
            //            Id = tarea.Id,
            //            Descripcion = tarea.Descripcion,
            //            FechaDeFinalizacion = tarea.FechaDeFinalizacion,
            //            HaFinalizado = tarea.HaFinalizado,
            //            //Seguimiento = tarea.Seguimiento,
            //            //Incidencias = tarea.Incidencias,
            //            //Responsable = tarea.Responsable,
            //        })
            //{ SeguimientoVisible = false });
        }
    }
    
}
