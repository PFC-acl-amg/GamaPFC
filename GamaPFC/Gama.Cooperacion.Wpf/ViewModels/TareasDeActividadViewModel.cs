using Core;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class TareasDeActividadViewModel : ViewModelBase
    {
        private IActividadRepository _actividadRepository;
        private int _CantidadMensajes;
        private string _tituloForo;
        private string _tituloForoMensaje;
        private bool _VisibleCrearForo;
        private bool _OcultarCrearForo;
        private bool _VisibleCrearTarea;
        private bool _VisibleMensajeForo;
        private bool _VisibleCrearSeguimiento;
        private string _NuevoMensajeForo;
        private ActividadWrapper _Actividad;
        public IActividadRepository _ActividadRepository { get; set; }
        private IEventoRepository _EventoRepository;
        private IEventAggregator _EventAggregator;
        private bool _PopupEstaAbierto = false;
        private ForoWrapper _ForoSeleccionado;

        public TareasDeActividadViewModel(
            IActividadRepository actividadRepository,
            IEventoRepository eventoRepository,
            IEventAggregator eventAggregator, ISession session)     // Constructor de la clase
        {
            _VisibleCrearForo = false;
            _OcultarCrearForo = true;
            _VisibleMensajeForo = false;
            _EventoRepository = eventoRepository;
            _actividadRepository = actividadRepository;
            _actividadRepository.Session = session;
            _EventoRepository.Session = session;
            _EventAggregator = eventAggregator;

            MensajesDisponibleEnForo = new ObservableCollection<Mensaje>();
            ForosDisponibles = new ObservableCollection<ForoWrapper>();
            EventoActividad = new ObservableCollection<Evento>();

            _EventAggregator.GetEvent<CargarNuevaActividadEvent>().Subscribe(OnCargarNuevaActividadEvent);
            _EventAggregator.GetEvent<NuevoForoCreadoEvent>().Subscribe(OnNuevoForoCreadoEvent);
            _EventAggregator.GetEvent<PublicarEventosActividad>().Subscribe(OnPublicarEventosActividad);
            CrearForoCommand = new DelegateCommand(OnCrearForoCommand, OnCrearForoCommand_CanExecute);
            MensajesForoCommand = new DelegateCommand<object>(OnMensajeForoCommand, OnMensajeForoCommand_CanExecute);
            AceptarCrearForoCommand = new DelegateCommand(OnAceptarCrearForoCommand, OnAceptarCrearForoCommand_CanExecute);
            AceptarNuevoMensajeCommand = new DelegateCommand<object>(OnAceptarNuevoMensajeCommand, OnAceptarNuevoMensajeCommand_CanExecute);

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
        public string NuevoMensajeForo
        {
            get { return _NuevoMensajeForo; }
            set { SetProperty(ref _NuevoMensajeForo, value); }
        }
        public ICommand AceptarCrearForoCommand { get; private set; }
        public ICommand AceptarNuevoMensajeCommand { get; private set; }
        public ICommand MensajesForoCommand { get; private set; }
        public ICommand CrearForoCommand { get; private set; }
        private void OnMensajeForoCommand(object wrapper)
        {
            ((ForoWrapper)wrapper).ForoVisible = !((ForoWrapper)wrapper).ForoVisible;
        }
        private void OnAceptarCrearForoCommand()
        {
            var mensajeForo = new MensajeWrapper(new Mensaje() {
                Titulo = TituloForoMensaje,
                FechaDePublicacion = DateTime.Now,
            });
            var foroW= (new ForoWrapper(new Foro()
                                        { Titulo = TituloForo, FechaDePublicacion = DateTime.Now})
                                        { ForoVisible = true });
            foroW.Mensajes.Add(mensajeForo);
            Actividad.Model.AddForo(foroW.Model);
            _EventAggregator.GetEvent<NuevoForoCreadoEvent>().Publish(foroW);
            AuxiliarOnPublicarEventosActividad(Ocurrencia.FORO_CREADO, TituloForo);
            CrearForoVisible = false;
            OcultarForoVisible = true;
        }
        private void OnAceptarNuevoMensajeCommand(object wrapper)
        {
            var nuevoMensaje = new MensajeWrapper(new Mensaje()
            {
                Titulo = NuevoMensajeForo,
                FechaDePublicacion = DateTime.Now,
            });
            //((ForoWrapper)wrapper).Mensajes.Add(nuevoMensaje);
            ((ForoWrapper)wrapper).Mensajes.Insert(0,nuevoMensaje);
            AuxiliarOnPublicarEventosActividad(Ocurrencia.MENSAJE_PUBLICADO_EN_FORO,((ForoWrapper)wrapper).Titulo);
            NuevoMensajeForo = null;
        }
        private bool OnAceptarNuevoMensajeCommand_CanExecute(object wrapper)
        {
            return true;
        }
        private bool OnMensajeForoCommand_CanExecute(object wrapper)
        {
            return true;
        }
        private bool OnAceptarCrearForoCommand_CanExecute()
        {
            return true;
        }
        public ActividadWrapper Actividad
        {
            get { return _Actividad; }
            set { SetProperty(ref _Actividad, value); }
        }
        public void LoadActividad(ActividadWrapper wrapper)
        {
            Actividad = wrapper;
            Actividad.AcceptChanges();
            _EventAggregator.GetEvent<CargarNuevaActividadEvent>().Publish(Actividad.Id);    
        }
        public ObservableCollection<ForoWrapper> ForosDisponibles { get; private set; }
        public ObservableCollection<Mensaje> MensajesDisponibleEnForo { get; private set; }
        public ObservableCollection<Evento> EventoActividad { get; private set; }
        public ForoWrapper ForoSelecionado { get; set; }
        public bool CrearForoVisible
        {
            get { return _VisibleCrearForo; }
            set { SetProperty(ref _VisibleCrearForo, value); }
        }
        public bool MensajeForoVisible
        {
            get { return _VisibleMensajeForo; }
            set { SetProperty(ref _VisibleMensajeForo, value); }
        }
        public bool OcultarForoVisible
        {
            get { return _OcultarCrearForo; }
            set { SetProperty(ref _OcultarCrearForo, value); }
        }
        private bool OnCrearForoCommand_CanExecute()
        {
            return true;
        }
        private void OnCargarNuevaActividadEvent(int id)
        {
            var actividad = _actividadRepository.GetById(id);
            foreach (var forito in actividad.Foros)
            {
                ForosDisponibles.Add(new ForoWrapper(
                    new Foro() {
                        Titulo = forito.Titulo,
                        FechaDePublicacion = forito.FechaDePublicacion,
                        Mensajes = forito.Mensajes })
                    { ForoVisible = false });
            }
            // Recorrido para obtener los eventos publicados en la Actividad cargada
            foreach (var evento in actividad.Eventos)
            {
                EventoActividad.Add(evento);
            }

        }
        private void OnNuevoForoCreadoEvent(ForoWrapper forito)
        {
            ForosDisponibles.Insert(0, (new ForoWrapper(new Foro()
            { Titulo = forito.Titulo, FechaDePublicacion = forito.FechaDePublicacion, Mensajes=forito.Model.Mensajes })
            { ForoVisible = true }));
        }
        private void AuxiliarOnPublicarEventosActividad(Ocurrencia tipoEvento,string tituloEvento)
        {
            var eventoDeActividad = new Evento()
            {
                FechaDePublicacion = DateTime.Now,
                Titulo = tituloEvento,
                Ocurrencia = tipoEvento,
            };
            _EventAggregator.GetEvent<PublicarEventosActividad>().Publish(eventoDeActividad);
        }
        private void OnPublicarEventosActividad(Evento GenerarEvento)
        {
            EventoActividad.Insert(0, GenerarEvento);
            // Falta código para BBDD
            //Actividad.Model.AddEvento(GenerarEvento);
        }
        public List<EventoWrapper> EventosDisponibles { get; private set; }

        private void OnCrearForoCommand()
        {
            if (CrearForoVisible == false)
            {
                CrearForoVisible = true;
                OcultarForoVisible = false;
            }
            else
            {
                CrearForoVisible = false;
                OcultarForoVisible = true;
            }
        }

 
    }   // Clase TareasDeActividadVM
}       // Fin namespace
