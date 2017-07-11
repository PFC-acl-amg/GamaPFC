using Core;
using Gama.Common.Debug;
using Gama.Cooperacion.Wpf.Eventos;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class StatusBarViewModel : ViewModelBase
    {
        private const string DefaultMensaje = "Barra de estado...";
        private IEventAggregator _EventAggregator;
        private string _Mensaje;
        private bool _ActivarFondo;
        private DispatcherTimer _Timer;

        public StatusBarViewModel(
           IEventAggregator eventAggregator)
        {
            Debug.StartWatch();
            _EventAggregator = eventAggregator;

            _EventAggregator.GetEvent<ActividadActualizadaEvent>().Subscribe(
                (id) => MostrarMensaje("La persona se ha actualizado con éxito."));

            _EventAggregator.GetEvent<NuevoCooperanteCreadoEvent>().Subscribe(
                (id) => MostrarMensaje("Nuevo Cooperante creado con éxito"));

            _EventAggregator.GetEvent<CooperanteModificadoEvent>().Subscribe(
                (id) => MostrarMensaje("Cooperante modificado con éxito"));

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

            //_EventAggregator.GetEvent<BackupFinalizadoEvent>().Subscribe(
            //    () => MostrarMensaje("La copia de seguridad se ha realizado con éxito."));

            Mensaje = DefaultMensaje;
            _Timer = new DispatcherTimer();
            _Timer.Tick += _timer_Tick;
            _Timer.Interval = new TimeSpan(0, 0, 4);
            Debug.StopWatch("StatusBar");
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            ActivarFondo = false;
            Mensaje = DefaultMensaje;
        }
        public string Mensaje
        {
            get { return _Mensaje; }
            set { SetProperty(ref _Mensaje, value); }
        }
        public bool ActivarFondo
        {
            get { return _ActivarFondo; }
            set { SetProperty(ref _ActivarFondo, value); }
        }
        private void MostrarMensaje(string mensaje)
        {
            Mensaje = mensaje;
            ActivarFondo = true;
            _Timer.Start();
        }
        private void OnActividadActualizadaEvent(int obj)
        {
            Mensaje = "La actividad se ha actualizado con éxito.";
            ActivarFondo = true;
            _Timer.Start();
        }
    }
}
