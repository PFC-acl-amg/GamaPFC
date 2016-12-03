using Core;
using Core.DataAccess;
using Gama.Socios.DataAccess;
using Gama.Socios.Wpf.Services;
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
    public class SociosModule : ModuleBase
    {
        public SociosModule(IUnityContainer container, IRegionManager regionManager)
           : base(container, regionManager)
        {
            this.Entorno = Entorno.Desarrollo;
            this.UseFaker = true;
        }

        public override void Initialize()
        {
            RegisterViews();
            RegisterViewModels();
            RegisterServices();

            #region Database Seeding
            try
            {
                if (UseFaker)
                {
                    var sessionFactory = Container.Resolve<INHibernateSessionFactory>();

                    var personaRepository = new SocioRepository();
                    var session = sessionFactory.OpenSession();
                    personaRepository.Session = session;

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

            InitializeNavigation();
        }

        private void RegisterViews()
        {
            
        }

        private void RegisterViewModels()
        {

        }

        private void RegisterServices()
        {
            Container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactory());
            Container.RegisterType<ISession>(
                new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenSession()));
            Container.RegisterType<ISocioRepository, SocioRepository>();
        }

        private void InitializeNavigation()
        {

        }
    }
}
