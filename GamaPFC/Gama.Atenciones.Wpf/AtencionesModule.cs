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
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Gama.Atenciones.Wpf
{
    public static class AtencionesResources
    {
        public static List<string> TodosLosNif { get; set; }

        public static void AddNif(string nif)
        {
            if (!TodosLosNif.Contains(nif))
            {
                TodosLosNif.Add(nif);
            }
        }
    }

    public class AtencionesModule : ModuleBase
    {
        public AtencionesModule(IUnityContainer container, IRegionManager regionManager)
           : base(container, regionManager)
        {
            this.Entorno = Entorno.Desarrollo;
            this.UseFaker = false;
        }

        public override void Initialize()
        {
            RegisterServices();
            RegisterViews();
            RegisterViewModels();

            var sessionFactory = Container.Resolve<INHibernateSessionFactory>();
            var personaRepository = new PersonaRepository();
            var session = sessionFactory.OpenSession();
            personaRepository.Session = session;

            #region Database Seeding
            try
            {
                if (UseFaker)
                {

                    var personas = new FakePersonaRepository().GetAll(); //personaRepository.GetAll();
                    var citas = new FakeCitaRepository().GetAll();
                    var atenciones = new FakeAtencionRepository().GetAll();

                    personas.ForEach(p => p.Id = 0);
                    citas.ForEach(c => c.Id = 0);
                    atenciones.ForEach(a => a.Id = 0);

                    var random = new Random();
                    var opciones = new bool[] { true, false, true, false, true, true, false, true, false };

                    for (int i = 0; i < personas.Count; i++)
                    {
                        var persona = personas[i];
                        var cita = citas[i];
                        var atencion = atenciones[i];
                        var derivacion = new Derivacion
                        {
                            Id = 0,
                            Atencion = atencion,
                            EsDeFormacion = opciones[random.Next(0, 8)],
                            EsDeFormacion_Realizada = opciones[random.Next(0, 8)],
                            EsDeOrientacionLaboral = opciones[random.Next(0, 8)],
                            EsDeOrientacionLaboral_Realizada = opciones[random.Next(0, 8)],
                            EsExterna = opciones[random.Next(0, 8)],
                            EsExterna_Realizada = opciones[random.Next(0, 8)],
                            EsJuridica = opciones[random.Next(0, 8)],
                            EsJuridica_Realizada = opciones[random.Next(0, 8)],
                            EsPsicologica = opciones[random.Next(0, 8)],
                            EsPsicologica_Realizada = opciones[random.Next(0, 8)],
                            EsSocial = opciones[random.Next(0, 8)],
                            EsSocial_Realizada = opciones[random.Next(0, 8)],
                            Externa = "Externa",
                            Externa_Realizada = "Externa realizada",
                            Tipo = "",
                        };

                        atencion.Derivacion = derivacion;

                        cita.SetAtencion(atencion);
                        persona.AddCita(citas[i]);

                        personaRepository.Create(persona);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw ex;
            }
            #endregion

            // Recogemos todos los NIF para usarlos en validación
            // No lo hacemos en el wrapper directamente para eliminar el acomplamiento
            // del wrapper a los servicios. 
            AtencionesResources.TodosLosNif = personaRepository.GetNifs();

            // Preparamos la estructura de carpeta para la primera vez
            InicializarDirectorios();

            InitializeNavigation();
        }

        private void InicializarDirectorios()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string iconsAndImagesPath = path + @"IconsAndImages\";

            if (!Directory.Exists(iconsAndImagesPath))
            {
                Directory.CreateDirectory(iconsAndImagesPath);

                var icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/default_search_icon.png"));
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(icon));

                using (var fileStream = new System.IO.FileStream(iconsAndImagesPath + "default_search_icon.png", System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/default_user_icon.png"));
                encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(icon));

                using (var fileStream = new System.IO.FileStream(iconsAndImagesPath + "default_user_icon.png", System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
        }

        private void RegisterViews()
        {
            Container.RegisterType<object, DashboardView>("DashboardView");

            Container.RegisterType<object, EditarAtencionesView>("EditarAtencionesView");
            Container.RegisterType<object, EditarCitasView>("EditarCitasView");
            Container.RegisterType<object, EditarPersonaView>("EditarPersonaView");

            Container.RegisterType<object, GraficasView>("GraficasView");

            Container.RegisterType<object, ListadoDePersonasView>("ListadoDePersonasView");
            Container.RegisterType<object, PanelSwitcherView>("PanelSwitcherView");
            Container.RegisterType<object, PersonasContentView>("PersonasContentView");
            Container.RegisterType<object, StatusBarView>("StatusBarView");
            Container.RegisterType<object, ToolbarView>("ToolbarView");
        }

        private void RegisterViewModels()
        {
            Container.RegisterType<DashboardViewModel>();

            Container.RegisterType<EditarAtencionesViewModel>();
            Container.RegisterType<EditarCitasViewModel>();
            Container.RegisterType<EditarPersonaViewModel>();

            Container.RegisterInstance(new GraficasViewModel(
                Container.Resolve<IPersonaRepository>(),
                Container.Resolve<ISession>()));

            Container.RegisterType<ListadoDePersonasViewModel>();
            Container.RegisterType<PanelSwitcherViewModel>();
            Container.RegisterType<PersonasContentViewModel>();
            Container.RegisterType<SearchBoxViewModel>();
            Container.RegisterType<StatusBarViewModel>();
            Container.RegisterType<ToolbarViewModel>();
        }

        private void RegisterServices()
        {
            Container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactory());
            Container.RegisterType<ISession>(
                new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenSession()));
            Container.RegisterType<IPersonaRepository, PersonaRepository>();
            Container.RegisterType<ICitaRepository, CitaRepository>();
            Container.RegisterType<IAtencionRepository, AtencionRepository>();
            Container.RegisterType<IAtencionesSettings, AtencionesSettings>();
        }

        private void InitializeNavigation()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.PanelSwitcherRegion, typeof(PanelSwitcherView));
            RegionManager.RegisterViewWithRegion(RegionNames.ToolbarRegion, typeof(ToolbarView));
            RegionManager.RegisterViewWithRegion(RegionNames.SearchBoxRegion, typeof(SearchBoxView));
            RegionManager.RegisterViewWithRegion(RegionNames.StatusBarRegion, typeof(StatusBarView));
            RegionManager.RequestNavigate(RegionNames.ContentRegion, "DashboardView");

            RegionManager.AddToRegion(RegionNames.ContentRegion, Container.Resolve<PersonasContentView>());
            RegionManager.AddToRegion(RegionNames.PersonasTabContentRegion, Container.Resolve<ListadoDePersonasView>());
        }
    }
}
