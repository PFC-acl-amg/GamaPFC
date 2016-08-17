using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public abstract class ModuleBase : IModule
    {
        protected IUnityContainer Container { get; set; }
        protected IRegionManager RegionManager { get; private set; }
        public bool Debug { get; set; }

        public ModuleBase(IUnityContainer container, IRegionManager regionManager)
        {
            this.Container = container;
            this.RegionManager = regionManager;
        }

        public abstract void Initialize();
    }
}
