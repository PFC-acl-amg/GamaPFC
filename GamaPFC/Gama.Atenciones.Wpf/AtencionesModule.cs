using Core;
using Core.DataAccess;
using Gama.Atenciones.Business;
using Gama.Atenciones.DataAccess;
using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.ViewModels;
using Gama.Atenciones.Wpf.Views;
using Gama.Common;
using Microsoft.Practices.Unity;
using NHibernate;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Gama.Atenciones.Wpf
{
    public static class AtencionesResources
    {
        public static List<string> TodosLosNifDeAsistentes { get; set; }

        public static void AddNifAAsistente(string nif)
        {
            if (!TodosLosNifDeAsistentes.Contains(nif))
            {
                TodosLosNifDeAsistentes.Add(nif);
            }
        }

        public static List<Persona> Personas { get; set; }
        public static ClientService ClientService { get; set; }
        public static string ClientId { get; set; }
    }

    public class AtencionesModule : ModuleBase
    {
       // private ClientService _ClientService;

        public bool ClearDatabase { get; private set; }

        public AtencionesModule(IUnityContainer container, IRegionManager regionManager)
           : base(container, regionManager)
        {
            this.Entorno = Entorno.Desarrollo;
            this.ClearDatabase = false;
            this.SeedDatabase = false;
        }

        public override void Initialize()
        {
            RegisterServices();

            var sessionFactory = Container.Resolve<INHibernateSessionFactory>();
            var personaRepository = Container.Resolve <IPersonaRepository>();
            var citaRepository = Container.Resolve<ICitaRepository>();
            var asistenteRepository = Container.Resolve<IAsistenteRepository>();
            var session = sessionFactory.OpenSession();
            personaRepository.Session = session;
            citaRepository.Session = session;
            asistenteRepository.Session = session;

            var personaRepository2 = new NHibernateOneSessionRepository<Persona, int>()
            {
                Session = session
            };

            if (ClearDatabase)
            {
                citaRepository.DeleteAll();
                asistenteRepository.DeleteAll();
                personaRepository.DeleteAll();
            }

            #region Database Seeding
            try
            {
                if (SeedDatabase)
                {

                    var personas = new FakePersonaRepository().GetAll(); //personaRepository.GetAll();
                    var citas = new FakeCitaRepository().GetAll();
                    var atenciones = new FakeAtencionRepository().GetAll();

                    personas.ForEach(p => p.Id = 0);
                    //citas.ForEach(c => c.Id = 0);
                    //atenciones.ForEach(a => a.Id = 0);

                    var random = new Random();
                    var opciones = new bool[] { true, false, true, false, true, true, false, true, false };

                    for (int i = 0; i < personas.Count; i++)
                    {
                        var persona = personas[i];
                        //var cita = citas[i];
                        //var atencion = atenciones[i];
                        //var derivacion = new Derivacion
                        //{
                        //    Id = 0,
                        //    Atencion = atencion,
                        //    EsDeFormacion = opciones[random.Next(0, 8)],
                        //    EsDeFormacion_Realizada = opciones[random.Next(0, 8)],
                        //    EsDeOrientacionLaboral = opciones[random.Next(0, 8)],
                        //    EsDeOrientacionLaboral_Realizada = opciones[random.Next(0, 8)],
                        //    EsExterna = opciones[random.Next(0, 8)],
                        //    EsExterna_Realizada = opciones[random.Next(0, 8)],
                        //    EsJuridica = opciones[random.Next(0, 8)],
                        //    EsJuridica_Realizada = opciones[random.Next(0, 8)],
                        //    EsPsicologica = opciones[random.Next(0, 8)],
                        //    EsPsicologica_Realizada = opciones[random.Next(0, 8)],
                        //    EsSocial = opciones[random.Next(0, 8)],
                        //    EsSocial_Realizada = opciones[random.Next(0, 8)],
                        //    Externa = "Externa",
                        //    Externa_Realizada = "Externa realizada",
                        //    Tipo = "",
                        //};

                        //atencion.Derivacion = derivacion;

                        //cita.SetAtencion(atencion);
                        //persona.AddCita(citas[i]);

                        personaRepository2.Create(persona);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }
            #endregion

            // Preparamos la estructura de carpeta para la primera vez
            InicializarDirectorios();

            RegisterViews();
            RegisterViewModels();
            InitializeNavigation();

            ConectarConServidor();
            //AtencionesResources.ClientService.EnviarMensaje("jijiji");
        }

        private void ConectarConServidor()
        {
            AtencionesResources.ClientService = new ClientService(Container.Resolve<EventAggregator>());
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

        private void RegisterViews()
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
        }

        private void RegisterViewModels()
        {
            Container.RegisterType<AsistentesContentViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<AsistenteViewModel>();
            Container.RegisterType<CitasContentViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<DashboardViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<EditarAtencionesViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<EditarCitasViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<EditarPersonaViewModel>();
            Container.RegisterType<GraficasContentViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ListadoDePersonasViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<PanelSwitcherViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<PersonasContentViewModel>(new ContainerControlledLifetimeManager());
            //Container.RegisterType<PreferenciasViewModel>();
            Container.RegisterType<RightCommandsViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<SearchBoxViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<StatusBarViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ToolbarViewModel>(new ContainerControlledLifetimeManager());
        }

        private void RegisterServices()
        {
        }

        private void InitializeNavigation()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.RightCommandsRegion, typeof(RightCommandsView));
        }
    }
}
