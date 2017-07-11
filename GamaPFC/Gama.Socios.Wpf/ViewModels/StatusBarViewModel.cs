using Core;
using Gama.Common.Debug;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Eventos;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Gama.Socios.Wpf.ViewModels
{
    public class StatusBarViewModel : ViewModelBase
    {
        public static string DefaultMensaje = "Barra de estado...";
        private IEventAggregator _EventAggregator;
        private string _Mensaje;
        private bool _ActivarFondo;
        private DispatcherTimer _Timer;
        public StatusBarViewModel(
           IEventAggregator eventAggregator)
        {
            Debug.StartWatch();
            _EventAggregator = eventAggregator;

            //_EventAggregator.GetEvent<PersonaActualizadaEvent>().Subscribe(
            //    (id) => MostrarMensaje("La persona se ha actualizado con éxito."));

            //_EventAggregator.GetEvent<PersonaEliminadaEvent>().Subscribe(
            //    (id) => MostrarMensaje("La persona y todos sus registros han sido eliminados con éxito."));

            //_EventAggregator.GetEvent<AtencionActualizadaEvent>().Subscribe(
            //    (id) => MostrarMensaje("La atención se ha actualizado con éxito"));

            //_EventAggregator.GetEvent<AsistenteCreadoEvent>().Subscribe(
            //    (id) => MostrarMensaje("El asistente se ha creado con éxito"));

            //_EventAggregator.GetEvent<AsistenteActualizadoEvent>().Subscribe(
            //    (id) => MostrarMensaje("El asistente se ha actualizado con éxito"));

            //_EventAggregator.GetEvent<CitaCreadaEvent>().Subscribe(
            //    (id) => MostrarMensaje("La cita se ha añadido con éxito."));

            //_EventAggregator.GetEvent<CitaActualizadaEvent>().Subscribe(
            //    (id) => MostrarMensaje("La cita se ha actualizado con éxito."));

            //_EventAggregator.GetEvent<CitaEliminadaEvent>().Subscribe(
            //    (id) => MostrarMensaje("La cita se ha eliminado con éxito."));

            _EventAggregator.GetEvent<BackupFinalizadoEvent>().Subscribe(
                () => MostrarMensaje("La copia de seguridad se ha realizado con éxito."));

            Mensaje = DefaultMensaje;
            _Timer = new DispatcherTimer();
            _Timer.Tick += _timer_Tick;
            _Timer.Interval = new TimeSpan(0, 0, 4);
            Debug.StopWatch("StatusBar");
        }
        //public StatusBarViewModel(
        //    IEventAggregator eventAggregator)
        //{
        //    _EventAggregator = eventAggregator;

        //    _EventAggregator.GetEvent<SocioActualizadoEvent>().Subscribe(OnSocioActualizadoEvent);

        //    Mensaje = DefaultMensaje;
        //    _Timer = new DispatcherTimer();
        //    _Timer.Tick += _timer_Tick;
        //    _Timer.Interval = new TimeSpan(0, 0, 2);
        //}

        public string Mensaje
        {
            get { return _Mensaje; }
            set { SetProperty(ref _Mensaje, value); }
        }
        private void MostrarMensaje(string mensaje)
        {
            Mensaje = mensaje;
            ActivarFondo = true;
            _Timer.Start();
        }
        public bool ActivarFondo
        {
            get { return _ActivarFondo; }
            set { SetProperty(ref _ActivarFondo, value); }
        }

        private void OnSocioActualizadoEvent(Socio obj)
        {
            Mensaje = "El socio se ha actualizado con éxito.";
            ActivarFondo = true;
            _Timer.Start();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            ActivarFondo = false;
            Mensaje = DefaultMensaje;
        }
    }
}
