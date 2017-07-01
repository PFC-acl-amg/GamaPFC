using Core;
using Gama.Common.Communication;
using Gama.Common.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.ViewModels;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gama.Cooperacion.Wpf
{
    public class ShellViewModel : ViewModelBase
    {
        private EventAggregator _EventAggregator;
        private ImageSource _IconSource;
        Dictionary<string, bool> _Panels = new Dictionary<string, bool>();
        private Preferencias _Preferencias;
        private Thread _PreloadThread;

        public ShellViewModel(
            EventAggregator eventAggregator,
            PanelSwitcherViewModel panelSwitcherViewModel,
            ToolbarViewModel toolbarViewModel,
            StatusBarViewModel statusBarViewModel,
            Preferencias preferencias,
            ISession session
            )
        {
            _EventAggregator = eventAggregator;
            PanelSwitcherViewModel = panelSwitcherViewModel;
            ToolbarViewModel = toolbarViewModel;
            StatusBarViewModel = statusBarViewModel;
            _Preferencias = preferencias;

            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnActiveViewChangedEvent);
            _EventAggregator.GetEvent<ServidorActualizadoDesdeFueraEvent>().Subscribe(OnServidorActualizadoDesdeFueraEvent);

            _Panels.Add("DashboardView", false);
            _Panels.Add("PersonasContentView", false);
            _Panels.Add("CitasContentView", false);
            _Panels.Add("AsistentesContentView", false);
            _Panels.Add("GraficasContentView", false);

            SetVisiblePanel("DashboardView");

            _PreloadThread = new Thread(ConectarConServidor);
            _PreloadThread.SetApartmentState(ApartmentState.STA);
            _PreloadThread.Start();
        }

        public PanelSwitcherViewModel PanelSwitcherViewModel { get; set; }
        public ToolbarViewModel ToolbarViewModel { get; set; }
        public StatusBarViewModel StatusBarViewModel { get; set; }

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

        public ImageSource IconSource
        {
            get { return _IconSource; }
            set { SetProperty(ref _IconSource, value); }
        }

        private void ConectarConServidor()
        {
            CooperacionResources.ClientService = new ClientService(_EventAggregator, CooperacionResources.ClientId);
        }

        private void SetVisiblePanel(string panel)
        {
            _Panels["DashboardView"] = false;
            _Panels["PersonasContentView"] = false;
            _Panels["CitasContentView"] = false;
            _Panels["AsistentesContentView"] = false;
            _Panels["GraficasContentView"] = false;

            _Panels[panel] = true;

            DashboardViewIsVisible = _Panels["DashboardView"];
            PersonasContentViewIsVisible = _Panels["PersonasContentView"];
            CitasContentViewIsVisible = _Panels["CitasContentView"];
            AsistentesContentViewIsVisible = _Panels["AsistentesContentView"];
            GraficasContentViewIsVisible = _Panels["GraficasContentView"];
        }

        private void OnServidorActualizadoDesdeFueraEvent(string code)
        {
            //if (code != AtencionesResources.ClientId)
            //{
            //    _AsistenteRepository.UpdateClient();
            //    _PersonaRepository.UpdateClient();
            //    _CitaRepository.UpdateClient();
            //    _AtencionRepository.UpdateClient();

            //    AsistentesContentViewModel.OnActualizarServidor();
            //    CitasContentViewModel.OnActualizarServidor();
            //    DashboardViewModel.OnActualizarServidor();
            //    GraficasContentViewModel.OnActualizarServidor();
            //    PersonasContentViewModel.OnActualizarServidor();
            //    SearchBoxViewModel.OnActualizarServidor();
            //    ToolbarViewModel.OnActualizarServidor();
            //}
        }

        public void OnCloseApplication()
        {
            //throw new NotImplementedException();
        }

        private void OnActiveViewChangedEvent(string viewName)
        {
            SetVisiblePanel(viewName);
        }
    }
}
