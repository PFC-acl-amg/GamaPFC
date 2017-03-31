using Core;
using Gama.Atenciones.Wpf.UIEvents;
using Gama.Common;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gama.Atenciones.Wpf.ViewModels
{
    public class PanelSwitcherViewModel : ViewModelBase
    {
        private string _ActivePanel;
        private IEventAggregator _EventAggregator;
        private IRegionManager _RegionManager;

        public PanelSwitcherViewModel() { }

        public PanelSwitcherViewModel(IRegionManager regionManager,
            IEventAggregator eventAggregator)
        {
            _RegionManager = regionManager;
            _EventAggregator = eventAggregator;

            ActivePanel = "DashboardView";

            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnActiveViewChangedEvent);

            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        public string ActivePanel
        {
            get { return _ActivePanel; }
            set { SetProperty(ref _ActivePanel, value); }
        }

        public ICommand NavigateCommand { get; private set; }

        private void OnActiveViewChangedEvent(string viewName)
        {
            ActivePanel = viewName;
        }

        private void Navigate(string viewName)
        {
            _RegionManager.RequestNavigate(RegionNames.ContentRegion, viewName);
        }

    }
}
