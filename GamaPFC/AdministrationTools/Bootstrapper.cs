using AdministrationTools.Controllers;
using Core.DataAccess;
using Gama.Atenciones.DataAccess;
using Gama.Atenciones.Wpf.Services;
using Gama.Socios.Wpf.Services;
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
            container.RegisterType<Gama.Socios.DataAccess.NHibernateSessionFactoryWeb>();
            var sociosSessionFactory = new Gama.Socios.DataAccess.NHibernateSessionFactoryWeb();
            var session = sociosSessionFactory.SessionFactory.OpenSession();
            //var socioRepository = new SocioRepository();

            container.RegisterType<ISocioRepository, SocioRepository>();


            container.RegisterType<AccountController>(new InjectionConstructor());
            //container.RegisterType<RolesAdminController>(new InjectionConstructor());
            container.RegisterType<ManageController>(new InjectionConstructor());
            //container.RegisterType<UsersAdminController>(new InjectionConstructor());
        }
    }
}
