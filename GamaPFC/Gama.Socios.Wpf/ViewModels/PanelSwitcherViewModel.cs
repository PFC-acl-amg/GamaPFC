using Core;
using Gama.Common;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Socios.Wpf.ViewModels
{
    public class PanelSwitcherViewModel : ViewModelBase
    {
        private IRegionManager _RegionManager;

        public PanelSwitcherViewModel(IRegionManager regionManager)
        {
            _RegionManager = regionManager;

            this.NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        private void Navigate(string viewName)
        {
            _RegionManager.RequestNavigate(RegionNames.ContentRegion, viewName);
        }

        public ICommand NavigateCommand { get; private set; }
    }
}
