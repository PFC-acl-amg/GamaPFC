using Core;
using Gama.Atenciones.Wpf.UIEvents;
using Gama.Atenciones.Wpf.ViewModels;
using Gama.Common.Eventos;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gama.Atenciones.Wpf
{
    public class ShellViewModel : ViewModelBase
    {
        private IEventAggregator _EventAggregator;
        private ImageSource _IconSource;
        private bool _PreferenciasFlyoutIsOpen = false;

        public ShellViewModel(IEventAggregator eventAggregator,
            PersonasContentViewModel personasContentViewModel,
            DashboardViewModel dashboardViewModel,
            CitasContentViewModel citasContentViewModel)
        {
            PersonasContentViewModel = personasContentViewModel;
            DashboardViewModel = dashboardViewModel;
            _CitasContentViewModel = citasContentViewModel;
            _EventAggregator = eventAggregator;
            Title = "Módulo no cargado";
            _EventAggregator.GetEvent<AbrirPreferenciasEvent>().Subscribe(OnTogglePreferenciasEvent);
            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnActiveViewChangedEvent);

            DashboardViewIsVisible = true;
            PersonasContentViewIsVisible = false;
        }

        private object _SelectedViewModel;
        public PersonasContentViewModel PersonasContentViewModel { get; set; }
        public DashboardViewModel DashboardViewModel { get; set; }
        private CitasContentViewModel _CitasContentViewModel;

        public object SelectedViewModel
        {
            get { return _SelectedViewModel; }
            set { SetProperty(ref _SelectedViewModel, value); }
        }

        private bool _DashboardViewIsVisible = true;
        public bool DashboardViewIsVisible
        {
            get { return _DashboardViewIsVisible; }
            set { SetProperty(ref _DashboardViewIsVisible, value); }
        }

        private bool _PersonasContentViewIsVisible = false;
        public bool PersonasContentViewIsVisible
        {
            get { return _PersonasContentViewIsVisible; }
            set { SetProperty(ref _PersonasContentViewIsVisible, value); }
        }

        private void OnActiveViewChangedEvent(string viewName)
        {
            switch (viewName)
            {
                case "DashboardView":
                    DashboardViewIsVisible = true;
                    PersonasContentViewIsVisible = false;
                    //SelectedViewModel = _DashboardViewModel;
                    break;
                case "PersonasContentView":
                    DashboardViewIsVisible = false;
                    PersonasContentViewIsVisible = true;
                    //SelectedViewModel = _PersonasContentViewModel;
                    break;
                case "CitasContentView":
                    //SelectedViewModel = _CitasContentViewModel;
                    break;
            }
        }

        private void OnTogglePreferenciasEvent()
        {
            PreferenciasFlyoutIsOpen = true;
        }

        public bool PreferenciasFlyoutIsOpen
        {
            get { return _PreferenciasFlyoutIsOpen; }
            set { SetProperty(ref _PreferenciasFlyoutIsOpen, value); }
        }

        public ImageSource IconSource
        {
            get { return _IconSource; }
            set { SetProperty(ref _IconSource, value); }
        }
    }
}
