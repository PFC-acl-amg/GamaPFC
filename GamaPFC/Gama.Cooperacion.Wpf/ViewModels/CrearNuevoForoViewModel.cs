using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class CrearNuevoForoViewModel : ViewModelBase
    {
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM o hay excepciones por DialogCloser
        private IActividadRepository _ActividadRepository;
        private IEventAggregator _EventAggregator;
        private string _tituloForo;
        private string _tituloForoMensaje;
        public CrearNuevoForoViewModel(IActividadRepository ActividadRepository, IEventAggregator EventAggregator)
        {
            Foro = new ForoWrapper(new Foro());
            _ActividadRepository = ActividadRepository;
            _EventAggregator = EventAggregator;

            AceptarCommand = new DelegateCommand(OnAceptarCommand_Execute,
                OnAceptarCommand_CanExecute);
            CancelarCommand = new DelegateCommand(OnCancelarCommand_Execute);
        }
        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
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
        public ForoWrapper Foro { get; private set; }
        public ActividadWrapper Actividad { get; private set; }
        public ICommand AceptarCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }
        public ISession Session
        {
            get { return null; }
            set
            {
                _ActividadRepository.Session = value;
            }
        }
        public void Load(ActividadWrapper actividad)
        {
            Actividad = actividad;
            Actividad.AcceptChanges();
        }
        private void OnAceptarCommand_Execute()
        {
            var foroW = (new ForoWrapper(new Foro()
                { Titulo = TituloForo, FechaDePublicacion = DateTime.Now, Actividad = Actividad.Model })
                { ForoVisible = false });
            var mensajeForo = new MensajeWrapper(new Mensaje()
            {
                Titulo = TituloForoMensaje,
                FechaDePublicacion = DateTime.Now,
                Foro = foroW.Model
            });
            foroW.Mensajes.Add(mensajeForo);
            Actividad.Foros.Add(foroW);
            _ActividadRepository.Update(Actividad.Model);
            
            _EventAggregator.GetEvent<NuevoForoCreadoEvent>().Publish(foroW);
            var eventoDeActividad = new Evento()
            {
                FechaDePublicacion = DateTime.Now,
                Titulo = TituloForo,
                Ocurrencia = Ocurrencia.FORO_CREADO.ToString(),
            };
            _EventAggregator.GetEvent<PublicarEventosActividad>().Publish(eventoDeActividad);
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

    }// Fin => public class CrearNuevoForoViewModel : ViewModelBase
}    // Fin =>namespace Gama.Cooperacion.Wpf.ViewModels
