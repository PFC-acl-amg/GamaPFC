using Core;
using Core.DataAccess;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.UIEvents;
using Gama.Atenciones.Wpf.ViewModels;
using Gama.Common.Eventos;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
        Dictionary<string, bool> _Panels = new Dictionary<string, bool>();
        private Preferencias _Preferencias;

        public ShellViewModel(
            IEventAggregator eventAggregator,
            PersonaRepository personaRepository,
            CitaRepository citaRepository,
            AtencionRepository atencionRepository,
            AsistenteRepository asistenteRepository,
            StatusBarViewModel statusBarViewModel,
            SearchBoxViewModel searchBoxViewModel,
            PersonasContentViewModel personasContentViewModel,
            DashboardViewModel dashboardViewModel,
            CitasContentViewModel citasContentViewModel,
            AsistentesContentViewModel asistentesContentViewModel,
            GraficasContentViewModel graficasContentViewModel,
            ToolbarViewModel toolbarViewModel,
            Preferencias preferencias,
            ISession session)
        {
            SearchBoxViewModel = searchBoxViewModel;
            PersonasContentViewModel = personasContentViewModel;
            DashboardViewModel = dashboardViewModel;
            CitasContentViewModel = citasContentViewModel;
            AsistentesContentViewModel = asistentesContentViewModel;
            GraficasContentViewModel = graficasContentViewModel;
            StatusBarViewModel = statusBarViewModel;
            ToolbarViewModel = toolbarViewModel;
            _Preferencias = preferencias;

            _PersonaRepository = personaRepository;
            _CitaRepository = citaRepository;
            _AtencionRepository = atencionRepository;
            _AsistenteRepository = asistenteRepository;

            _PersonaRepository.Session = session;
            _CitaRepository.Session = session;
            _AtencionRepository.Session = session;
            _AsistenteRepository.Session = session;

            _EventAggregator = eventAggregator;
            Title = "SERVICIO DE ATENCIONES";
            _EventAggregator.GetEvent<AbrirPreferenciasEvent>().Subscribe(OnTogglePreferenciasEvent);
            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnActiveViewChangedEvent);
            _EventAggregator.GetEvent<ServidorActualizadoDesdeFueraEvent>().Subscribe(OnServidorActualizadoDesdeFueraEvent);

            _Panels.Add("DashboardView", false);
            _Panels.Add("PersonasContentView", false);
            _Panels.Add("CitasContentView", false);
            _Panels.Add("AsistentesContentView", false);
            _Panels.Add("GraficasContentView", false);

            SetVisiblePanel("DashboardView");
        }

        public PersonasContentViewModel PersonasContentViewModel { get; set; }
        public DashboardViewModel DashboardViewModel { get; set; }
        public CitasContentViewModel CitasContentViewModel { get; set; }
        public AsistentesContentViewModel AsistentesContentViewModel { get; set; }
        public GraficasContentViewModel GraficasContentViewModel { get; set; }
        public StatusBarViewModel StatusBarViewModel { get; set; }
        public SearchBoxViewModel SearchBoxViewModel { get; set; }
        public ToolbarViewModel ToolbarViewModel { get; set; }

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
        private PersonaRepository _PersonaRepository;
        private CitaRepository _CitaRepository;
        private AtencionRepository _AtencionRepository;
        private AsistenteRepository _AsistenteRepository;

        public bool GraficasContentViewIsVisible
        {
            get { return _GraficasViewIsVisible; }
            set { SetProperty(ref _GraficasViewIsVisible, value); }
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

        private void OnServidorActualizadoDesdeFueraEvent()
        {
            _AsistenteRepository.ActualizarCliente();

            AsistentesContentViewModel.OnActualizarServidor();
            CitasContentViewModel.OnActualizarServidor();
            DashboardViewModel.OnActualizarServidor();
            GraficasContentViewModel.OnActualizarServidor();
            PersonasContentViewModel.OnActualizarServidor();
            SearchBoxViewModel.OnActualizarServidor();
            ToolbarViewModel.OnActualizarServidor();
        }

        public void OnCloseApplication()
        {
            var preferencias = _Preferencias;
            if (preferencias.DoBackupOnClose)
            {
                string connectionString =
                    ConfigurationManager.ConnectionStrings["GamaAtencionesMySql"].ConnectionString;
                DBHelper.Backup(
                    connectionString: connectionString, 
                    fileName: preferencias.AutomaticBackupPath + DateTime.Now.ToString().Replace('/', '-').Replace(':','-') + " - atenciones backup.sql");

                DirectoryInfo directory = new DirectoryInfo(preferencias.AutomaticBackupPath);

                if (preferencias.BackupDeleteDateLimit.HasValue)
                    foreach (FileInfo fileInfo in directory.GetFiles())
                        if (fileInfo.CreationTime < preferencias.BackupDeleteDateLimit.Value
                            && fileInfo.CreationTime < DateTime.Now.Date) // Para no borrar el que acabamos de poner
                            fileInfo.Delete();

                AtencionesResources.ClientService.Desconectar();
            }
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
