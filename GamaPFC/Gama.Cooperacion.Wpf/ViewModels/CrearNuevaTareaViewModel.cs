using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class CrearNuevaTareaViewModel : ViewModelBase
    {
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser
        private IActividadRepository _ActividadRepository;
        private IEventAggregator _EventAggregator;
        private string _DescripcionNuevaTarea;
        private CooperanteWrapper _ResponsableTarea;
        private TareaWrapper _NuevaTarea;
        private DateTime? _FechaFinTarea;
        private int _ModificarTarea;
        private int _TareaID;
        private ITareaRepository _TareaRepository;

        public CrearNuevaTareaViewModel(IActividadRepository actividadRepository, 
            ITareaRepository tareaRepository,
            IEventAggregator EventAggregator)
        {
            Gama.Common.Debug.Debug.StartWatch();
            _ActividadRepository = actividadRepository;
            _TareaRepository = tareaRepository;
            _EventAggregator = EventAggregator;
            _ModificarTarea = 0;
            _TareaID = 0;

            _NuevaTarea = new TareaWrapper(new Tarea());

            _EventAggregator.GetEvent<ActividadActualizadaEvent>().Subscribe(PublicarCooperante);

            CooperantesSeleccionados = new ObservableCollection<CooperanteWrapper>();
            AceptarCommand = new DelegateCommand(OnAceptarCommand_Execute,
                OnAceptarCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand_Execute);
            AñadirCooperantesComboBox = new DelegateCommand(OnAñadirCooperantesComboBox, OnAñadirCooperantesComboBox_CanExecute);
            Gama.Common.Debug.Debug.StopWatch("CrearNuevaTareaViewModel");
        }
        private void PublicarCooperante(Actividad ActividadSeleccionada)
        {
            OnAñadirCooperantesComboBox();
        }
        //public TareaWrapper NuevaTarea
        //{
        //    get { return _NuevaTarea; }
        //    set { SetProperty(ref _NuevaTarea, value); }
        //}
        private void OnAñadirCooperantesComboBox()
        {
            Actividad.Cooperantes.Remove(Actividad.Cooperantes.Where(c => c.Nombre == "").FirstOrDefault());
            foreach (var cooperante in Actividad.Cooperantes)
            {
                var cooperNuevo = CooperantesSeleccionados.Where(c => c.Id == cooperante.Id).FirstOrDefault();
                if ((cooperNuevo.Nombre != null)||(cooperNuevo!=null))
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
        private bool OnAñadirCooperantesComboBox_CanExecute()
        {
            return true;
        }
        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }
        public string DescripcionNuevaTarea
        {
            get { return _DescripcionNuevaTarea; }
            set { SetProperty(ref _DescripcionNuevaTarea, value); }
        }
        public CooperanteWrapper ResponsableTarea
        {
            get { return _ResponsableTarea; }
            set
            {
                ((DelegateCommand)AñadirCooperantesComboBox).RaiseCanExecuteChanged();
                SetProperty(ref _ResponsableTarea, value);
            }
        }
        public DateTime? FechaFinTarea
        {
            get { return _FechaFinTarea; }
            set { SetProperty(ref _FechaFinTarea, value); }
        }
        public TareaWrapper Tarea { get; private set; }
        public ActividadWrapper Actividad { get; private set; }
        public ICommand AceptarCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }
        public ICommand AñadirCooperantesComboBox { get; private set; }
        public ObservableCollection<CooperanteWrapper> CooperantesSeleccionados { get; private set; }
        public ISession Session
        {
            get { return null; }
            set
            {
                _ActividadRepository.Session = value;
                _TareaRepository.Session = value;
            }
        }
        public void LoadActividad(ActividadWrapper actividad)
        {
            Actividad = actividad;
            if (CooperantesSeleccionados != null)
            {
                CooperantesSeleccionados.Clear();
            }
            
            foreach (var cooper3 in Actividad.Cooperantes)
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
            CooperantesSeleccionados.Add(new CooperanteWrapper(
                    new Cooperante()
                    {
                        Id = actividad.Coordinador.Id,
                        Dni = actividad.Coordinador.Dni,
                        Nombre = actividad.Coordinador.Nombre,
                        Apellido = actividad.Coordinador.Apellido
                    }));
            Actividad.AcceptChanges();
        }
        public void LoadTarea(TareaWrapper tarea) // Esto es necesario para cuando edito una tarea que tengo que pasar la informacion de la tarea
        {
            DescripcionNuevaTarea = tarea.Descripcion;
            ResponsableTarea = tarea.Responsable;
            FechaFinTarea = tarea.FechaDeFinalizacion;
            _ModificarTarea = 1;
            _TareaID = tarea.Id;
        }
        private void OnAceptarCommand_Execute()
        {
            if (_ModificarTarea == 0)
            {
                var NuevaTarea = (new TareaWrapper(new Business.Tarea()
                {
                    Descripcion = DescripcionNuevaTarea,
                    FechaDeFinalizacion = FechaFinTarea,
                    Responsable = ResponsableTarea.Model,
                    HaFinalizado = false,
                    Actividad = Actividad.Model
                })
                { SeguimientoVisible = false });
                var nuevoSeguimiento = new SeguimientoWrapper(new Seguimiento()
                {
                    Descripcion = "Trabajos realizados en la Tarea",
                    FechaDePublicacion = DateTime.Now,
                    Tarea = NuevaTarea.Model
                });
                var incidenciaTarea = (new IncidenciaWrapper(new Incidencia()
                {
                    Descripcion = "Problemas surgidos durante el desarrollo de la tarea",
                    FechaDePublicacion = DateTime.Now,
                    Tarea = NuevaTarea.Model
                }));
                NuevaTarea.Incidencias.Add(incidenciaTarea);
                NuevaTarea.Seguimiento.Add(nuevoSeguimiento);
                Actividad.Tareas.Add(NuevaTarea); 

                // Antes de nada comprobamos que no está el Cooperante Dumy en la Lista de cooperantes de la Actividad para que no intente añadirlo a BBDD
                Actividad.Cooperantes.Remove(Actividad.Cooperantes.Where(c => c.Nombre == "").FirstOrDefault());
                _ActividadRepository.Update(Actividad.Model);
                // Se ha añadido a la base de datos la nueva tarea => Informar a las vistas muestren las tareas para actualizar la lista que muestran.
                _EventAggregator.GetEvent<NuevaTareaCreadaEvent>().Publish(NuevaTarea);

                // Generar el evento asocioado a la creacion de la nueva tarea.
                var eventoDeActividad = new Evento()
                {
                    FechaDePublicacion = DateTime.Now,
                    Titulo = DescripcionNuevaTarea,
                    Ocurrencia = Ocurrencia.NUEVA_TAREA_PUBLICADA.ToString(),
                };
                _EventAggregator.GetEvent<PublicarEventosActividad>().Publish(eventoDeActividad);
                _ModificarTarea = 0;
            }
            else
            {
                Actividad.Tareas.Where(ident => ident.Id == _TareaID).First().Descripcion = DescripcionNuevaTarea;
                Actividad.Tareas.Where(ident => ident.Id == _TareaID).First().FechaDeFinalizacion = FechaFinTarea;
                Actividad.Tareas.Where(ident => ident.Id == _TareaID).First().Responsable = new CooperanteWrapper(new Cooperante());
                Actividad.Tareas.Where(ident => ident.Id == _TareaID).First().Responsable = ResponsableTarea;

                Actividad.UpdatedAt = DateTime.Now;
                _ActividadRepository.Update(Actividad.Model);
                Actividad.AcceptChanges();
                _EventAggregator.GetEvent<TareaModificadaEvent>().Publish(_TareaID);
            }
            Cerrar = true;
        }
        private bool OnAceptarCommand_CanExecute()
        {
            //return TituloForo != null && TituloForoMensaje != null;
            //var resultado = Actividad.IsChanged && Actividad.IsValid;
            //return resultado;
            return true;
        }
        private void OnCancelarCommand_Execute()
        {
            Actividad.RejectChanges();
            Cerrar = true;
        }
        private void InvalidateCommands()
        {
            ((DelegateCommand)AceptarCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)CancelarCommand).RaiseCanExecuteChanged();
        }
    }
}
