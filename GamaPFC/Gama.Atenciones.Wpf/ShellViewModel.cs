using Core;
using Gama.Atenciones.Wpf.UIEvents;
using Gama.Atenciones.Wpf.ViewModels;
using Gama.Common.Eventos;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            CitasContentViewModel citasContentViewModel,
            AsistentesContentViewModel asistentesContentViewModel,
            GraficasContentViewModel graficasContentViewModel)
        {
            PersonasContentViewModel = personasContentViewModel;
            DashboardViewModel = dashboardViewModel;
            CitasContentViewModel = citasContentViewModel;
            AsistentesContentViewModel = asistentesContentViewModel;
            GraficasContentViewModel = graficasContentViewModel;

            _EventAggregator = eventAggregator;
            Title = "SERVICIO DE ATENCIONES";
            _EventAggregator.GetEvent<AbrirPreferenciasEvent>().Subscribe(OnTogglePreferenciasEvent);
            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnActiveViewChangedEvent);

            SetVisiblePanel("DashboardView");
        }
        
        public PersonasContentViewModel PersonasContentViewModel { get; set; }
        public DashboardViewModel DashboardViewModel { get; set; }
        public CitasContentViewModel CitasContentViewModel { get; set; }
        public AsistentesContentViewModel AsistentesContentViewModel { get; set; }
        public GraficasContentViewModel GraficasContentViewModel { get; set; }

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

        private bool _CitasContentViewIsVisible = false;
        public bool CitasContentViewIsVisible
        {
            get { return _CitasContentViewIsVisible; }
            set { SetProperty(ref _CitasContentViewIsVisible, value); }
        }

        private bool _AsistentesContentViewIsVisible = false;
        public bool AsistentesContentViewIsVisible
        {
            get { return _AsistentesContentViewIsVisible; }
            set { SetProperty(ref _AsistentesContentViewIsVisible, value); }
        }

        private bool _GraficasViewIsVisible = false;
        public bool GraficasContentViewIsVisible
        {
            get { return _GraficasViewIsVisible; }
            set { SetProperty(ref _GraficasViewIsVisible, value); }
        }

        private void SetVisiblePanel(string panel)
        {
            Dictionary<string, bool> values = new Dictionary<string, bool>();
            values.Add("DashboardView", false);
            values.Add("PersonasContentView", false);
            values.Add("CitasContentView", false);
            values.Add("AsistentesContentView", false);
            values.Add("GraficasContentView", false);

            values[panel] = true;

            DashboardViewIsVisible = values["DashboardView"];
            PersonasContentViewIsVisible = values["PersonasContentView"];
            CitasContentViewIsVisible = values["CitasContentView"];
            AsistentesContentViewIsVisible = values["AsistentesContentView"];
            GraficasContentViewIsVisible = values["GraficasContentView"];
        }

        private void OnActiveViewChangedEvent(string viewName)
        {
            SetVisiblePanel(viewName);
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
