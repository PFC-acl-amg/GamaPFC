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
        private string _NuevoSeguimiento;
        private TareaWrapper _TareaSeleccionada;
        public SeguimientoTareaViewModel(IActividadRepository ActividadRepository, IEventAggregator EventAggregator)
        {
            _ActividadRepository = ActividadRepository;
            _EventAggregator = EventAggregator;
            
            AceptarSeguimientoCommand = new DelegateCommand(OnAceptarSeguimientoCommand_Execute,
                OnAceptarSeguimientoCommand_CanExecute);
        }
        //--------------------------------
        //Contenedores (ObservableCollections, lists,...
        //--------------------------------
        public ActividadWrapper Actividad { get; private set; }
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
        //--------------------------------
        // ICommands
        //--------------------------------
        public ICommand AceptarSeguimientoCommand { get; private set; }

        //-----------------------------------
        // ICommands Implementaciones
        //-----------------------------------
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
                Ocurrencia = Ocurrencia.SEGUIMIENGO_EN_TAREA.ToString(),
            };
            _EventAggregator.GetEvent<PublicarEventosActividad>().Publish(eventoDeActividad);
            NuevoSeguimiento = null;
            Actividad.AcceptChanges();
        }
        private bool OnAceptarSeguimientoCommand_CanExecute()
        {
            return true;
        }

        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }
       
        public string NuevoSeguimiento
        {
            get { return _NuevoSeguimiento; }
            set { SetProperty(ref _NuevoSeguimiento, value); }
        }
        
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
