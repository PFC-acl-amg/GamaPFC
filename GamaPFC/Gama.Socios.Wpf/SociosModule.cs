using Core;
using Core.DataAccess;
using Gama.Common;
using Gama.Socios.DataAccess;
using Gama.Socios.Wpf.FakeServices;
using Gama.Socios.Wpf.Services;
using Gama.Socios.Wpf.ViewModels;
using Gama.Socios.Wpf.Views;
using Microsoft.Practices.Unity;
using NHibernate;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf
{
    public static class SociosResources
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

    public class SociosModule : ModuleBase
    {
        public SociosModule(IUnityContainer container, IRegionManager regionManager)
           : base(container, regionManager)
        {
            this.Entorno = Entorno.Desarrollo;
            this.UseFaker = false;
        }

        public override void Initialize()
        {
            RegisterViews();
            RegisterViewModels();
            RegisterServices();

            var sessionFactory = Container.Resolve<INHibernateSessionFactory>();
            var socioRepository = new SocioRepository();
            var session = sessionFactory.OpenSession();
            socioRepository.Session = session;

            #region Database Seeding
            try
            {
                if (UseFaker)
                {

                    foreach(var socio in (new FakeSocioRepository().GetAll()))
                    {
                        socio.AddPeriodoDeAlta(new Business.PeriodoDeAlta
                        {
                            FechaDeAlta = DateTime.Now.AddYears(-3).AddMonths(-4),
                        });

                        socioRepository.Create(socio);
                    }

                    //var personas = new FakePersonaRepository().GetAll(); //personaRepository.GetAll();
                    //var citas = new FakeCitaRepository().GetAll();
                    //var atenciones = new FakeAtencionRepository().GetAll();

                    //personas.ForEach(p => p.Id = 0);
                    //citas.ForEach(c => c.Id = 0);
                    //atenciones.ForEach(a => a.Id = 0);

                    //var random = new Random();
                    //var opciones = new bool[] { true, false, true, false, true, true, false, true, false };

                    //for (int i = 0; i < personas.Count; i++)
                    //{
                    //    var persona = personas[i];
                    //    var cita = citas[i];
                    //    var atencion = atenciones[i];
                    //    var derivacion = new Derivacion
                    //    {
                    //        Id = 0,
                    //        Atencion = atencion,
                    //        EsDeFormacion = opciones[random.Next(0, 8)],
                    //        EsDeFormacion_Realizada = opciones[random.Next(0, 8)],
                    //        EsDeOrientacionLaboral = opciones[random.Next(0, 8)],
                    //        EsDeOrientacionLaboral_Realizada = opciones[random.Next(0, 8)],
                    //        EsExterna = opciones[random.Next(0, 8)],
                    //        EsExterna_Realizada = opciones[random.Next(0, 8)],
                    //        EsJuridica = opciones[random.Next(0, 8)],
                    //        EsJuridica_Realizada = opciones[random.Next(0, 8)],
                    //        EsPsicologica = opciones[random.Next(0, 8)],
                    //        EsPsicologica_Realizada = opciones[random.Next(0, 8)],
                    //        EsSocial = opciones[random.Next(0, 8)],
                    //        EsSocial_Realizada = opciones[random.Next(0, 8)],
                    //        Externa = "Externa",
                    //        Externa_Realizada = "Externa realizada",
                    //        Tipo = "",
                    //    };

                    //    atencion.Derivacion = derivacion;

                    //    cita.SetAtencion(atencion);
                    //    persona.AddCita(citas[i]);

                    //    personaRepository.Create(persona);
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
            SociosResources.TodosLosNif = socioRepository.GetNifs();

            InitializeNavigation();
        }

        private void RegisterViews()
        {
            Container.RegisterType<object, DashboardView>("DashboardView");
            Container.RegisterType<object, EditarCuotasView>("EditarCuotasView");
            Container.RegisterType<object, EditarPeriodosDeAltaView>("EditarPeriodosDeAltaView");
            Container.RegisterType<object, EditarSocioView>("EditarSocioView");
            Container.RegisterType<object, ListadoDeSociosView>("ListadoDeSociosView");
            Container.RegisterType<object, PanelSwitcherView>("PanelSwitcherView");
            Container.RegisterType<object, SociosContentView>("SociosContentView");
            Container.RegisterType<object, SocioView>("SocioView");
            Container.RegisterType<object, ToolbarView>("ToolbarView");
        }

        private void RegisterViewModels()
        {
            Container.RegisterType<DashboardViewModel>();
            Container.RegisterType<EditarCuotasViewModel>();
            Container.RegisterType<EditarPeriodosDeAltaViewModel>();
            Container.RegisterType<EditarSocioViewModel>();
            Container.RegisterType<ListadoDeSociosViewModel>();
            Container.RegisterType<NuevoSocioViewModel>();
            Container.RegisterType<PanelSwitcherViewModel>();
            Container.RegisterType<SociosContentViewModel>();
            Container.RegisterType<SocioViewModel>();
            Container.RegisterType<ToolbarViewModel>();
        }

        private void RegisterServices()
        {
            Container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactory());
            Container.RegisterType<ISession>(
                new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenSession()));

            Container.RegisterType<ISocioRepository, SocioRepository>();
            Container.RegisterInstance<ISociosSettings>(new SociosSettings());
            Container.RegisterInstance<ExportService>(new ExportService());
        }

        private void InitializeNavigation()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.PanelSwitcherRegion, typeof(PanelSwitcherView));
            RegionManager.RegisterViewWithRegion(RegionNames.ToolbarRegion, typeof(ToolbarView));
            RegionManager.RegisterViewWithRegion(RegionNames.SearchBoxRegion, typeof(SearchBoxView));
            RegionManager.RegisterViewWithRegion(RegionNames.StatusBarRegion, typeof(StatusBarView));
            RegionManager.RequestNavigate(RegionNames.ContentRegion, "DashboardView");

            RegionManager.AddToRegion(RegionNames.ContentRegion, Container.Resolve<SociosContentView>());
            RegionManager.AddToRegion(RegionNames.SociosTabContentRegion, Container.Resolve<ListadoDeSociosView>());
        }
    }
}
