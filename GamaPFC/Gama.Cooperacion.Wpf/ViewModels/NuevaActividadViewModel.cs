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
            Actividad.PropertyChanged += Actividad_PropertyChanged;

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

        private void OnAceptarCommand()
        {
            // Eliminamos el cooperante dummy
            Actividad.Cooperantes.Remove(Actividad.Cooperantes.Where(c => c.Nombre == null).First());
            // Creamos el evento de nueva actividad creada
            var evento = new Evento()
            {
                Titulo = Actividad.Titulo,
                FechaDePublicacion = DateTime.Now,
                Ocurrencia = Ocurrencia.Nueva_Actividad,
            };
            // -------
            // Evento creado se añade a la actidivad
            Actividad.Model.AddEvento(evento);
            // ------
            // Crear Foro
            var foro = new Foro()
            {
                Titulo = "Primer Foro",
                FechaDePublicacion = DateTime.Now,   
            };
            var mensajeForo = new Mensaje()
            {
                Titulo = "Primer mensaje del foro",
                FechaDePublicacion = DateTime.Now,
            };
            foro.AddMensaje(mensajeForo);
            // -------
            // Evento creado se añade a la actidivad
            Actividad.Model.AddForo(foro);
            //----
            Actividad.CreatedAt = DateTime.Now;
            _ActividadRepository.Create(Actividad.Model);
            _EventAggregator.GetEvent<NuevaActividadEvent>().Publish(Actividad.Id);
            Cerrar = true;
        }

        private bool OnAceptarCommand_CanExecute()
        {
            var resultado = Actividad.Titulo != null && Actividad.Coordinador.Nombre != null;
            return resultado;
        }

        private void OnCancelarCommand()
        {
            Cerrar = true;
        }
    }
}
