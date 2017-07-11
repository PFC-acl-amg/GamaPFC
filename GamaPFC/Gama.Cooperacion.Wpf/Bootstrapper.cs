using Prism.Modularity;
using Prism.Unity;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Unity;
using Gama.Common.BaseClasses;
using System.IO;
using Gama.Common;
using Gama.Cooperacion.Wpf.Services;
using System.Runtime.Serialization.Formatters.Binary;
using Core.DataAccess;
using Gama.Cooperacion.DataAccess;
using NHibernate;
using System.Linq;
using Gama.Cooperacion.Business;

namespace Gama.Cooperacion.Wpf
{
    public class Bootstrapper : UnityBootstrapperBase
    {
        public Bootstrapper(string title = "COOPERACIÓN") : base(title)
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

            ((ShellViewModel)((FrameworkElement)Shell).DataContext).Title = "COOPERACIÓN";
            ((ShellViewModel)((FrameworkElement)Shell).DataContext).IconSource = icon;

            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow.Show();
            ColeccionEstadosActividades.EstadosActividades = new System.Collections.Generic.Dictionary<string, int>();
            ColeccionEstadosActividades.EstadosActividades.Add("Comenzado", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("NoComenzado", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("FueraPlazo", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("ProximasFinalizaciones", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("Finalizado", 0);

            TerminarPreload();
        }

        protected override void InitializeDirectories()
        {
            if (!Directory.Exists(ResourceNames.AppDataFolder))
                Directory.CreateDirectory(ResourceNames.AppDataFolder);

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
            Container.RegisterType<IActividadRepository, ActividadRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICooperanteRepository, CooperanteRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IForoRepository, ForoRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ITareaRepository, TareaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IEventoRepository, EventoRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IIncidenciaRepository, IncidenciaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ITareaRepository, TareaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISeguimientoRepository, SeguimientoRepository>(new ContainerControlledLifetimeManager());

            Container.RegisterInstance<Preferencias>(new Preferencias());
        }

        protected override void GenerateDatabaseConfiguration()
        {
            if (_CLEAR_DATABASE)
            {
                var session = Container.Resolve<ISession>();
                var cooperanteRepository = new NHibernateOneSessionRepository<Cooperante, int>();
                var actividadRepository = new NHibernateOneSessionRepository<Actividad, int>();
                cooperanteRepository.Session = session;
                actividadRepository.Session = session;

                actividadRepository.DeleteAll();
                cooperanteRepository.DeleteAll();
            }

            if (_SEED_DATABASE)
            {
                var session = Container.Resolve<ISession>();
                var cooperanteRepository = new NHibernateOneSessionRepository<Cooperante, int>();
                var actividadRepository = new NHibernateOneSessionRepository<Actividad, int>();
                cooperanteRepository.Session = session;
                actividadRepository.Session = session;

                var cooperantesFake = new FakeCooperanteRepository().GetAll().Take(10).ToList();
                var actividadesFake = new FakeActividadRepository().GetAll();

                foreach (var cooperante in cooperantesFake) 
                    cooperanteRepository.Create(cooperante);

                var coordinador = cooperanteRepository.GetAll().First();

                for (int i = 0; i < 3; i++)
                {
                    var actividad = actividadesFake[i];
                    var eventosFake = new FakeEventoRepository().GetAll();
                    var foroFake = new FakeForoRepository().GetAll();
                    var mensajeForoFake = new FakeMensajeRepository().GetAll();
                    var tareaFake = new FakeTareaRepository().GetAll();
                    var seguimientoFake = new FakeSeguimientoRepository().GetAll();
                    var incidenciaFake = new FakeIncidenciaRepository().GetAll();
                    actividad.Coordinador = cooperantesFake[i];
                    actividad.AddCooperantes(cooperantesFake.Where(x => x.Id != actividad.Coordinador.Id));
                    actividadRepository.Create(actividad);
                }
            }

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
