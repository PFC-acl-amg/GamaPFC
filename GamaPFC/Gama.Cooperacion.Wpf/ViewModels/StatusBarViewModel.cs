using Core;
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
        private const string _DefaultMensaje = "Barra de estado...";
        private IEventAggregator _eventAggregator;
        private string _Mensaje;
        private bool _ActivarFondo;
        private DispatcherTimer _timer;

        public StatusBarViewModel(
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<ActividadActualizadaEvent>().Subscribe(OnActividadActualizadaEvent);

            Mensaje = _DefaultMensaje;
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 2);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            ActivarFondo = false;
            Mensaje = _DefaultMensaje;
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

        private void OnActividadActualizadaEvent(int obj)
        {
            Mensaje = "La actividad se ha actualizado con éxito.";
            ActivarFondo = true;
            _timer.Start();
        }
    }
}
