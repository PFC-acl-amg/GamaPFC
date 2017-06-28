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
            Container.RegisterType<IActividadRepository, ActividadRepository>();
            Container.RegisterType<ICooperanteRepository, CooperanteRepository>();
            Container.RegisterType<IForoRepository, ForoRepository>();
            Container.RegisterType<ITareaRepository, TareaRepository>();
            Container.RegisterInstance<ICooperacionSettings>(new CooperacionSettings());
        }

        protected override void GenerateDatabaseConfiguration()
        {
            if (_SEED_DATABASE)
            {
                var session = Container.Resolve<ISession>();
                var cooperanteRepository = Container.Resolve<ICooperanteRepository>();
                cooperanteRepository.Session = session;
                var cooperantesDummy = new FakeCooperanteRepository().GetAll().Take(2);

                foreach (var cooperante in cooperantesDummy) // Crea tambien mas cooperantes de forma automatica
                {
                    cooperanteRepository.Create(cooperante);
                }

                var actividadRepository = Container.Resolve<IActividadRepository>();
                //var eventoRepository = Container.Resolve<IEventoRepository>();

                actividadRepository.Session = session;
                cooperanteRepository.Session = session;
                //eventoRepository.Session = session;

                foreach (var cooperante in cooperantesDummy)
                {
                    //cooperanteRepository.Create(cooperante); // para crear cooperantes nuevos forma automatica
                }

                //var cooperanteRepository = Container.Resolve<ICooperanteRepository>();
                //var actividadRepository = Container.Resolve<IActividadRepository>();
                //var session = Container.Resolve<ISession>();
                //actividadRepository.Session = session;
                //cooperanteRepository.Session = session;

                var coordinador = cooperanteRepository.GetAll().First();
                var actividadesFake = new FakeActividadRepository().GetAll();

                foreach (var actividad in actividadesFake.Take(1))
                {
                    var eventosFake = new FakeEventoRepository().GetAll();
                    var foroFake = new FakeForoRepository().GetAll();
                    var mensajeForoFake = new FakeMensajeRepository().GetAll();
                    var tareaFake = new FakeTareaRepository().GetAll();
                    var seguimientoFake = new FakeSeguimientoRepository().GetAll();
                    var incidenciaFake = new FakeIncidenciaRepository().GetAll();
                    //foreach (var tarea in tareaFake)
                    //{
                    //    var seguimientoFake = new FakeSeguimientoRepository().GetAll();
                    //    var incidenciaFake = new FakeIncidenciaRepository().GetAll();
                    //    int j = 0;
                    //    int k = 0;
                    //    int l = 0;
                    //    foreach (var seguimiento in seguimientoFake)
                    //    {
                    //        tarea.Seguimiento.Insert(j, seguimiento);
                    //        j++;
                    //    }
                    //    foreach (var incidencia in incidenciaFake)
                    //    {
                    //        tarea.Incidencias.Insert(l, incidencia);
                    //        l++;
                    //    }
                    //    actividad.Tareas.Insert(k, tarea);
                    //    k++;
                    //}
                    actividad.Coordinador = coordinador;
                    //foreach (var InsertandoTareas in tareaFake)
                    //{
                    //    foreach (var InsertandoSeguimientos in seguimientoFake)
                    //    {
                    //        InsertandoTareas.AddSeguimiento(InsertandoSeguimientos);
                    //    }
                    //    foreach(var InsertandoIncidencias in incidenciaFake)
                    //    {
                    //        InsertandoTareas.AddIncidencia(InsertandoIncidencias);
                    //    }
                    //    InsertandoTareas.Responsable = coordinador;
                    //    actividad.AddTarea(InsertandoTareas);
                    //}
                    //foreach (var InsertandoEvento in eventosFake)
                    //{
                    //    actividad.AddEvento(InsertandoEvento);
                    //}
                    //foreach (var InsertandoForos in foroFake)
                    //{
                    //    foreach (var InsertandoMensajesForos in mensajeForoFake)
                    //    {
                    //        InsertandoForos.AddMensaje(InsertandoMensajesForos);
                    //    }
                    //    actividad.AddForo(InsertandoForos);
                    //}
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
