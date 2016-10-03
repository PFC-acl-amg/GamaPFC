using Core;
using Core.DataAccess;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf
{
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
            RegisterViews();
            RegisterViewModels();
            RegisterServices();
            InitializeNavigation();

            try {
                var sessionFactory = new NHibernateSessionFactory();
                var factory = sessionFactory.GetSessionFactory();

                if (UseFaker)
                {
                    var personaRepository = new PersonaRepository();
                    var session = factory.OpenSession();
                    personaRepository.Session = session;

                    var personas = new FakePersonaRepository().GetAll();
                    foreach (var persona in personas)
                    {
                        persona.Id = 0;
                        personaRepository.Create(persona);
                    }

                    var citaRepository = new CitaRepository();
                    citaRepository.Session = session;
                    var citas = new FakeCitaRepository().GetAll();
                    var personaParaCita = personaRepository.GetById(1);
                    foreach (var cita in citas)
                    {
                        cita.Id = 0;
                        cita.Persona = personaParaCita;
                        citaRepository.Create(cita);
                    }

                    var atencionRepository = new AtencionRepository();
                    atencionRepository.Session = session;
                    var atenciones = new FakeAtencionRepository().GetAll();
                    var citaParaAtencion = citaRepository.GetById(1);
                    foreach ( var atencion in atenciones )
                    {
                        atencion.Id = 0;
                        atencion.Cita = citaParaAtencion;
                        atencionRepository.Create(atencion);
                    }
                }
            } 
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }

        private void RegisterViews()
        {
            Container.RegisterType<object, DashboardView>("DashboardView");
        }

        private void RegisterViewModels()
        {
            Container.RegisterType<DashboardViewModel>();
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
            RegionManager.RequestNavigate(RegionNames.ContentRegion, "DashboardView");
        }
    }
}
