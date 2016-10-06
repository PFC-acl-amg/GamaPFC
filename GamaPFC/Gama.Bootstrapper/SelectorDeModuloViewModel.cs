using Core;
using Gama.Bootstrapper.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Bootstrapper
{
    public class SelectorDeModuloViewModel : ViewModelBase
    {
        private ILoginService _LoginService;
        private bool? _Cerrar; // Debe ser nulo al inicializarse el VM, o hay excepción con DialogCloser
        private bool _MostrarLogin;

        public SelectorDeModuloViewModel(ILoginService loginService)
        {
            _LoginService = loginService;

            SeleccionarModuloCommand = new DelegateCommand<string>(OnSeleccionarModuloCommandExecute);
            AccederCommand = new DelegateCommand(OnAccederCommandExecute);

            this.Usuario = "atenciones";
            this.Password = "secret";

            SeHaAccedido = false;

            SeleccionarModuloCommand.Execute("atenciones");
        }

        public ICommand SeleccionarModuloCommand { get; private set; }
        public ICommand AccederCommand { get; private set; }

        public Modulos? ModuloSeleccionado { get; private set; }

        public bool? Cerrar
        {
            get { return _Cerrar; }
            set { SetProperty(ref _Cerrar, value); }
        }

        public bool MostrarLogin
        {
            get { return _MostrarLogin; }
            set { SetProperty(ref _MostrarLogin, value); }
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

        private void OnAccederCommandExecute()
        {
            if (_LoginService.CheckCredentials(Usuario, Password))
            {
                SeHaAccedido = true;
                Cerrar = true;
            }
        }
    }
}
