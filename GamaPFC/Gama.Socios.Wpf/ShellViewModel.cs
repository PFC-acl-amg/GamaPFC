using Core;
using Core.DataAccess;
using Gama.Common;
using Gama.Common.Communication;
using Gama.Common.Eventos;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Services;
using Gama.Socios.Wpf.ViewModels;
using MySql.Data.MySqlClient;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
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
        private ISocioRepository _SocioRepository;
        private IPeriodoDeAltaRepository _PeriodoDeAltaRepository;
        private ICuotaRepository _CuotaRepository;
        private ImageSource _IconSource;
        private Preferencias _Preferencias;
        private Thread _PreloadThread;
        private List<Socio> _Socios = new List<Socio>();
        private List<PeriodoDeAlta> _PeriodoDeAlta = new List<PeriodoDeAlta>();
        private List<Cuota> _Cuotas = new List<Cuota>();
        Dictionary<string, bool> _Panels = new Dictionary<string, bool>();

        public ShellViewModel(
            EventAggregator eventAggregator,
            ISocioRepository socioRepository,
            IPeriodoDeAltaRepository periodoDeAltaRepository,
            ICuotaRepository cuotaRepository,
            DashboardViewModel dashboardViewModel,
            SociosContentViewModel sociosContentViewModel,
            SearchBoxViewModel searchBoxViewModel,
            PanelSwitcherViewModel panelSwitcherViewModel,
            ToolbarViewModel toolbarViewModel,
            StatusBarViewModel statusBarViewModel,
            RightCommandsViewModel rightCommandsViewModel,
            GraficasContentViewModel graficasContentViewModel,
            Preferencias preferencias,
            ISession session
           )
        {
            DashboardViewModel = dashboardViewModel;
            SociosContentViewModel = sociosContentViewModel;
            SearchBoxViewModel = searchBoxViewModel;
            PanelSwitcherViewModel = panelSwitcherViewModel;
            ToolbarViewModel = toolbarViewModel;
            StatusBarViewModel = statusBarViewModel;
            RightCommandsViewModel = rightCommandsViewModel;
            GraficasContentViewModel = graficasContentViewModel;
            _Preferencias = preferencias;

            _SocioRepository = socioRepository;
            _PeriodoDeAltaRepository = periodoDeAltaRepository;
            _CuotaRepository = cuotaRepository;

            _EventAggregator = eventAggregator;
            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnActiveViewChangedEvent);
            _EventAggregator.GetEvent<ServidorActualizadoDesdeFueraEvent>().Subscribe(OnServidorActualizadoDesdeFueraEvent);

            _Panels.Add("DashboardView", false);
            _Panels.Add("ContabilidadView", false);
            _Panels.Add("SociosContentView", false);
            _Panels.Add("GraficasContentView", false);

            SetVisiblePanel("DashboardView");

            _PreloadThread = new Thread(ConectarConServidor);
            _PreloadThread.SetApartmentState(ApartmentState.STA);
            _PreloadThread.Start();
        }

        public DashboardViewModel DashboardViewModel { get; private set; }
        public SociosContentViewModel SociosContentViewModel { get; private set; }
        public SearchBoxViewModel SearchBoxViewModel { get; private set; }
        public PanelSwitcherViewModel PanelSwitcherViewModel { get; private set; }
        public ToolbarViewModel ToolbarViewModel { get; private set; }
        public StatusBarViewModel StatusBarViewModel { get; private set; }
        public RightCommandsViewModel RightCommandsViewModel { get; private set; }
        public GraficasContentViewModel GraficasContentViewModel { get; private set; }

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
            SociosResources.ClientService = new ClientService(_EventAggregator, SociosResources.ClientId);
        }

        private void SetVisiblePanel(string panel)
        {
            _Panels["DashboardView"] = false;
            _Panels["ContabilidadView"] = false;
            _Panels["SociosContentView"] = false;
            _Panels["GraficasContentView"] = false;

            _Panels[panel] = true;

            DashboardViewIsVisible = _Panels["DashboardView"];
            SociosContentViewIsVisible = _Panels["SociosContentView"];
            GraficasContentViewIsVisible = _Panels["GraficasContentView"];
        }

        private void OnServidorActualizadoDesdeFueraEvent(string code)
        {
            string moduleName = code.Substring(code.IndexOf("@MODULO:@") + 9);
            string codigo = code.Substring(0, SociosResources.ClientId.Length);

            if (codigo != SociosResources.ClientId && moduleName.Contains("SOCIOS"))
            {
                DoRawThings();

                _SocioRepository.UpdateClient();
                
                DashboardViewModel.OnActualizarServidor();
                SociosContentViewModel.OnActualizarServidor();
                SearchBoxViewModel.OnActualizarServidor();
                ToolbarViewModel.OnActualizarServidor();
            }
        }

        private void DoRawThings()
        {
            _Socios.Clear();
            _PeriodoDeAlta.Clear();
            _Cuotas.Clear();

            Socio socio;
            MySqlDataReader reader;
            try
            {
                using (MySqlConnection mysqlConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["GamaSociosMySql"].ConnectionString))
                {
                    using (MySqlCommand sqlCommand = new MySqlCommand())
                    {
                        sqlCommand.Connection = mysqlConnection;
                        mysqlConnection.Open();
                        //UIServices.SetBusyState();

                        sqlCommand.CommandText = "SELECT Id, DireccionPostal, Email, FechaDeNacimiento, Facebook, Linkedin, " +
                            "Nacionalidad, Nif, Nombre, Telefono, Twitter, EstaDadoDeAlta, ImagenUpdatedAt, CreatedAt, UpdatedAt " +
                            "FROM socios ORDER BY Nombre ASC"
                            ;

                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                socio = new Socio()
                                {
                                    Id = (int)reader["Id"],
                                    Nombre = reader["Nombre"].ToString(),
                                    Nif = reader["Nif"].ToString(),
                                    _SavedNif = reader["Nif"].ToString(),
                                    DireccionPostal = reader["DireccionPostal"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    FechaDeNacimiento = reader["FechaDeNacimiento"] as DateTime?,
                                    Facebook = reader["Facebook"].ToString(),
                                    LinkedIn = reader["LinkedIn"].ToString(),
                                    Nacionalidad = reader["Nacionalidad"].ToString(),
                                    Telefono = reader["Telefono"].ToString(),
                                    Twitter = reader["Twitter"].ToString(),
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    UpdatedAt = reader["UpdatedAt"] as DateTime?,
                                    ImagenUpdatedAt = reader["ImagenUpdatedAt"] as DateTime?,
                                    EstaDadoDeAlta = (bool)reader["EstaDadoDeAlta"],
                                };

                                socio.Decrypt();
                                _Socios.Add(socio);
                            }
                        }

                        SociosResources.TodosLosNif = _Socios.Select(x => x.Nif).ToList();

                        foreach (var socioSinImagen in _Socios)
                        {
                            string path = ResourceNames.GetSocioImagePath(socioSinImagen.Id);
                            if (!File.Exists(path) && socioSinImagen.ImagenUpdatedAt != null)
                            {
                                sqlCommand.CommandText = $"SELECT Imagen FROM socios WHERE Id = {socioSinImagen.Id}";
                                using (reader = sqlCommand.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        socioSinImagen.Imagen = Core.Encryption.Cipher.Decrypt((reader["Imagen"] as byte[]));
                                        using (Image image = Image.FromStream(new MemoryStream(socioSinImagen.Imagen)))
                                        {
                                            using (MemoryStream memory = new MemoryStream())
                                            {
                                                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                                                {
                                                    image.Save(memory, ImageFormat.Jpeg);
                                                    byte[] bytes = memory.ToArray();
                                                    fs.Write(bytes, 0, bytes.Length);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Si se ha actualizado la imagen de la persona
                                DateTime lastWriteTime = File.GetLastWriteTime(path);
                                DateTime updatedTime = (socioSinImagen.ImagenUpdatedAt ?? DateTime.Now.AddYears(-100));
                                if (DateTime.Compare(lastWriteTime, updatedTime) < 0)
                                {
                                    sqlCommand.CommandText = $"SELECT Imagen FROM socios WHERE Id = {socioSinImagen.Id}";
                                    using (reader = sqlCommand.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            socioSinImagen.Imagen = Core.Encryption.Cipher.Decrypt((reader["Imagen"] as byte[]));
                                            using (Image image = Image.FromStream(new MemoryStream(socioSinImagen.Imagen)))
                                            {
                                                using (MemoryStream memory = new MemoryStream())
                                                {
                                                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                                                    {
                                                        image.Save(memory, ImageFormat.Jpeg);
                                                        byte[] bytes = memory.ToArray();
                                                        fs.Write(bytes, 0, bytes.Length);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                socioSinImagen.Imagen = ImageToByteArray(new Bitmap(ResourceNames.GetSocioImagePath(socioSinImagen.Id)));
                            }
                        }

                        //Debug.StopWatch("-----PERSONAS----");
                        //Debug.StartWatch();

                        sqlCommand.CommandText = "SELECT * FROM periodosdealta";

                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var periodoDeAlta = new PeriodoDeAlta
                                {
                                    Id = (int)reader["Id"],
                                    FechaDeAlta = (DateTime)reader["FechaDealta"],
                                    FechaDeBaja = reader["FechaDeBaja"] as DateTime?,
                                };

                                _PeriodoDeAlta.Add(periodoDeAlta);

                                socio = _Socios.Where(s => s.Id == (int)reader["Socio_id"]).Single();
                                socio.AddPeriodoDeAlta(periodoDeAlta);
                            }
                        }

                        sqlCommand.CommandText = "SELECT * FROM cuotas";
                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var cuota = new Cuota
                                {
                                    Id = (int)reader["Id"],
                                    Fecha = (DateTime)reader["Fecha"],
                                    CantidadPagada = (double)reader["CantidadPagada"],
                                    CantidadTotal = (double)reader["CantidadTotal"],
                                    Comentarios = (string)reader["Comentarios"],
                                    EstaPagado = (bool)reader["EstaPagado"],
                                    NoContabilizar = (bool)reader["NoContabilizar"],
                                };

                                _Cuotas.Add(cuota);

                                var periodoDeAlta = _PeriodoDeAlta.Where(x => x.Id == (int)reader["PeriodoDeAlta_id"]).Single();
                                periodoDeAlta.AddCuota(cuota);
                            }
                        }

                        mysqlConnection.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            _SocioRepository.Socios = _Socios;
            _PeriodoDeAltaRepository.PeriodosDeAlta = _PeriodoDeAlta;
            _CuotaRepository.Cuotas = _Cuotas;
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
        }

        public void OnCloseApplication()
        {
            try
            {
                if (SociosResources.ClientService != null)
                    SociosResources.ClientService.Desconectar();

                var preferencias = _Preferencias;
                if (preferencias.DoBackupOnClose)
                {
                    string connectionString =
                        ConfigurationManager.ConnectionStrings["GamaSociosMySql"].ConnectionString;
                    DBHelper.Backup(
                        connectionString: connectionString,
                        fileName: preferencias.AutomaticBackupPath + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + " - socios backup.sql");

                    DirectoryInfo directory = new DirectoryInfo(preferencias.AutomaticBackupPath);

                    if (preferencias.BackupDeleteDateLimit.HasValue)
                        foreach (FileInfo fileInfo in directory.GetFiles())
                            if (fileInfo.CreationTime < preferencias.BackupDeleteDateLimit.Value
                                && fileInfo.CreationTime < DateTime.Now.Date) // Para no borrar el que acabamos de poner
                                fileInfo.Delete();
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
