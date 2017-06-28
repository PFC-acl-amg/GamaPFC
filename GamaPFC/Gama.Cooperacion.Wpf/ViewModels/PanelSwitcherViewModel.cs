//using Core;
//using Gama.Common;
//using NHibernate;
//using Prism.Commands;
//using Prism.Regions;


using Core;
using Gama.Common;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.UIEvents;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Windows.Input;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class PanelSwitcherViewModel : ViewModelBase
    {
        private IRegionManager _regionManager;
        private string _ActivePanel;
        private IEventAggregator _eventAggregator;
        public PanelSwitcherViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            ActivePanel = "DashboardView";
            this.NavigateCommand = new DelegateCommand<string>(Navigate);

            _eventAggregator.GetEvent<ActividadSeleccionadaEvent>().Subscribe(OnActividadSeleccionadaEvent);
            _eventAggregator.GetEvent<NuevaActividadEvent>().Subscribe(OnNuevaActividadEvent);
            
        }

        private void Navigate(string viewName)
        {
            ActivePanel = viewName;
            _regionManager.RequestNavigate(RegionNames.ContentRegion, viewName);
        }
        private void OnNuevaActividadEvent(int id)
        {
            ActivePanel = "ActividadesContentView";
        }
        private void OnActividadSeleccionadaEvent(int id)
        {
            ActivePanel = "ActividadesContentView";
        }
        public DelegateCommand<string> NavigateCommand { get; private set; }
        public string ActivePanel
        {
            get { return _ActivePanel; }
            set { SetProperty(ref _ActivePanel, value); }
        }
    }
}
