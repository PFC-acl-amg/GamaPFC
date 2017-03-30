using Prism.Modularity;
using Prism.Unity;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Unity;


namespace Gama.Atenciones.Wpf
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

            title = "SERVICIO DE ATENCIONES";
            icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/icono_modulo_atenciones.png"));

            ((ShellViewModel)((FrameworkElement)Shell).DataContext).Title = title;
            ((ShellViewModel)((FrameworkElement)Shell).DataContext).IconSource = icon;

            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            Type atencionesModuleType = typeof(AtencionesModule);
            ModuleCatalog.AddModule(new ModuleInfo()
            {
                ModuleName = atencionesModuleType.Name,
                ModuleType = atencionesModuleType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });
        }
    }
}
