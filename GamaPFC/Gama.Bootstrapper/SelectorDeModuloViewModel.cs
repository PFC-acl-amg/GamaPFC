using Core;
using Gama.Bootstrapper.Services;
using Gama.Common.Eventos;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Gama.Bootstrapper
{
    public enum Modulos
    {
        Cooperacion,
        GestionDeSocios,
        ServicioDeAtenciones,
    }
    public class SelectorDeModuloViewModel : ObservableObject
    {
        private LoginService _LoginService;
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM, o hay excepción con DialogCloser
        private bool _MostrarLogin;
        private IEventAggregator _EventAggregator;

        public SelectorDeModuloViewModel(LoginService loginService, IEventAggregator eventAggregator)
        {
            _LoginService = loginService;
            _EventAggregator = eventAggregator;

            SeleccionarModuloCommand = new DelegateCommand<string>(OnSeleccionarModuloCommandExecute);
            AccederCommand = new DelegateCommand(OnAccederCommandExecute);

            SeHaAccedido = false;

            Usuario = "admin";
            Password = "clave";
            //SeleccionarModuloCommand.Execute("atenciones");

            _EventAggregator.GetEvent<VolverASeleccionDeModuloEvent>().Subscribe(OnVolverASeleccionDeModuloEvent);

            _Timer = new DispatcherTimer();
            _Timer.Tick += _timer_Tick;
            _Timer.Interval = new TimeSpan(0, 0, 2);
        }

        private void OnVolverASeleccionDeModuloEvent()
        {
            VolverASeleccionDeModulo = true;
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            //Application.Current.Shutdown();
        }

        public ICommand SeleccionarModuloCommand { get; private set; }
        public ICommand AccederCommand { get; private set; }
        public bool VolverASeleccionDeModulo { get; set; }

        public Modulos ModuloSeleccionado { get; private set; }

        public bool? Cerrar
        {
            get { return _Cerrar; }
            set {
                _Cerrar = value;
                OnPropertyChanged(nameof(Cerrar));
            }
        }

        public bool MostrarLogin
        {
            get { return _MostrarLogin; }
            set
            {
                _MostrarLogin = value;
                OnPropertyChanged(nameof(MostrarLogin));
            }
        }

        public string Usuario { get; set; }
        public string Password { get; set; }
        public bool SeHaAccedido { get; private set; }

        private void OnSeleccionarModuloCommandExecute(string modulo)
        {
            switch (modulo)
            {
                case "atenciones":
                    ModuloSeleccionado = Modulos.ServicioDeAtenciones;
                    break;
                case "socios":
                    ModuloSeleccionado = Modulos.GestionDeSocios;
                    break;
                case "cooperacion":
                    ModuloSeleccionado = Modulos.Cooperacion;
                    break;
                default:
                    throw new Exception("¡El módulo seleccionado no existe!");
            }

            MostrarLogin = true;
        }

        private bool _HayErrores;
        public bool HayErrores
        {
            get { return _HayErrores; }
            set { SetProperty(ref _HayErrores, value); }
        }


        private DispatcherTimer _Timer;

        private void OnAccederCommandExecute()
        {
            if (_LoginService.CheckCredentials(ModuloSeleccionado, Usuario, Password))
            {
                SeHaAccedido = true;
                Cerrar = true;
            }
            else
            {
                HayErrores = true;
                _Timer.Start();
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            HayErrores = false;
        }
    }
}
