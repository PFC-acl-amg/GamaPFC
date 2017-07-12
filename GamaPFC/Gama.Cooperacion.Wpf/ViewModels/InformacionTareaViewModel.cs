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
        private bool? _Cerrar;
        private IActividadRepository _ActividadRepository;
        private IEventAggregator _EventAggregator;
        private string _NuevaIncidencia;
        private TareaWrapper _TareaSeleccionada;
        // Fin Zona
        // Constructor de la clase
        public InformacionTareaViewModel(IActividadRepository ActividadRepository, IEventAggregator EventAggregator)
        {
            _ActividadRepository = ActividadRepository;
            _EventAggregator = EventAggregator;

            AceptarIncidenciaCommand = new DelegateCommand(OnAceptarIncidenciaCommand_Execute,
                OnAceptarIncidenciaCommand_CanExecute);
        }
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
        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }
       
        public string NuevaIncidencia
        {
            get { return _NuevaIncidencia; }
            set { SetProperty(ref _NuevaIncidencia, value); }
        }

        public ICommand AceptarIncidenciaCommand { get; private set; }
        private bool OnAceptarIncidenciaCommand_CanExecute()
        {
            return true;
        }
        private void OnAceptarIncidenciaCommand_Execute()
        {
            var tareaSeleccionada = Actividad.Tareas.Where(x => x.Id == TareaSeleccionada.Id).First();
            var nuevaIncidencia = new IncidenciaWrapper(new Incidencia()
            {
                Descripcion = NuevaIncidencia,
                FechaDePublicacion = DateTime.Now,
                Tarea = tareaSeleccionada.Model
            });
            tareaSeleccionada.Incidencias.Add(nuevaIncidencia);
            _ActividadRepository.Update(Actividad.Model);
            TareaSeleccionada.Incidencias.Insert(0, nuevaIncidencia);
            var eventoDeActividad = new Evento()
            {
                FechaDePublicacion = DateTime.Now,
                Titulo = TareaSeleccionada.Descripcion,
                Ocurrencia = Ocurrencia.INCIDENCIA_EN_TAREA.ToString(),
            };
            _EventAggregator.GetEvent<PublicarEventosActividad>().Publish(eventoDeActividad);
            NuevaIncidencia = null;
            Actividad.AcceptChanges();
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
