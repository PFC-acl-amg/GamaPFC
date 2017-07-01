using Core;
using Core.Util;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Views;
using Gama.Common.Debug;
using Gama.Common.Eventos;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class RightCommandsViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private const string MensajeBaseNoConectado = "Estado: No conectado";
        private const string MensajeBaseIntentandoConectar = "Estado: No conectado. Intentando reconectar en ";
        private const string MensajeBaseConectado = "Estado: Conectado";
        private DispatcherTimer _Timer;
        private int _ContadorDeSegundosParaReintentarConexion = 0;
        private BackgroundWorker _BackgroundWorker;

        //public RightCommandsViewModel()
        //{
        //    MensajeAMostrar = MensajeBaseNoConectado;
        //    _Timer = new DispatcherTimer();
        //    _Timer.Tick += _timer_Tick;
        //    _Timer.Interval = new TimeSpan(0, 0, 1);
        //}

        public RightCommandsViewModel(EventAggregator eventAggregator)
        {
            Debug.StartStopWatch();
            _EventAggregator = eventAggregator;
            _BackgroundWorker = new BackgroundWorker();
            _BackgroundWorker.DoWork += BackgroundWorker_DoWork;

            _Timer = new DispatcherTimer();
            _Timer.Tick += _timer_Tick;
            _Timer.Interval = new TimeSpan(0, 0, 1);

            if (AtencionesResources.ClientService != null && AtencionesResources.ClientService.IsConnected())
            {
                EstaConectadoAlServidor = true;
                MensajeAMostrar = MensajeBaseConectado;
            }
            else
            {
                EstaConectadoAlServidor = false;
                MensajeAMostrar = MensajeBaseNoConectado;
                _Timer.Start();
            }

            AbrirPreferenciasCommand = new DelegateCommand(OnAbrirPreferenciasCommandExecute);
            VolverASeleccionDeModuloCommand = new DelegateCommand(OnVolverASeleccionDeModuloExecute);

            _EventAggregator.GetEvent<LaConexionConElServidorHaCambiadoEvent>().Subscribe(OnLaConexionConElServidorHaCambiadoEvent);
            Debug.StopWatch("RightCommandsView");
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            TryReconnect();
        }

        public ICommand AbrirPreferenciasCommand { get; private set; }
        public ICommand VolverASeleccionDeModuloCommand { get; private set; }

        private string _MensajeAMostrar;
        public string MensajeAMostrar
        {
            get { return _MensajeAMostrar; }
            set { SetProperty(ref _MensajeAMostrar, value); }
        }

        private bool _EstaConectadoAlServidor;
        public bool EstaConectadoAlServidor
        {
            get { return _EstaConectadoAlServidor; }
            set { SetProperty(ref _EstaConectadoAlServidor, value); }
        }

        private void OnAbrirPreferenciasCommandExecute()
        {
            var o = new PreferenciasView();
            o.ShowDialog();
        }

        private void OnVolverASeleccionDeModuloExecute()
        {
            UIServices.RestartApplication();
        }

        private void OnLaConexionConElServidorHaCambiadoEvent(MensajeDeConexion mensajeDeConexion)
        {
            if (mensajeDeConexion == MensajeDeConexion.Conectado)
            {
                MensajeAMostrar = MensajeBaseConectado;
                EstaConectadoAlServidor = true;
            }
            else if (mensajeDeConexion == MensajeDeConexion.NoConectado)
            {
                EstaConectadoAlServidor = false;
                MensajeAMostrar = MensajeBaseNoConectado;
                if (_Timer != null)
                    _Timer.Start();
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _ContadorDeSegundosParaReintentarConexion--;
            MensajeAMostrar = MensajeBaseIntentandoConectar + $"{_ContadorDeSegundosParaReintentarConexion} segundos";
            if (_ContadorDeSegundosParaReintentarConexion <= 0)
            {
                MensajeAMostrar = "Estado: No conectado. Intentando reconectar en estos momentos...";
                _Timer.Stop();
                _ContadorDeSegundosParaReintentarConexion = 5;
                _BackgroundWorker.RunWorkerAsync();
            }
        }

        private void TryReconnect()
        {
            AtencionesResources.ClientService.TryConnect();
        }
    }
}
