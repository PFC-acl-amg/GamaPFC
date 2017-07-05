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

            CooperantesSeleccionados = new ObservableCollection<CooperanteWrapper>();
            AceptarCommand = new DelegateCommand(OnAceptarCommand_Execute,
                OnAceptarCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand_Execute);
            AñadirCooperantesComboBox = new DelegateCommand(OnAñadirCooperantesComboBox, OnAñadirCooperantesComboBox_CanExecute);
            Gama.Common.Debug.Debug.StopWatch("CrearNuevaTareaViewModel");
        }
        //public TareaWrapper NuevaTarea
        //{
        //    get { return _NuevaTarea; }
        //    set { SetProperty(ref _NuevaTarea, value); }
        //}
        private void OnAñadirCooperantesComboBox()
        {
            foreach (var cooperante in Actividad.Cooperantes)
            {
                var cooperNuevo = CooperantesSeleccionados.Where(c => c.Id == cooperante.Id).First();
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
                var nuevoSeguimiento = new SeguimientoWrapper(new Seguimiento()
                {
                    Descripcion = "Historial de Seguimiento de la Tarea",
                    FechaDePublicacion = DateTime.Now,
                });
                var incidenciaTarea = (new IncidenciaWrapper(new Incidencia()
                {
                    Descripcion = "Historial de incidencias durante desarrollo de la tarea",
                    FechaDePublicacion = DateTime.Now,
                }));
                var NuevaTarea = (new TareaWrapper(new Tarea()
                {
                    Descripcion = DescripcionNuevaTarea,
                    FechaDeFinalizacion = FechaFinTarea,
                    Responsable = ResponsableTarea.Model,
                    HaFinalizado = false,
                    Actividad = Actividad.Model
                })
                { SeguimientoVisible = false });
                NuevaTarea.Responsable = ResponsableTarea;
                NuevaTarea.Descripcion = DescripcionNuevaTarea;
                NuevaTarea.FechaDeFinalizacion = FechaFinTarea;
                NuevaTarea.Responsable = ResponsableTarea;
                NuevaTarea.HaFinalizado = false;
                NuevaTarea.Actividad = Actividad;
                NuevaTarea.Incidencias.Add(incidenciaTarea);
                NuevaTarea.Seguimiento.Add(nuevoSeguimiento);
                Actividad.Tareas.Add(NuevaTarea); // Como Tareas es un Changetrakincolection cuando modificas tareas tambien modificael .model
                //Por lo que no tengo que hacer Actividad.Model.AddTarea(NuevaTarea.Model);

                _TareaRepository.Create(NuevaTarea.Model);
                //_ActividadRepository.Update(Actividad.Model);

                _EventAggregator.GetEvent<NuevaTareaCreadaEvent>().Publish(NuevaTarea);
                var eventoDeActividad = new Evento()
                {
                    FechaDePublicacion = DateTime.Now,
                    Titulo = DescripcionNuevaTarea,
                    Ocurrencia = Ocurrencia.NUEVA_TAREA_PUBLICADA,
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
