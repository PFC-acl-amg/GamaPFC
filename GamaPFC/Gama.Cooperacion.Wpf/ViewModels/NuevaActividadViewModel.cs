using Core;
using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class NuevaActividadViewModel : ViewModelBase
    {
        private IActividadRepository _ActividadRepository;
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM, o hay excepción con DialogCloser
        private IEventAggregator _EventAggregator;
        private InformacionDeActividadViewModel _ActividadVM;
        private int _ModificarActividad;    // Controlar si se modifica una actividad o se crea una nueva

        public NuevaActividadViewModel(IActividadRepository actividadRepository,
            IEventAggregator eventAggregator,
            InformacionDeActividadViewModel actividadViewModel,
            ISession session)
        {
            _EventAggregator = eventAggregator;
            _ActividadVM = actividadViewModel;
            _ActividadRepository = actividadRepository;
            _ActividadRepository.Session = session;
            _ActividadVM._ActividadRepository = _ActividadRepository;
            Actividad.IsInEditionMode = true;
            Actividad.PropertyChanged += Actividad_PropertyChanged;
            _ModificarActividad = 0;

            AceptarCommand = new DelegateCommand(OnAceptarCommand, OnAceptarCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand);
        }

        private void Actividad_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ((DelegateCommand)AceptarCommand).RaiseCanExecuteChanged();
        }

        public InformacionDeActividadViewModel ActividadVM
        {
            get { return _ActividadVM; }
        }

        public ActividadWrapper Actividad => _ActividadVM.Actividad;

        // Para cerrar la ventana
        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }
        public ICommand AceptarCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }

        public void Load(Actividad actividad)
        {
            var wrapper = new ActividadWrapper(actividad);
            // Si la actividad no tenia cooperantes colocamos el dumy para poder añadir cooperantes a la actividad
            if (actividad.Cooperantes.Count == 0) wrapper.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
            _ModificarActividad = 1;
            _ActividadVM.Load(wrapper);
            _ActividadVM.Actividad.IsInEditionMode = true;
            Actividad.PropertyChanged += Actividad_PropertyChanged;
        }

        private void OnAceptarCommand()
        {
            if (_ModificarActividad == 0)   // Si es 0 se va a crear una nueva actividad
            {
                // Eliminamos el cooperante dummy
                // El valor de Nombre del Dumy es "", no es c.Nombre == null
                Actividad.Cooperantes.Remove(Actividad.Cooperantes.Where(c => c.Nombre == "").FirstOrDefault());
                // Creamos el evento de nueva actividad creada
                var evento = new Evento()
                {
                    Titulo = Actividad.Titulo,
                    FechaDePublicacion = DateTime.Now,
                    Ocurrencia = Ocurrencia.Nueva_Actividad.ToString(),
                };
                Actividad.Model.AddEvento(evento);   // Evento creado se añade a la actidivad
                var foro = new Foro()               // Crear Foro
                {
                    Titulo = "Primer Foro",
                    FechaDePublicacion = DateTime.Now,
                };
                var mensajeForo = new Mensaje()     // Primer Mensaje para el foro
                {
                    Titulo = "Primer mensaje del foro",
                    FechaDePublicacion = DateTime.Now,
                };
                foro.AddMensaje(mensajeForo);       // El mensaje se añade al foro  
                Actividad.Model.AddForo(foro);      // El foro se añade a la actividad
                Actividad.CreatedAt = DateTime.Now;
                _ActividadRepository.Create(Actividad.Model);   // Se crea la actividad
                _EventAggregator.GetEvent<ActividadCreadaEvent>().Publish(Actividad.Id);
                _EventAggregator.GetEvent<PublicarNuevaActividad>().Publish(evento);
                Cerrar = true;
            }
            else
            {
                if(_ModificarActividad== 1) // Si es 1 se va a modificar una actividad ya existente en la base de datos
                {
                    Actividad.Cooperantes.Remove(Actividad.Cooperantes.Where(c => c.Nombre == null).FirstOrDefault()); // Si se deja el cooperanre dumy falla insercion BBDD
                    Actividad.UpdatedAt = DateTime.Now;
                    _ActividadRepository.Update(Actividad.Model);
                    Actividad.AcceptChanges();
                    _ModificarActividad = 0;
                    _EventAggregator.GetEvent<ActividadActualizadaEvent>().Publish(Actividad.Id);
                    Cerrar = true;
                }
            }
            
        }

        private bool OnAceptarCommand_CanExecute()
        {
            // Para que el boton de Aceptarse muestre activo tieneque cumpplirse que se haya
            // escrito un titulo y se haya escogido a un coordinador
            //var resultado = Actividad.Titulo != null && Actividad.Coordinador.Nombre != null;
            var resultado = Actividad.IsChanged && Actividad.IsValid;
            return resultado;
        }

        private void OnCancelarCommand()
        {
            Cerrar = true;
        }
    }
}
