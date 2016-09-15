using Core;
using Core.DataAccess;
using Gama.Common;
using Gama.Cooperacion.DataAccess;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.ViewModels;
using Gama.Cooperacion.Wpf.Views;
using Microsoft.Practices.Unity;
using NHibernate;
using Prism.Logging;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf
{
    public class CooperacionModule : ModuleBase
    {
        public CooperacionModule(IUnityContainer container, IRegionManager regionManager)
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

            ILoggerFacade log = Container.Resolve<ILoggerFacade>();
            log.Log("ok", Category.Exception, Priority.None);

            if (this.UseFaker)
            {
                var cooperanteRepository = Container.Resolve<ICooperanteRepository>();
                var cooperantesDummy = new FakeCooperanteRepository().GetAll();

                foreach (var cooperante in cooperantesDummy)
                {
                    cooperanteRepository.Create(cooperante);
                }
            }
        }

        private void RegisterViews()
        {
            Container.RegisterType<object, ActividadDetailView>("ActividadDetailView");
            Container.RegisterType<object, ActividadesContentView>("ActividadesContentView");
            Container.RegisterType<object, DashboardView>("DashboardView");
            Container.RegisterType<object, ListadoDeActividadesView>("ListadoDeActividadesView");
            Container.RegisterType<object, NuevaActividadView>("NuevaActividadView");
            Container.RegisterType<object, PanelSwitcherView>("PanelSwitcherView");
            Container.RegisterType<object, ToolbarView>("ToolbarView");
        }

        private void RegisterViewModels()
        {
            Container.RegisterType<ActividadDetailViewModel>();
            Container.RegisterType<ActividadesContentViewModel>();
            Container.RegisterType<InformacionDeActividadViewModel>();
            Container.RegisterType<DashboardViewModel>();
            Container.RegisterType<ListadoDeActividadesViewModel>();
            Container.RegisterType<NuevaActividadViewModel>();
            Container.RegisterType<PanelSwitcherViewModel>();
            Container.RegisterType<ToolbarViewModel>();
        }

        private void RegisterServices()
        {
            //Container.RegisterInstance(typeof(INHibernateHelper), new NHibernateHelper());
            //Container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactory());

            //Container.RegisterInstance(typeof(ISessionHelper),
            //    new SessionHelper(Container.Resolve<INHibernateHelper>()));
            Container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactory());
            Container.RegisterType<ISession>(
                new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenSession()));
            Container.RegisterType<IStatelessSession>(
                new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenStatelessSession()));

            Container.RegisterType<IActividadRepository, ActividadRepository>();

            //Container.RegisterInstance(typeof(IActividadRepository),
            //    new ActividadRepository(Container.Resolve<ISessionHelper>()));
            Container.RegisterType<ICooperanteRepository, CooperanteRepository>();

            //Container.RegisterInstance(typeof(IActividadRepository), new FakeActividadRepository());
            //Container.RegisterInstance(typeof(ICooperanteRepository), new FakeCooperanteRepository());
        }

        private void InitializeNavigation()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.PanelSwitcherRegion, typeof(PanelSwitcherView));
            RegionManager.RegisterViewWithRegion(RegionNames.ToolbarRegion, typeof(ToolbarView));
            RegionManager.RequestNavigate(RegionNames.ContentRegion, "ActividadesContentView");
            RegionManager.RequestNavigate(RegionNames.ActividadesTabContentRegion, "ListadoDeActividadesView");
            RegionManager.RequestNavigate(RegionNames.ContentRegion, "DashboardView");
        }
    }
}
