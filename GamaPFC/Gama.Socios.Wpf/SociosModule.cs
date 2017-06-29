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
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf
{
    public class SociosModule : ModuleBase
    {
        public SociosModule(IUnityContainer container, IRegionManager regionManager)
           : base(container, regionManager)
        {

        }

        public override void Initialize()
        {
            RegisterViews();
            RegisterViewModels();
            InitializeNavigation();
        }

        private void RegisterViews()
        {
            Container.RegisterType<object, ContabilidadView>("ContabilidadView");
            Container.RegisterType<object, DashboardView>("DashboardView");
            Container.RegisterType<object, EditarCuotasView>("EditarCuotasView");
            Container.RegisterType<object, EditarPeriodosDeAltaView>("EditarPeriodosDeAltaView");
            Container.RegisterType<object, EditarSocioView>("EditarSocioView");
            Container.RegisterType<object, ListadoDeSociosView>("ListadoDeSociosView");
            Container.RegisterType<object, PanelSwitcherView>("PanelSwitcherView");
            Container.RegisterType<object, SociosContentView>("SociosContentView");
            Container.RegisterType<object, SocioView>("SocioView");
            Container.RegisterType<object, ToolbarView>("ToolbarView");
            Container.RegisterType<object, RightCommandsView>("RightCommandsView");
            Container.RegisterType<object, PreferenciasView>("PreferenciasView");
        }

        private void RegisterViewModels()
        {
            Container.RegisterType<ContabilidadViewModel>();
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
            Container.RegisterType<PreferenciasViewModel>();
            Container.RegisterType<RightCommandsViewModel>();
        }

        private void InitializeNavigation()
        {
            //RegionManager.RegisterViewWithRegion(RegionNames.PanelSwitcherRegion, typeof(PanelSwitcherView));
            //RegionManager.RegisterViewWithRegion(RegionNames.ToolbarRegion, typeof(ToolbarView));
            //RegionManager.RegisterViewWithRegion(RegionNames.SearchBoxRegion, typeof(SearchBoxView));
            //RegionManager.RegisterViewWithRegion(RegionNames.StatusBarRegion, typeof(StatusBarView));
            RegionManager.RegisterViewWithRegion(RegionNames.RightCommandsRegion, typeof(RightCommandsView));
            //RegionManager.RegisterViewWithRegion(RegionNames.PreferenciasRegion, typeof(PreferenciasView));
            RegionManager.RequestNavigate(RegionNames.ContentRegion, "DashboardView");

            RegionManager.AddToRegion(RegionNames.ContentRegion, Container.Resolve<SociosContentView>());
            RegionManager.AddToRegion(RegionNames.ContentRegion, Container.Resolve<ContabilidadView>());
            RegionManager.AddToRegion(RegionNames.SociosTabContentRegion, Container.Resolve<ListadoDeSociosView>());
        }
    }
}
