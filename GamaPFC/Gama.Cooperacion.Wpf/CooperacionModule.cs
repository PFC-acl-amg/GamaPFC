using Core;
using Core.DataAccess;
using Gama.Common;
using Gama.Cooperacion.Wpf.DataAccess;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.ViewModels;
using Gama.Cooperacion.Wpf.Views;
using Microsoft.Practices.Unity;
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

        }

        public override void Initialize()
        {
            Container.RegisterType<object, ActividadDetailView>("ActividadDetailView");
            Container.RegisterType<object, ActividadesContentView>("ActividadesContentView");
            Container.RegisterType<object, DashboardView>("DashboardView");
            Container.RegisterType<object, ListadoDeActividadesView>("ListadoDeActividadesView");
            Container.RegisterType<object, NuevaActividadView>("NuevaActividadView");
            Container.RegisterType<object, PanelSwitcherView>("PanelSwitcherView");
            Container.RegisterType<object, ToolbarView>("ToolbarView");

            Container.RegisterType<ActividadDetailViewModel>();
            Container.RegisterType<ActividadesContentViewModel>();
            Container.RegisterType<DashboardViewModel>();
            Container.RegisterType<ListadoDeActividadesViewModel>();
            Container.RegisterType<NuevaActividadViewModel>();
            Container.RegisterType<PanelSwitcherViewModel>();
            Container.RegisterType<ToolbarViewModel>();

            Container.RegisterInstance(typeof(INHibernateHelper), new NHibernateHelper());
            Container.RegisterInstance(typeof(ISessionHelper),
                new SessionHelper(Container.Resolve<INHibernateHelper>()));
            Container.RegisterInstance(typeof(IActividadRepository),
                new ActividadRepository(Container.Resolve<ISessionHelper>()));

            RegionManager.RegisterViewWithRegion(RegionNames.PanelSwitcherRegion, typeof(PanelSwitcherView));
            RegionManager.RegisterViewWithRegion(RegionNames.ToolbarRegion, typeof(ToolbarView));
            RegionManager.RequestNavigate(RegionNames.ContentRegion, "ActividadesContentView");
            RegionManager.RequestNavigate(RegionNames.ActividadesTabContentRegion, "ListadoDeActividadesView");
            RegionManager.RequestNavigate(RegionNames.ContentRegion, "DashboardView");

        }
    }
}
