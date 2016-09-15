using Core;
using Gama.Common;
using NHibernate;
using Prism.Commands;
using Prism.Regions;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class PanelSwitcherViewModel : ViewModelBase
    {
        private IRegionManager _regionManager;

        public PanelSwitcherViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            this.NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        private void Navigate(string viewName)
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, viewName);
        }

        public DelegateCommand<string> NavigateCommand { get; private set; }
    }
}
