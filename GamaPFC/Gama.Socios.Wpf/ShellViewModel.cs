using Core;
using Core.DataAccess;
using Gama.Common.Communication;
using Gama.Common.Eventos;
using Gama.Socios.Wpf.Services;
using Gama.Socios.Wpf.ViewModels;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gama.Socios.Wpf
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
            ContabilidadViewModel contabilidadViewModel,
            SociosContentViewModel sociosContentViewModel,
            SearchBoxViewModel searchBoxViewModel,
            PanelSwitcherViewModel panelSwitcherViewModel,
            ToolbarViewModel toolbarViewModel,
            StatusBarViewModel statusBarViewModel,
            RightCommandsViewModel rightCommandsViewModel,
            Preferencias preferencias,
            ISession session
           )
        {
            DashboardViewModel = dashboardViewModel;
            ContabilidadViewModel = contabilidadViewModel;
            SociosContentViewModel = sociosContentViewModel;
            SearchBoxViewModel = searchBoxViewModel;
            PanelSwitcherViewModel = panelSwitcherViewModel;
            ToolbarViewModel = toolbarViewModel;
            StatusBarViewModel = statusBarViewModel;
            RightCommandsViewModel = rightCommandsViewModel;
            _Preferencias = preferencias;

            _EventAggregator = eventAggregator;
            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnActiveViewChangedEvent);
            _EventAggregator.GetEvent<ServidorActualizadoDesdeFueraEvent>().Subscribe(OnServidorActualizadoDesdeFueraEvent);

            _Panels.Add("DashboardView", false);
            _Panels.Add("ContabilidadView", false);
            _Panels.Add("SociosContentView", false);
            //_Panels.Add("AsistentesContentView", false);
            //_Panels.Add("GraficasContentView", false);

            SetVisiblePanel("DashboardView");

            _PreloadThread = new Thread(ConectarConServidor);
            _PreloadThread.SetApartmentState(ApartmentState.STA);
            _PreloadThread.Start();
        }

        public DashboardViewModel DashboardViewModel { get; private set; }
        public ContabilidadViewModel ContabilidadViewModel { get; private set; }
        public SociosContentViewModel SociosContentViewModel { get; private set; }
        public SearchBoxViewModel SearchBoxViewModel { get; private set; }
        public PanelSwitcherViewModel PanelSwitcherViewModel { get; private set; }
        public ToolbarViewModel ToolbarViewModel { get; private set; }
        public StatusBarViewModel StatusBarViewModel { get; private set; }
        public RightCommandsViewModel RightCommandsViewModel { get; private set; }

        private bool _DashboardViewIsVisible = true;
        public bool DashboardViewIsVisible
        {
            get { return _DashboardViewIsVisible; }
            set { SetProperty(ref _DashboardViewIsVisible, value); }
        }

        private bool _ContabilidadViewIsVisible = false;
        public bool ContabilidadViewIsVisible
        {
            get { return _ContabilidadViewIsVisible; }
            set { SetProperty(ref _ContabilidadViewIsVisible, value); }
        }

        private bool _SociosContentViewIsVisible = false;
        public bool SociosContentViewIsVisible
        {
            get { return _SociosContentViewIsVisible; }
            set { SetProperty(ref _SociosContentViewIsVisible, value); }
        }

        public ImageSource IconSource
        {
            get { return _IconSource; }
            set { SetProperty(ref _IconSource, value); }
        }

        private void ConectarConServidor()
        {
            SociosResources.ClientService = new ClientService(_EventAggregator, SociosResources.ClientId);
        }

        private void SetVisiblePanel(string panel)
        {
            _Panels["DashboardView"] = false;
            _Panels["ContabilidadView"] = false;
            _Panels["SociosContentView"] = false;
            //_Panels["AsistentesContentView"] = false;
            //_Panels["GraficasContentView"] = false;

            _Panels[panel] = true;

            DashboardViewIsVisible = _Panels["DashboardView"];
            ContabilidadViewIsVisible = _Panels["ContabilidadView"];
            SociosContentViewIsVisible = _Panels["SociosContentView"];
            //CitasContentViewIsVisible = _Panels["CitasContentView"];
            //AsistentesContentViewIsVisible = _Panels["AsistentesContentView"];
            //GraficasContentViewIsVisible = _Panels["GraficasContentView"];
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
            try
            {
                var preferencias = _Preferencias;
                if (preferencias.DoBackupOnClose)
                {
                    string connectionString =
                        ConfigurationManager.ConnectionStrings["GamaSociossMySql"].ConnectionString;
                    DBHelper.Backup(
                        connectionString: connectionString,
                        fileName: preferencias.AutomaticBackupPath + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + " - socios backup.sql");

                    DirectoryInfo directory = new DirectoryInfo(preferencias.AutomaticBackupPath);

                    if (preferencias.BackupDeleteDateLimit.HasValue)
                        foreach (FileInfo fileInfo in directory.GetFiles())
                            if (fileInfo.CreationTime < preferencias.BackupDeleteDateLimit.Value
                                && fileInfo.CreationTime < DateTime.Now.Date) // Para no borrar el que acabamos de poner
                                fileInfo.Delete();

                    if (SociosResources.ClientService != null)
                        SociosResources.ClientService.Desconectar();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void OnActiveViewChangedEvent(string viewName)
        {
            SetVisiblePanel(viewName);
        }
    }
}
