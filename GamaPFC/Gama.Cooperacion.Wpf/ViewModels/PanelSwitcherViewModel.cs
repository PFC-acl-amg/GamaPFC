//using Core;
//using Gama.Common;
//using NHibernate;
//using Prism.Commands;
//using Prism.Regions;


using Core;
using Gama.Common;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Common.Eventos;
using NHibernate;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Windows.Input;
using System;

namespace Gama.Cooperacion.Wpf.ViewModels
{
    public class PanelSwitcherViewModel : ViewModelBase
    {
        private IRegionManager _regionManager;
        private string _ActivePanel;
        private IEventAggregator _EventAggregator;
        public PanelSwitcherViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _EventAggregator = eventAggregator;
            ActivePanel = "DashboardView";
            this.NavigateCommand = new DelegateCommand<string>(Navigate);

            _EventAggregator.GetEvent<ActividadSeleccionadaEvent>().Subscribe(OnActividadSeleccionadaEvent);
            _EventAggregator.GetEvent<ActividadCreadaEvent>().Subscribe(OnNuevaActividadEvent);

            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnActiveViewChangedEvent);

        }

        private void OnActiveViewChangedEvent(string viewName)
        {
            ActivePanel = viewName;
        }

        private void Navigate(string viewName)
        {
            ActivePanel = viewName;
            _EventAggregator.GetEvent<ActiveViewChanged>().Publish(viewName);
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
