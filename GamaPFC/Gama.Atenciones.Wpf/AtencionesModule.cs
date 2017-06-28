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
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Gama.Atenciones.Wpf
{
    public class AtencionesModule : ModuleBase
    {
        public AtencionesModule(IUnityContainer container, IRegionManager regionManager)
           : base(container, regionManager)
        { }

        public override void Initialize()
        {
            RegisterViews();
            RegisterViewModels();
            InitializeNavigation();
        }

        private void RegisterViews()
        {
            Container.RegisterType<object, AsistentesContentView>("AsistentesContentView");
            Container.RegisterType<object, AsistenteView>("AsistenteView");
            Container.RegisterType<object, CitasContentView>("CitasContentView");
            Container.RegisterType<object, DashboardView>("DashboardView");
            Container.RegisterType<object, EditarAtencionesView>("EditarAtencionesView");
            Container.RegisterType<object, EditarCitasView>("EditarCitasView");
            Container.RegisterType<object, EditarPersonaView>("EditarPersonaView");
            Container.RegisterType<object, GraficasContentView>("GraficasContentView");
            Container.RegisterType<object, ListadoDePersonasView>("ListadoDePersonasView");
            Container.RegisterType<object, PanelSwitcherView>("PanelSwitcherView");
            Container.RegisterType<object, PersonasContentView>("PersonasContentView");
            Container.RegisterType<object, StatusBarView>("StatusBarView");
            Container.RegisterType<object, ToolbarView>("ToolbarView");
            Container.RegisterType<object, RightCommandsView>("RightCommandsView");
            Container.RegisterType<object, PreferenciasView>("PreferenciasView");
        }

        private void RegisterViewModels()
        {
            Container.RegisterType<AsistentesContentViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<AsistenteViewModel>();
            Container.RegisterType<CitasContentViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<DashboardViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<EditarAtencionesViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<EditarCitasViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<EditarPersonaViewModel>();
            Container.RegisterType<GraficasContentViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ListadoDePersonasViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<PanelSwitcherViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<PersonasContentViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<RightCommandsViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<SearchBoxViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<StatusBarViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ToolbarViewModel>(new ContainerControlledLifetimeManager());
        }

        private void InitializeNavigation()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.RightCommandsRegion, typeof(RightCommandsView));
        }
    }
}
