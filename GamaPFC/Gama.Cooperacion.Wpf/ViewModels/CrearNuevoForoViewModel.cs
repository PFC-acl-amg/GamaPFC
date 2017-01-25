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
// 1º => Se convierte la clase CrearNuevoForoViewModel en publica y que herede de la clase ViewModelBase
// para ello tenemos que añadir tambien el using core porque esta clase esta definida en ese espacio
// 2º => Los atributos privados de la clase necesitamos:
// * _Cerrarpra controlar el cierre de la ventana => Esta estará asociada a Cerrar que neceitamos definirla propertyChange
// *_actividadRepository para realizar el acceso a BBDD que necesita using Gama.Cooperacion.Wpf.Services
// *_EventAggregator para poder anunciar que se ha creado un foro al group box de tareasActividadViewModel donde se muestran los foros
// ---- y necesita using Prims.Event
// * pendiente de si falta algo mas.
// 3º => Creamos el constructor de la clase public CrearNuevoForoViewModel. Por lo pronto dos parametro de entrada ActividadRepository
// ---- y EventAggregator
// * Creamos un foro vacio por lo pronto Foro = new ForoWrapper(new Foro()); y neceita public ForoWrapper Foro { get; private set; }
// * Hacemos las asignaciones de las variables privadas _actividadRwpository y _EventAggregator
// * Definimos los delegateCommand (using prims.Commands) para los dos botones aceptar y cancelar usando Icommand
// 4º => La variable Session se pasa desde TareasDeActividadViewModel y aqui hay que definir su get y set (using Nhibernate)
// Como modificamos la Actividad necesitamos pasarla a este ViewModel para ello usamos Load que se llamara desde el boton 
//---- para cargar la ventana
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
            // Por lo pronto probamos con estos dos lineas porque creo que no las necesito
            // Con tener Session creo que es suficiente para poder guardar el foro => ERROR si no necesito porque tengo que
            // usar Actividad.Foros.Add(Foro); antes de llamar a actividadRepository
            // Foro.Actividad = actividad; //Para que no de error en ForoWrapper Actividad tiene que tener set sin private
        }
        private void OnAceptarCommand_Execute()
        {
           var mensajeForo = new MensajeWrapper(new Mensaje()
           {
               Titulo = TituloForoMensaje,
               FechaDePublicacion = DateTime.Now,
           });
            var foroW = (new ForoWrapper(new Foro()
                { Titulo = TituloForo, FechaDePublicacion = DateTime.Now })
                { ForoVisible = true });
            foroW.Model.AddMensaje(mensajeForo.Model);
            Actividad.Model.AddForo(foroW.Model);
            _ActividadRepository.Update(Actividad.Model);
            _EventAggregator.GetEvent<NuevoForoCreadoEvent>().Publish(foroW);
            var eventoDeActividad = new Evento()
            {
                FechaDePublicacion = DateTime.Now,
                Titulo = TituloForo,
                Ocurrencia = Ocurrencia.FORO_CREADO,
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
