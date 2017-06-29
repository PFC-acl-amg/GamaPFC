using Prism.Modularity;
using Prism.Unity;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Unity;
using Gama.Common.BaseClasses;
using System.IO;
using Gama.Common;
using Gama.Socios.Wpf.Services;
using System.Runtime.Serialization.Formatters.Binary;
using Core.DataAccess;
using Gama.Socios.DataAccess;
using NHibernate;
using Gama.Socios.Wpf.FakeServices;

namespace Gama.Socios.Wpf
{
    public class Bootstrapper : UnityBootstrapperBase
    {
        public Bootstrapper(string title = "GESTIÓN DE SOCIOS") : base(title)
        {
            _CLEAR_DATABASE = false;
            _SEED_DATABASE = false;
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/icono_modulo_cooperacion.png"));

            ((ShellViewModel)((FrameworkElement)Shell).DataContext).Title = "GESTIÓN DE SOCIOS";
            ((ShellViewModel)((FrameworkElement)Shell).DataContext).IconSource = icon;

            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow.Show();

            TerminarPreload();
        }

        protected override void InitializeDirectories()
        {
            if (!Directory.Exists(ResourceNames.IconsAndImagesFolder))
                Directory.CreateDirectory(ResourceNames.IconsAndImagesFolder);

            try
            {
                BitmapImage icon;
                BitmapEncoder encoder;

                // Default Search Icon
                if (!File.Exists(ResourceNames.DefaultSearchIconPath))
                {
                    icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/default_search_icon.png"));
                    encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(icon));

                    using (var fileStream =
                        new FileStream(ResourceNames.DefaultSearchIconPath, System.IO.FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }

                // Default User Icon
                if (!File.Exists(ResourceNames.DefaultUserIconPath))
                {
                    icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/default_user_icon.png"));
                    encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(icon));

                    using (var fileStream =
                        new FileStream(ResourceNames.DefaultUserIconPath, System.IO.FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }

                // Atención Icon
                if (!File.Exists(ResourceNames.AtencionIconPath))
                {
                    icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Atenciones.Wpf;component/Resources/Images/atencion_icon.png"));
                    encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(icon));

                    using (var fileStream =
                        new FileStream(ResourceNames.AtencionIconPath, System.IO.FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected override void ConfigurePreferences()
        {
            Preferencias preferencias;

            if (!Directory.Exists(Preferencias.PreferenciasPathFolder))
                Directory.CreateDirectory(Preferencias.PreferenciasPathFolder);

            if (File.Exists(Preferencias.PreferenciasPath))
            {
                var preferenciasFile = File.Open(Preferencias.PreferenciasPath, FileMode.Open);
                preferencias = (Preferencias)new BinaryFormatter().Deserialize(preferenciasFile);
                preferenciasFile.Close();
            }
            else
            {
                preferencias = new Preferencias();
                new BinaryFormatter().Serialize(File.Create(Preferencias.PreferenciasPath), preferencias);
            }

            Container.RegisterInstance(preferencias);
        }

        protected override void RegisterServices()
        {
            Container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactory());
            Container.RegisterType<ISession>(
                new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenSession()));
            Container.RegisterType<ISocioRepository, SocioRepository>();

            Container.RegisterInstance(new ExportService());
        }

        protected override void GenerateDatabaseConfiguration()
        {
            var sessionFactory = Container.Resolve<INHibernateSessionFactory>();
            var socioRepository = new SocioRepository();
            var session = sessionFactory.OpenSession();
            socioRepository.Session = session;

            try
            {
                if (_SEED_DATABASE)
                {

                    foreach (var socio in (new FakeSocioRepository().GetAll()))
                    {
                        socioRepository.Create(socio);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw ex;
            }

            // Recogemos todos los NIF para usarlos en validación
            // No lo hacemos en el wrapper directamente para eliminar el acomplamiento
            // del wrapper a los servicios. 
            SociosResources.TodosLosNif = socioRepository.GetNifs();
        }

        protected override void ConfigureModuleCatalog()
        {
            Type atencionesModuleType = typeof(SociosModule);
            ModuleCatalog.AddModule(new ModuleInfo()
            {
                ModuleName = atencionesModuleType.Name,
                ModuleType = atencionesModuleType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });
        }
    }
}
