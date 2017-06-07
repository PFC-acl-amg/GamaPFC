using Prism.Modularity;
using Prism.Unity;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Unity;


namespace Gama.Cooperacion.Wpf
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            string title = "";
            BitmapImage icon = new BitmapImage();

            title = "COOPERACIÓN";
            icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/icono_modulo_cooperacion.png"));

            ((ShellViewModel)((FrameworkElement)Shell).DataContext).Title = title;
            ((ShellViewModel)((FrameworkElement)Shell).DataContext).IconSource = icon;

            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow.Show();
            ColeccionEstadosActividades.EstadosActividades = new System.Collections.Generic.Dictionary<string, int>();
            ColeccionEstadosActividades.EstadosActividades.Add("Comenzado", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("NoComenzado", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("FueraPlazo", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("ProximasFinalizaciones", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("Finalizado", 0);

        }

        protected override void ConfigureModuleCatalog()
        {
            Type atencionesModuleType = typeof(CooperacionModule);
            ModuleCatalog.AddModule(new ModuleInfo()
            {
                ModuleName = atencionesModuleType.Name,
                ModuleType = atencionesModuleType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });
        }
    }
}
