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
            DashboardViewModel dashboardViewModel,
            ActividadesContentViewModel actividadesContentVieWModel,
            PanelSwitcherViewModel panelSwitcherViewModel,
            CalendarioDeActividadesViewModel calendarioDeActividadesViewModel,
            CooperantesContentViewModel cooperantesContentViewModel,
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
            DashboardViewModel = dashboardViewModel;
            _Preferencias = preferencias;
            CalendarioDeActividadesViewModel = calendarioDeActividadesViewModel;
            CooperantesContentViewModel = cooperantesContentViewModel;
            ActividadesContentViewModel = actividadesContentVieWModel;

            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnActiveViewChangedEvent);
            _EventAggregator.GetEvent<ServidorActualizadoDesdeFueraEvent>().Subscribe(OnServidorActualizadoDesdeFueraEvent);

            _Panels.Add("DashboardView", false);
            _Panels.Add("CalendarioDeActividadesView", false);
            _Panels.Add("CooperantesContentView", false);
            _Panels.Add("ActividadesContentView", false);
            _Panels.Add("GraficasContentView", false);

            SetVisiblePanel("DashboardView");

            _PreloadThread = new Thread(ConectarConServidor);
            _PreloadThread.SetApartmentState(ApartmentState.STA);
            _PreloadThread.Start();
        }

        public PanelSwitcherViewModel PanelSwitcherViewModel { get; set; }
        public ToolbarViewModel ToolbarViewModel { get; set; }
        public StatusBarViewModel StatusBarViewModel { get; set; }
        public CalendarioDeActividadesViewModel CalendarioDeActividadesViewModel { get; set; }
        public CooperantesContentViewModel CooperantesContentViewModel { get; set; }
        public DashboardViewModel DashboardViewModel { get; set; }
        public ActividadesContentViewModel ActividadesContentViewModel { get; set; }

        private bool _DashboardViewIsVisible = true;
        public bool DashboardViewIsVisible
        {
            get { return _DashboardViewIsVisible; }
            set { SetProperty(ref _DashboardViewIsVisible, value); }
        }

        private bool _CooperantesContentViewIsvisible = false;
        public bool CooperantesContentViewIsVisible
        {
            get { return _CooperantesContentViewIsvisible; }
            set { SetProperty(ref _CooperantesContentViewIsvisible, value); }
        }

        private bool _ActividadesContentViewIsVisible = false;
        public bool ActividadesContentViewIsVisible
        {
            get { return _ActividadesContentViewIsVisible; }
            set { SetProperty(ref _ActividadesContentViewIsVisible, value); }
        }

        private bool _CalendarioDeActividadesViewIsVisible = false;
        public bool CalendarioDeActividadesViewIsVisible
        {
            get { return _CalendarioDeActividadesViewIsVisible; }
            set { SetProperty(ref _CalendarioDeActividadesViewIsVisible, value); }
        }

        private bool _GraficasContentViewIsVisible = false;
        public bool GraficasContentViewIsVisible
        {
            get { return _GraficasContentViewIsVisible; }
            set { SetProperty(ref _GraficasContentViewIsVisible, value); }
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
            _Panels["CalendarioDeActividadesView"] = false;
            _Panels["CooperantesContentView"] = false;
            _Panels["ActividadesContentView"] = false;
            //_Panels["AsistentesContentView"] = false;
            _Panels["GraficasContentView"] = false;

            _Panels[panel] = true;

            DashboardViewIsVisible = _Panels["DashboardView"];
            CalendarioDeActividadesViewIsVisible = _Panels["CalendarioDeActividadesView"];
            CooperantesContentViewIsVisible = _Panels["CooperantesContentView"];
            ActividadesContentViewIsVisible = _Panels["ActividadesContentView"];
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
