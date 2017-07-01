using Prism.Modularity;
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
using Gama.Common.BaseClasses;
using System.Linq;
using System.ComponentModel;

namespace Gama.Atenciones.Wpf
{
    public class Bootstrapper : UnityBootstrapperBase
    {
        public Bootstrapper(string title = "SERVICIO DE ATENCIONES") : base(title)
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

            ((ShellViewModel)((FrameworkElement)Shell).DataContext).Title = "SERVICIO DE ATENCIONES";
            ((ShellViewModel)((FrameworkElement)Shell).DataContext).IconSource =
                new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/icono_modulo_atenciones.png"));

            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow.ShowActivated = true;
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
            Container.RegisterType<IPersonaRepository, PersonaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICitaRepository, CitaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAtencionRepository, AtencionRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAsistenteRepository, AsistenteRepository>(new ContainerControlledLifetimeManager());
        }

        protected override void GenerateDatabaseConfiguration()
        {
            if (_CLEAR_DATABASE || _SEED_DATABASE)
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

            InitializeCollections();
        }

        private void InitializeCollections()
        {

            var session = Container.Resolve<ISession>();
            _personaRepository = Container.Resolve<IPersonaRepository>();
            _citaRepository = Container.Resolve<ICitaRepository>();
            _atencionRepository = Container.Resolve<IAtencionRepository>();
            _asistenteRepository = Container.Resolve<IAsistenteRepository>();

           _personaRepository.Session = session;
           _citaRepository.Session = session;
           _atencionRepository.Session = session;
           _asistenteRepository.Session = session;

            AtencionesResources.StartStopWatch();
            var personas = _personaRepository.Personas;
            AtencionesResources.StopStopWatch("Personas");
            AtencionesResources.StartStopWatch();
            _citaRepository.Citas = personas.SelectMany(p => p.Citas).ToList();
            AtencionesResources.StopStopWatch("Citas");
            AtencionesResources.StartStopWatch();
            _atencionRepository.Atenciones = _citaRepository.Citas.Select(c => c.Atencion).Where(a => a != null).ToList();
            AtencionesResources.StopStopWatch("Atenciones");

            //_BackgroundWorker = new BackgroundWorker();
            //_BackgroundWorker.DoWork += BackgroundWorker_DoWork;

            //_BackgroundWorker.RunWorkerAsync();
        }

        private BackgroundWorker _BackgroundWorker;
        private IAsistenteRepository _asistenteRepository;
        private IPersonaRepository _personaRepository;
        private ICitaRepository _citaRepository;
        private IAtencionRepository _atencionRepository;

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ONDo();
        }

        private void ONDo()
        {
            //AtencionesResources.StartStopWatch();
            //var asistentes = asistenteRepository.Asistentes;
            //AtencionesResources.StopStopWatch("Asistentes");
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
