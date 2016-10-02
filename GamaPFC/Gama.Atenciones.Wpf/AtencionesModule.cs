using Core;
using Gama.Atenciones.DataAccess;
using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Wpf.Services;
using Microsoft.Practices.Unity;
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

                    //var personas = new FakePersonaRepository().GetAll();
                    //foreach(var persona in personas)
                    //{
                    //    persona.Id = 0;
                    //    personaRepository.Create(persona);
                    //}

                    var citaRepository = new CitaRepository();
                    citaRepository.Session = session;
                    var citas = new FakeCitaRepository().GetAll();
                    var personaParaCita = personaRepository.GetById(1);
                    foreach(var cita in citas)
                    {
                        cita.Id = 0;
                        cita.Persona = personaParaCita;
                        citaRepository.Create(cita);
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
            //throw new NotImplementedException();
        }

        private void RegisterViewModels()
        {
            //throw new NotImplementedException();
        }

        private void RegisterServices()
        {

            //throw new NotImplementedException();
        }

        private void InitializeNavigation()
        {
            //throw new NotImplementedException();
        }
    }
}
