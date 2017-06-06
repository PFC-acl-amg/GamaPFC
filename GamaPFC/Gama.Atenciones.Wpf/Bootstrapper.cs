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
using System.Linq;
using Prism.Events;
using Core.Util;

namespace Gama.Atenciones.Wpf
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            Container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactory());
            Container.RegisterType<ISession>(
                new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenSession()));
            Container.RegisterType<IPersonaRepository, PersonaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICitaRepository, CitaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAtencionRepository, AtencionRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAsistenteRepository, AsistenteRepository>(new ContainerControlledLifetimeManager());

            var session = Container.Resolve<ISession>();
            var personaRepository = Container.Resolve<IPersonaRepository>();
            var asistenteRepository = Container.Resolve<IAsistenteRepository>();
            personaRepository.Session = session;
            asistenteRepository.Session = session;

            //AtencionesResources.TodosLosNif = personaRepository.GetNifs();
            AtencionesResources.TodosLosNifDeAsistentes = asistenteRepository.GetNifs();

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
