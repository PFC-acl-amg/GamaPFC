using Prism.Unity;
using System;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Gama.Bootstrapper
{
    public enum Modulos
    {
        Cooperacion,
        GestionDeSocios,
        ServicioDeAtenciones,
    }

    public class Bootstrapper : UnityBootstrapper
    {
        private Modulos _moduloSeleccionado;

        public Bootstrapper(Modulos modulo) : base()
        {
            _moduloSeleccionado = modulo;
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            switch (_moduloSeleccionado)
            {
                case Modulos.Cooperacion:
                    break;
                case Modulos.GestionDeSocios:
                    break;
                case Modulos.ServicioDeAtenciones:
                    break;
                default:
                    break;
            }

            //Type cooperacionModuleType = typeof(Gama.Cooperacion.Wpf.CooperacionModule);
            //ModuleCatalog.AddModule(new ModuleInfo()
            //{
            //    ModuleName = cooperacionModuleType.Name,
            //    ModuleType = cooperacionModuleType.AssemblyQualifiedName,
            //    InitializationMode = InitializationMode.WhenAvailable
            //});
        }
    }
}
