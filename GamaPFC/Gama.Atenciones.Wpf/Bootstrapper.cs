using Prism.Modularity;
using Prism.Unity;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Unity;
using Core.DataAccess;
using NHibernate;
using Gama.Atenciones.Wpf.Services;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Gama.Atenciones.DataAccess;
using Gama.Atenciones.Wpf.ViewModels;
using Gama.Atenciones.Wpf.Views;

namespace Gama.Atenciones.Wpf
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            Container.RegisterType<object, AsistentesContentView>("AsistentesContentView");
            Container.RegisterType<object, AsistenteView>("AsistenteView");
            Container.RegisterType<object, CitasContentView>("CitasContentView");
            Container.RegisterType<object, DashboardView>("DashboardView");
            Container.RegisterType<object, EditarAtencionesView>("EditarAtencionesView");
            Container.RegisterType<object, EditarCitasView>("EditarCitasView");
            Container.RegisterType<object, EditarPersonaView>("EditarPersonaView");
            Container.RegisterType<object, GraficasContentView>("GraficasContentView");
            Container.RegisterType<object, ListadoDePersonasView>("ListadoDePersonasView");
            Container.RegisterType<object, PanelSwitcherView>("PanelSwitcherView");
            Container.RegisterType<object, PersonasContentView>("PersonasContentView");
            Container.RegisterType<object, StatusBarView>("StatusBarView");
            Container.RegisterType<object, ToolbarView>("ToolbarView");
            Container.RegisterType<object, RightCommandsView>("RightCommandsView");
            Container.RegisterType<object, PreferenciasView>("PreferenciasView");
            Container.RegisterType<AsistentesContentViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<AsistenteViewModel>();
            Container.RegisterType<CitasContentViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<DashboardViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<EditarAtencionesViewModel>();
            Container.RegisterType<EditarCitasViewModel>();
            Container.RegisterType<EditarPersonaViewModel>();
            Container.RegisterType<GraficasContentViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ListadoDePersonasViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<PanelSwitcherViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<PersonasContentViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<PreferenciasViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<RightCommandsViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<SearchBoxViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<StatusBarViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ToolbarViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactory());
            Container.RegisterType<ISession>(
                new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenSession()));
            Container.RegisterType<IPersonaRepository, PersonaRepository>();
            Container.RegisterType<ICitaRepository, CitaRepository>();
            Container.RegisterType<IAtencionRepository, AtencionRepository>();
            Container.RegisterType<IAsistenteRepository, AsistenteRepository>();

            PreferenciasDeAtenciones preferencias;
            string preferenciasPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + @"\preferencias_de_atenciones.cfg";

            if (File.Exists(preferenciasPath))
            {
                var preferenciasFile = File.Open(preferenciasPath, FileMode.Open);
                preferencias = (PreferenciasDeAtenciones)new BinaryFormatter().Deserialize(preferenciasFile);
                preferenciasFile.Close();
            }
            else
            {
                preferencias = new PreferenciasDeAtenciones();
                new BinaryFormatter().Serialize(File.Create(preferenciasPath), preferencias);
            }

            Container.RegisterInstance<PreferenciasDeAtenciones>(preferencias);
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
