using Prism.Unity;
using System;
using Microsoft.Practices.Unity;
using System.Windows;
using Prism.Modularity;
using System.Windows.Media.Imaging;

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
        public Modulos ModuloSeleccionado {get; set; }

        public Bootstrapper(Modulos modulo) : base()
        {
            ModuloSeleccionado = modulo;
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            string title = "";
            BitmapImage icon = new BitmapImage();
            switch(ModuloSeleccionado)
            {
                case Modulos.Cooperacion:
                    title = "MÓDULO DE COOPERACIÓN";
                    icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/icono_modulo_cooperacion.png"));
                    break;
                case Modulos.ServicioDeAtenciones:
                    title = "SERVICIO DE ATENCIONES";
                    icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/icono_modulo_atenciones.png"));
                    break;
                case Modulos.GestionDeSocios:
                    title = "GESTIÓN DE SOCIOS";
                    icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/icono_modulo_socios.png"));
                    break;
                default:
                    throw new Exception("¡El módulo no existe!");
            }

            ((ShellViewModel)((FrameworkElement)Shell).DataContext).Title = title;
            ((ShellViewModel)((FrameworkElement)Shell).DataContext).IconSource = icon;

            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            switch (ModuloSeleccionado)
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
                    Type sociosModuleType = typeof(Gama.Socios.Wpf.SociosModule);
                    ModuleCatalog.AddModule(new ModuleInfo()
                    {
                        ModuleName = sociosModuleType.Name,
                        ModuleType = sociosModuleType.AssemblyQualifiedName,
                        InitializationMode = InitializationMode.WhenAvailable
                    });
                    break;
                case Modulos.ServicioDeAtenciones:
                    Type atencionesModuleType = typeof(Gama.Atenciones.Wpf.AtencionesModule);
                    ModuleCatalog.AddModule(new ModuleInfo()
                    {
                        ModuleName = atencionesModuleType.Name,
                        ModuleType = atencionesModuleType.AssemblyQualifiedName,
                        InitializationMode = InitializationMode.WhenAvailable
                    });
                    break;
                default:
                    break;
            }
        }
    }
}
