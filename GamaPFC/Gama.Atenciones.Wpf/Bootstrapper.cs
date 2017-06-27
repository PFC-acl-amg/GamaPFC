using Prism.Modularity;
using Prism.Unity;
using Prism.Events;
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
using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Business;
using Gama.Common;

namespace Gama.Atenciones.Wpf
{
    public class Bootstrapper : UnityBootstrapper
    {
        private bool _CLEAR_DATABASE = false;
        private bool _SEED_DATABASE = false;

        protected override DependencyObject CreateShell()
        {
            InicializarDirectorios();
            ConfigurarPreferencias();
            ConectarConServidor();
            RegisterServices();
            ConfigureDatabase();

            var session = Container.Resolve<ISession>();
            var personaRepository = Container.Resolve<IPersonaRepository>();
            var asistenteRepository = Container.Resolve<IAsistenteRepository>();
            personaRepository.Session = session;
            asistenteRepository.Session = session;
            return Container.Resolve<Shell>();
        }

        private void InicializarDirectorios()
        {
            if (!Directory.Exists(ResourceNames.IconsAndImagesFolder))
            {
                Directory.CreateDirectory(ResourceNames.IconsAndImagesFolder);
            }

            try
            {
                BitmapImage icon;
                BitmapEncoder encoder;

                //
                // Default Search Icon
                //
                if (!File.Exists(ResourceNames.DefaultSearchIconPath))
                {
                    icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/default_search_icon.png"));
                    encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(icon));

                    using (var fileStream =
                        new System.IO.FileStream(ResourceNames.DefaultSearchIconPath, System.IO.FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }

                //
                // Default User Icon
                //
                if (!File.Exists(ResourceNames.DefaultUserIconPath))
                {
                    icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/default_user_icon.png"));
                    encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(icon));

                    using (var fileStream =
                        new System.IO.FileStream(ResourceNames.DefaultUserIconPath, System.IO.FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }

                //
                // Atención Icon
                //
                if (!File.Exists(ResourceNames.AtencionIconPath))
                {
                    icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Atenciones.Wpf;component/Resources/Images/atencion_icon.png"));
                    encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(icon));

                    using (var fileStream =
                        new System.IO.FileStream(ResourceNames.AtencionIconPath, System.IO.FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ConfigurarPreferencias()
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

        private void ConectarConServidor()
        {
            AtencionesResources.ClientService = new ClientService(Container.Resolve<EventAggregator>());
        }

        private void RegisterServices()
        {
            Container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactory());
            Container.RegisterType<ISession>(
                new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenSession()));
            Container.RegisterType<IPersonaRepository, PersonaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICitaRepository, CitaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAtencionRepository, AtencionRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAsistenteRepository, AsistenteRepository>(new ContainerControlledLifetimeManager());
        }

        private void ConfigureDatabase()
        {
            var sessionFactory = Container.Resolve<INHibernateSessionFactory>();

            // NOTA: No utilizamos los servicios directamente porque añaden código que afecta al resto de la aplicación
            //, a través del EventAggregator por ejemplo. Sólo requerimos la funcionalidad de base de datos.
            var personaRepository = new NHibernateOneSessionRepository<Persona, int>();// Container.Resolve<IPersonaRepository>();
            var citaRepository = new NHibernateOneSessionRepository<Cita, int>();// Container.Resolve<ICitaRepository>();
            var asistenteRepository = new NHibernateOneSessionRepository<Asistente, int>();// Container.Resolve<IAsistenteRepository>();
            var atencionRepository = new NHibernateOneSessionRepository<Atencion, int>();// Container.Resolve<IAtencionRepository>();
            var derivacionRepository = new NHibernateOneSessionRepository<Derivacion, int>();

            var session = sessionFactory.OpenSession();

            personaRepository.Session = session;
            citaRepository.Session = session;
            asistenteRepository.Session = session;
            atencionRepository.Session = session;
            derivacionRepository.Session = session;

            /// INICIALIZACIÓN 
            /// En este caso sí necesitamos crear el servicio tal cual porque accedemos a una función espsecífica
            var asisRep = Container.Resolve<IAsistenteRepository>();
            asisRep.Session = session;
            AtencionesResources.TodosLosNifDeAsistentes = asisRep.GetNifs();

            if (_CLEAR_DATABASE)
            {
                citaRepository.DeleteAll();
                asistenteRepository.DeleteAll();
                personaRepository.DeleteAll();
                atencionRepository.DeleteAll();
                derivacionRepository.DeleteAll();
            }

            try
            {
                if (_SEED_DATABASE)
                {
                    var personas = new FakePersonaRepository().GetAll(); //personaRepository.GetAll();
                    var citas = new FakeCitaRepository().GetAll();
                    var atenciones = new FakeAtencionRepository().GetAll();
                    var asistentes = new FakeAsistenteRepository().Asistentes;

                    //personas.ForEach(p => p.Id = 0);
                    //citas.ForEach(c => c.Id = 0);
                    //atenciones.ForEach(a => a.Id = 0);

                    var random = new Random();

                    foreach (var asistente in asistentes)
                        asistenteRepository.Create(asistente);
                
                    foreach (var persona in personas)
                        personaRepository.Create(persona);

                    foreach (var cita in citas)
                    {
                        var persona = personas[random.Next(0, personas.Count - 1)];
                        persona.AddCita(cita);
                        cita.Asistente = asistentes[random.Next(0, asistentes.Count - 1)];
                        citaRepository.Create(cita);
                    }

                    int i = 0;
                    foreach (var atencion in atenciones)
                    {
                        atencion.Cita = citas[i++];
                        atencion.Derivacion = FakeDerivacionRepository.Next(atencion);

                        atencionRepository.Create(atencion);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
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
