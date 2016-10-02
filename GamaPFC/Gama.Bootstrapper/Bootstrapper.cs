using Prism.Unity;
using System;
using Microsoft.Practices.Unity;
using System.Windows;
using Prism.Modularity;

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
                    Type cooperacionModuleType = typeof(Gama.Cooperacion.Wpf.CooperacionModule);
                    ModuleCatalog.AddModule(new ModuleInfo()
                    {
                        ModuleName = cooperacionModuleType.Name,
                        ModuleType = cooperacionModuleType.AssemblyQualifiedName,
                        InitializationMode = InitializationMode.WhenAvailable
                    });
                    break;
                case Modulos.GestionDeSocios:
                    break;
                case Modulos.ServicioDeAtenciones:
                    Type atencionesModuleType = typeof(Gama.Atenciones.Wpf.AtencionesModule);
                    var moduleInfo = new ModuleInfo()
                    {
                        ModuleName = atencionesModuleType.Name,
                        ModuleType = atencionesModuleType.AssemblyQualifiedName,
                        InitializationMode = InitializationMode.WhenAvailable
                    };
                    ModuleCatalog.AddModule(moduleInfo);
                    break;
                default:
                    break;
            }
        }
    }
}
