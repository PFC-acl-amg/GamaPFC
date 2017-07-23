using Core;
using Gama.Common;
using Gama.Common.Communication;
using Gama.Common.Debug;
using Gama.Common.Eventos;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.ViewModels;
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
            IActividadRepository actividadRepository,
            ICooperanteRepository cooperanteRepository,
            ITareaRepository tareaRepository,
            IMensajeRepository mensajeRepository,
            IIncidenciaRepository incidenciaRepository,
            IForoRepository foroRepository,
            IEventoRepository eventoRepository,
            EventAggregator eventAggregator,
            DashboardViewModel dashboardViewModel,
            ToolbarViewModel toolbarViewModel,
            ActividadesContentViewModel actividadesContentVieWModel,
            PanelSwitcherViewModel panelSwitcherViewModel,
            CalendarioDeActividadesViewModel calendarioDeActividadesViewModel,
            CooperantesContentViewModel cooperantesContentViewModel,
            StatusBarViewModel statusBarViewModel,
            RightCommandsViewModel rightCommandsViewModel,
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
            RightCommandsViewModel = rightCommandsViewModel;

            _ActividadRepository = actividadRepository; _ActividadRepository.Session = session;
            _CooperanteRepository = cooperanteRepository; _CooperanteRepository.Session = session;
            _TareaRepository = tareaRepository; _TareaRepository.Session = session;
            _IncidenciaRepository = incidenciaRepository; _IncidenciaRepository.Session = session;
            _ForoRepository = foroRepository; _ForoRepository.Session = session;
            _EventoRepository = eventoRepository; _EventoRepository.Session = session;
            _MensajeRepository = mensajeRepository; _MensajeRepository.Session = session;

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
        public RightCommandsViewModel RightCommandsViewModel { get; set; }

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
            string moduleName = code.Substring(code.IndexOf("@MODULO:@") + 9);
            string codigo = code.Substring(0, CooperacionResources.ClientId.Length);

            if (codigo != CooperacionResources.ClientId && moduleName.Contains("COOPERACION"))
            {
                DoRawThings();

                _CooperanteRepository.UpdateClient();

                DashboardViewModel.OnActualizarServidor();
                PanelSwitcherViewModel.OnActualizarServidor();
                ToolbarViewModel.OnActualizarServidor();
                CalendarioDeActividadesViewModel.OnActualizarServidor();
                CooperantesContentViewModel.OnActualizarServidor();
            }
        }


        //public PanelSwitcherViewModel PanelSwitcherViewModel { get; set; }
        //public ToolbarViewModel ToolbarViewModel { get; set; }
        //public StatusBarViewModel StatusBarViewModel { get; set; }
        //public CalendarioDeActividadesViewModel CalendarioDeActividadesViewModel { get; set; }
        //public CooperantesContentViewModel CooperantesContentViewModel { get; set; }
        //public DashboardViewModel DashboardViewModel { get; set; }
        //public ActividadesContentViewModel ActividadesContentViewModel { get; set; }
        //public RightCommandsViewModel RightCommandsViewModel { get; set; }

        private IActividadRepository _ActividadRepository;
        private ICooperanteRepository _CooperanteRepository;
        private IEventoRepository _EventoRepository;
        private IForoRepository _ForoRepository;
        private IIncidenciaRepository _IncidenciaRepository;
        private IMensajeRepository _MensajeRepository;
        private ITareaRepository _TareaRepository;
        private List<Cooperante> _Cooperantes = new List<Cooperante>();
        private List<Actividad> _Actividades = new List<Actividad>();
        private List<Evento> _Eventos = new List<Evento>();
        private List<Foro> _Foros = new List<Foro>();
        private List<Incidencia> _Incidencias = new List<Incidencia>();
        private List<Mensaje> _Mensajes = new List<Mensaje>();
        private List<Seguimiento> _Seguimientos = new List<Seguimiento>();
        private List<Tarea> _Tareas = new List<Tarea>();

        private void DoRawThings()
        {
            Debug.StartWatch();
            Actividad actividad;
            MySqlDataReader reader;
            try
            {
                using (MySqlConnection mysqlConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["GamaCooperacionMySql"].ConnectionString))
                {
                    using (MySqlCommand sqlCommand = new MySqlCommand())
                    {
                        sqlCommand.Connection = mysqlConnection;
                        mysqlConnection.Open();
                        //UIServices.SetBusyState();

                        sqlCommand.CommandText = "SELECT Id, Apellido, Dni, Nombre, Observaciones, " +
                            "Telefono, FotoUpdatedAt, FechaDeNacimiento, Provincia, Municipio, CP, " +
                            "Localidad, Calle, Numero, Portal, Piso, Puerta, TelefonoMovil, TelefonoAlternativo, " +
                            "Email, EmailAlternativo, CreatedAt, UpdatedAt, FotoUpdatedAt " +
                            "FROM cooperantes";

                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var cooperante = new Cooperante()
                                {
                                    Id = (int)reader["Id"],
                                    Nombre = reader["Nombre"].ToString(),
                                    Dni = reader["Dni"].ToString(),
                                    Provincia = reader["Provincia"].ToString(),
                                    Municipio = reader["Municipio"].ToString(),
                                    Localidad = reader["Localidad"].ToString(),
                                    CP = reader["CP"].ToString(),
                                    Calle = reader["Calle"].ToString(),
                                    Numero = reader["Numero"].ToString(),
                                    Portal = reader["Portal"].ToString(),
                                    Piso = reader["Piso"].ToString(),
                                    Puerta = reader["Puerta"].ToString(),
                                    Telefono = reader["Telefono"].ToString(),
                                    TelefonoMovil = reader["TelefonoMovil"].ToString(),
                                    TelefonoAlternativo = reader["TelefonoAlternativo"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    EmailAlternativo = reader["EmailAlternativo"].ToString(),
                                    FechaDeNacimiento = reader["FechaDeNacimiento"] as DateTime?,
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    UpdatedAt = reader["UpdatedAt"] as DateTime?,
                                    FotoUpdatedAt = reader["FotoUpdatedAt"] as DateTime?,
                                    Apellido = reader["Apellido"].ToString(),
                                    Observaciones = reader["Observaciones"].ToString(),
                                };

                                cooperante.Decrypt();
                                _Cooperantes.Add(cooperante);
                            }
                        }

                        CooperacionResources.TodosLosNifDeCooperantes = _Cooperantes.Select(x => x.Dni).ToList();

                        foreach (var cooperanteSinImagen in _Cooperantes)
                        {
                            string path = ResourceNames.GetCooperanteImagePath(cooperanteSinImagen.Id);
                            if (!File.Exists(path) && cooperanteSinImagen.FotoUpdatedAt != null)
                            {
                                sqlCommand.CommandText = $"SELECT Foto FROM cooperantes WHERE Id = {cooperanteSinImagen.Id}";
                                using (reader = sqlCommand.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        cooperanteSinImagen.Foto = Core.Encryption.Cipher.Decrypt((reader["Foto"] as byte[]));
                                        if (cooperanteSinImagen.Foto != null)
                                        {
                                            using (Image image = Image.FromStream(new MemoryStream(cooperanteSinImagen.Foto)))
                                            {
                                                using (MemoryStream memory = new MemoryStream())
                                                {
                                                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                                                    {
                                                        image.Save(memory, ImageFormat.Png);
                                                        byte[] bytes = memory.ToArray();
                                                        fs.Write(bytes, 0, bytes.Length);
                                                    }
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
                                DateTime updatedTime = (cooperanteSinImagen.FotoUpdatedAt ?? DateTime.Now.AddYears(-100));
                                if (DateTime.Compare(lastWriteTime, updatedTime) < 0)
                                {
                                    sqlCommand.CommandText = $"SELECT Foto FROM cooperantes WHERE Id = {cooperanteSinImagen.Id}";
                                    using (reader = sqlCommand.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            cooperanteSinImagen.Foto = Core.Encryption.Cipher.Decrypt((reader["Foto"] as byte[]));
                                            if (cooperanteSinImagen.Foto != null)
                                            {
                                                using (Image image = Image.FromStream(new MemoryStream(cooperanteSinImagen.Foto)))
                                                {
                                                    using (MemoryStream memory = new MemoryStream())
                                                    {
                                                        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                                                        {
                                                            image.Save(memory, ImageFormat.Png);
                                                            byte[] bytes = memory.ToArray();
                                                            fs.Write(bytes, 0, bytes.Length);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                path = ResourceNames.GetCooperanteImagePath(cooperanteSinImagen.Id);
                                if (File.Exists(path))
                                    cooperanteSinImagen.Foto = ImageToByteArray(new Bitmap(ResourceNames.GetCooperanteImagePath(cooperanteSinImagen.Id)));
                            }
                        }

                        //Debug.StopWatch("-----PERSONAS----");
                        //Debug.StartWatch();

                        sqlCommand.CommandText = "SELECT Actividad_id, Cooperante_id FROM cooperanteparticipaenactividad";
                        List<CooperanteParticipaEnActividad> cooperanteParticipaEnActividad = new List<CooperanteParticipaEnActividad>();
                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cooperanteParticipaEnActividad.Add(
                                    new CooperanteParticipaEnActividad((int)reader["Actividad_id"], (int)reader["Cooperante_id"]));
                            }
                        }

                        sqlCommand.CommandText = "SELECT Id, Descripcion, Estado, FechaDeInicio, FechaDeFin, " +
                            "Titulo, CreatedAt, UpdatedAt, Coordinador_id FROM actividades;";

                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                actividad = new Actividad()
                                {
                                    Id = (int)reader["Id"],
                                    Descripcion = reader["Descripcion"].ToString(),
                                    FechaDeInicio = (DateTime)reader["FechaDeFin"],
                                    FechaDeFin = (DateTime)reader["FechaDeFin"],
                                    Titulo = (string)reader["Titulo"],
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    UpdatedAt = reader["UpdatedAt"] as DateTime?,
                                };

                                actividad.Decrypt();

                                actividad.Coordinador = _Cooperantes.Find(x => x.Id == (int)reader["Coordinador_id"]);
                                actividad.CoordinadorId = actividad.Coordinador.Id;
                                actividad.Cooperantes = _Cooperantes.Where(
                                        cooperante => cooperanteParticipaEnActividad.Where(
                                            cpea => cpea.Actividad_id == actividad.Id && cpea.Cooperante_id == cooperante.Id)
                                            .ToList().Count > 0).ToList();
                                actividad.Coordinador.ActividadesDeQueEsCoordinador.Add(actividad);

                                foreach (var cooperante in actividad.Cooperantes)
                                {
                                    cooperante.ActividadesEnQueParticipa.Add(actividad);
                                }

                                _Actividades.Add(actividad);
                            }
                        }

                        sqlCommand.CommandText = "SELECT Id, Titulo, FechaDePublicacion, Ocurrencia, " +
                            "CreatedAt, UpdatedAt, Actividad_id FROM eventos;";

                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var evento = new Evento()
                                {
                                    Id = (int)reader["Id"],
                                    Ocurrencia = (string)reader["Ocurrencia"],
                                    FechaDePublicacion = (DateTime)reader["FechaDePublicacion"],
                                    Titulo = (string)reader["Titulo"],
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    UpdatedAt = reader["UpdatedAt"] as DateTime?,
                                };

                                evento.Decrypt();
                                actividad = _Actividades.Find(x => x.Id == (int)reader["Actividad_id"]);
                                evento.Actividad = actividad;
                                actividad.Eventos.Add(evento);

                                _Eventos.Add(evento);
                            }
                        }

                        sqlCommand.CommandText = "SELECT Id, Titulo, FechaDePublicacion, " +
                            "CreatedAt, UpdatedAt, Actividad_id FROM foros;";

                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var foro = new Foro()
                                {
                                    Id = (int)reader["Id"],
                                    FechaDePublicacion = (DateTime)reader["FechaDePublicacion"],
                                    Titulo = (string)reader["Titulo"],
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    UpdatedAt = reader["UpdatedAt"] as DateTime?,
                                };

                                foro.Decrypt();
                                actividad = _Actividades.Find(x => x.Id == (int)reader["Actividad_id"]);
                                foro.Actividad = actividad;
                                actividad.Foros.Add(foro);

                                _Foros.Add(foro);
                            }
                        }

                        sqlCommand.CommandText = "SELECT Id, Descripcion, FechaDeFinalizacion, HaFinalizado, " +
                            "CreatedAt, UpdatedAt, Actividad_id, Responsable_id FROM tareas;";

                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var tarea = new Tarea()
                                {
                                    Id = (int)reader["Id"],
                                    FechaDeFinalizacion = reader["FechaDeFinalizacion"] as DateTime?,
                                    Descripcion = (string)reader["Descripcion"],
                                    HaFinalizado = (bool)reader["HaFinalizado"],
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    UpdatedAt = reader["UpdatedAt"] as DateTime?,
                                };

                                tarea.Decrypt();
                                actividad = _Actividades.Find(x => x.Id == (int)reader["Actividad_id"]);
                                var responsable = _Cooperantes.Find(x => x.Id == (int)reader["Responsable_id"]);
                                tarea.Actividad = actividad;
                                tarea.Responsable = responsable;
                                actividad.Tareas.Add(tarea);

                                _Tareas.Add(tarea);
                            }
                        }

                        sqlCommand.CommandText = "SELECT Id, Descripcion, FechaDePublicacion, " +
                            "CreatedAt, UpdatedAt, Tarea_id FROM seguimientos;";

                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var seguimiento = new Seguimiento()
                                {
                                    Id = (int)reader["Id"],
                                    FechaDePublicacion = (DateTime)reader["FechaDePublicacion"],
                                    Descripcion = (string)reader["Descripcion"],
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    UpdatedAt = reader["UpdatedAt"] as DateTime?,
                                };

                                var tarea = _Tareas.Find(x => x.Id == (int)reader["Tarea_id"]);
                                tarea.AddSeguimiento(seguimiento);

                                _Seguimientos.Add(seguimiento);
                            }
                        }

                        sqlCommand.CommandText = "SELECT Id, Descripcion, FechaDePublicacion, Solucionada, " +
                            "CreatedAt, UpdatedAt, Tarea_id FROM incidenciastarea;";

                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var incidencia = new Incidencia()
                                {
                                    Id = (int)reader["Id"],
                                    FechaDePublicacion = (DateTime)reader["FechaDePublicacion"],
                                    Descripcion = (string)reader["Descripcion"],
                                    Solucionada = (int)reader["Solucionada"],
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    UpdatedAt = reader["UpdatedAt"] as DateTime?,
                                };

                                var tarea = _Tareas.Find(x => x.Id == (int)reader["Tarea_id"]);
                                tarea.AddIncidencia(incidencia);

                                _Incidencias.Add(incidencia);
                            }
                        }

                        sqlCommand.CommandText = "SELECT Id, Titulo, FechaDePublicacion, " +
                            "CreatedAt, UpdatedAt, Foro_id FROM mensajes;";

                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var mensaje = new Mensaje()
                                {
                                    Id = (int)reader["Id"],
                                    FechaDePublicacion = (DateTime)reader["FechaDePublicacion"],
                                    Titulo = (string)reader["Titulo"],
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    UpdatedAt = reader["UpdatedAt"] as DateTime?,
                                };

                                var foro = _Foros.Find(x => x.Id == (int)reader["Foro_id"]);
                                foro.AddMensaje(mensaje);

                                _Mensajes.Add(mensaje);
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

            _ActividadRepository.Actividades = _Actividades;
            _CooperanteRepository.Cooperantes = _Cooperantes;
            _EventoRepository.Eventos = _Eventos;
            _ForoRepository.Foros = _Foros;
            _TareaRepository.Tareas = _Tareas;
            _IncidenciaRepository.Incidencias = _Incidencias;
            _MensajeRepository.Mensajes = _Mensajes;
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
            //throw new NotImplementedException();
        }

        private void OnActiveViewChangedEvent(string viewName)
        {
            SetVisiblePanel(viewName);
        }
    }
}
