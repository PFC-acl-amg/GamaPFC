﻿using Prism.Modularity;
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
using System.Threading;
using Gama.Atenciones.Wpf.Views;
using System.Security.Permissions;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Gama.Common.Eventos;
using Gama.Common.Views;

namespace Gama.Atenciones.Wpf
{
    public class Bootstrapper : UnityBootstrapper
    {
        private bool _CLEAR_DATABASE = false;
        private bool _SEED_DATABASE = false;
        private Thread _PreloadThread;
        public static PreloaderView _PreloaderView;
        private ObservableCollection<string> _NotifyCollection;

        protected override DependencyObject CreateShell()
        {
            _NotifyCollection = new ObservableCollection<string>();

            _PreloadThread = new Thread(_PreLoad);
            _PreloadThread.SetApartmentState(ApartmentState.STA);
            _PreloadThread.Start();

            Thread.Sleep(200);
            lock (_PreloaderView)
            {
                _NotifyCollection.Add("x"); InicializarDirectorios();
                _NotifyCollection.Add("x"); ConfigurarPreferencias();
                _NotifyCollection.Add("x"); RegisterServices(); 
                _NotifyCollection.Add("x"); ConfigureDatabase(); 
                _NotifyCollection.Add("x");
            }

            var session = Container.Resolve<ISession>();
            var personaRepository = Container.Resolve<IPersonaRepository>();
            var asistenteRepository = Container.Resolve<IAsistenteRepository>();
            personaRepository.Session = session;
            asistenteRepository.Session = session;
            return Container.Resolve<Shell>();
        }

        private void _PreLoad()
        {
             _PreloaderView = new PreloaderView(_NotifyCollection);
            _PreloaderView.Titulo = "SERVICIO DE ATENCIONES";
            _PreloaderView.ShowDialog();
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        private void _KillTheThread()
        {
            _NotifyCollection.Add("<END>");
            //_PreloaderView.Close();
            _PreloadThread.Abort();
            _PreloadThread = null;
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

            //_PreloaderView.Close();
            //_PreloaderView = null;
            //lock(_PreloaderView)

            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow.ShowActivated = true;
            Application.Current.MainWindow.Show();

            _KillTheThread();

            //if (AtencionesResources.ClientService.IsConnected())
            //    Container.Resolve<EventAggregator>().GetEvent<LaConexionConElServidorHaCambiadoEvent>().Publish(MensajeDeConexion.Conectado);
            //else
            //    Container.Resolve<EventAggregator>().GetEvent<LaConexionConElServidorHaCambiadoEvent>().Publish(MensajeDeConexion.NoConectado);
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
