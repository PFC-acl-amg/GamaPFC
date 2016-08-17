using Core;
using Gama.Common;
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
            Container.RegisterType<DashboardView>();

            RegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(DashboardView));
        }
    }
}
