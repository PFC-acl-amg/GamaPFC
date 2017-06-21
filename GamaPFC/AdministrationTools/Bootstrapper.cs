using Core.DataAccess;
using Gama.Atenciones.DataAccess;
using Gama.Atenciones.Wpf.Services;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdministrationTools
{
    public class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new
            UnityDependencyResolver(container));

            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            RegisterTypes(container);

            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactoryWeb());
            container.RegisterType<ISession>(
                new InjectionFactory(c => container.Resolve<INHibernateSessionFactory>().OpenSession()));
            container.RegisterType<IPersonaRepository, PersonaRepository>();
            container.RegisterType<ICitaRepository, CitaRepository>();
            container.RegisterType<IAtencionRepository, AtencionRepository>();
            container.RegisterType<IAsistenteRepository, AsistenteRepository>();
        }
    }
}
