using Core;
using Core.DataAccess;
using Core.Util;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.UIEvents;
using Gama.Atenciones.Wpf.ViewModels;
using Gama.Common;
using Gama.Common.Communication;
using Gama.Common.Debug;
using Gama.Common.Eventos;
using MySql.Data.MySqlClient;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media;

namespace Gama.Atenciones.Wpf
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
            PersonaRepository personaRepository,
            CitaRepository citaRepository,
            AtencionRepository atencionRepository,
            AsistenteRepository asistenteRepository,
            PanelSwitcherViewModel panelSwitcherViewModel,
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
            PanelSwitcherViewModel = panelSwitcherViewModel;
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
            _EventAggregator.GetEvent<ActiveViewChanged>().Subscribe(OnActiveViewChangedEvent);
            _EventAggregator.GetEvent<ServidorActualizadoDesdeFueraEvent>().Subscribe(OnServidorActualizadoDesdeFueraEvent);

            _Panels.Add("DashboardView", false);
            _Panels.Add("PersonasContentView", false);
            _Panels.Add("CitasContentView", false);
            _Panels.Add("AsistentesContentView", false);
            _Panels.Add("GraficasContentView", false);

            //SetVisiblePanel("DashboardView");

            _PreloadThread = new Thread(ConectarConServidor);
            _PreloadThread.SetApartmentState(ApartmentState.STA);
            _PreloadThread.Start();
        }

        public PanelSwitcherViewModel PanelSwitcherViewModel { get; set; }
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

        private bool _PersonasContentViewIsVisible = true;
        public bool PersonasContentViewIsVisible
        {
            get { return _PersonasContentViewIsVisible; }
            set { SetProperty(ref _PersonasContentViewIsVisible, value); }
        }

        private bool _CitasContentViewIsVisible = true;
        public bool CitasContentViewIsVisible
        {
            get { return _CitasContentViewIsVisible; }
            set { SetProperty(ref _CitasContentViewIsVisible, value); }
        }

        private bool _AsistentesContentViewIsVisible = true;
        public bool AsistentesContentViewIsVisible
        {
            get { return _AsistentesContentViewIsVisible; }
            set { SetProperty(ref _AsistentesContentViewIsVisible, value); }
        }

        private bool _GraficasViewIsVisible = true;
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

        private PersonaRepository _PersonaRepository;
        private CitaRepository _CitaRepository;
        private AtencionRepository _AtencionRepository;
        private AsistenteRepository _AsistenteRepository;

        private void ConectarConServidor()
        {
            AtencionesResources.ClientService = new ClientService(_EventAggregator, AtencionesResources.ClientId);
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

        private List<Cita> _Citas = new List<Cita>();
        private List<Atencion> _Atenciones = new List<Atencion>();
        private List<Derivacion> _Derivaciones = new List<Derivacion>();
        private List<Asistente> _Asistentes = new List<Asistente>();
        private List<Persona> _Personas = new List<Persona>();

        private void OnServidorActualizadoDesdeFueraEvent(string code)
        {
            string moduleName = code.Substring(code.IndexOf("@MODULO:@") + 9);
            string codigo = code.Substring(0, AtencionesResources.ClientId.Length);

            if (codigo != AtencionesResources.ClientId && moduleName.Contains("ATENCIONES"))
            {
                DoRawThings();

                _AsistenteRepository.UpdateClient();
                _PersonaRepository.UpdateClient();
                _CitaRepository.UpdateClient();
                _AtencionRepository.UpdateClient();

                AsistentesContentViewModel.OnActualizarServidor();
                CitasContentViewModel.OnActualizarServidor();
                DashboardViewModel.OnActualizarServidor();
                GraficasContentViewModel.OnActualizarServidor();
                PersonasContentViewModel.OnActualizarServidor();
                SearchBoxViewModel.OnActualizarServidor();
                ToolbarViewModel.OnActualizarServidor();
            }
        }

        private void DoRawThings()
        {
            _Personas.Clear();
            _Asistentes.Clear();
            _Citas.Clear();
            _Atenciones.Clear();
            _Derivaciones.Clear();

            Persona persona;
            MySqlDataReader reader;
            try
            {
                using (MySqlConnection mysqlConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["GamaAtencionesMySql"].ConnectionString))
                {
                    using (MySqlCommand sqlCommand = new MySqlCommand())
                    {
                        sqlCommand.Connection = mysqlConnection;
                        mysqlConnection.Open();
                        //UIServices.SetBusyState();

                        sqlCommand.CommandText = "SELECT Id, Nombre, Nif, ComoConocioAGama, DireccionPostal, " +
                            "Email, EstadoCivil, FechaDeNacimiento, Facebook, IdentidadSexual, Linkedin, Nacionalidad, " +
                            "NivelAcademico, Ocupacion, OrientacionSexual, Telefono, Twitter, ViaDeAccesoAGama, CreatedAt, UpdatedAt, ImagenUpdatedAt " +
                            "FROM personas ORDER BY Nombre ASC";

                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                persona = new Persona()
                                {
                                    Id = (int)reader["Id"],
                                    Nombre = reader["Nombre"].ToString(),
                                    Nif = reader["Nif"].ToString(),
                                    _SavedNif = reader["Nif"].ToString(),
                                    ComoConocioAGama = reader["ComoConocioAGama"].ToString(),
                                    DireccionPostal = reader["DireccionPostal"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    EstadoCivil = reader["EstadoCivil"].ToString(),
                                    FechaDeNacimiento = reader["FechaDeNacimiento"] as DateTime?,
                                    Facebook = reader["Facebook"].ToString(),
                                    IdentidadSexual = reader["IdentidadSexual"].ToString(),
                                    LinkedIn = reader["LinkedIn"].ToString(),
                                    Nacionalidad = reader["Nacionalidad"].ToString(),
                                    NivelAcademico = reader["NivelAcademico"].ToString(),
                                    Ocupacion = reader["Ocupacion"].ToString(),
                                    OrientacionSexual = reader["OrientacionSexual"].ToString(),
                                    Telefono = reader["Telefono"].ToString(),
                                    Twitter = reader["Twitter"].ToString(),
                                    //Imagen = reader["Imagen"] as byte[],
                                    ViaDeAccesoAGama = reader["ViaDeAccesoAGama"].ToString(),
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    UpdatedAt = reader["UpdatedAt"] as DateTime?,
                                    ImagenUpdatedAt = reader["ImagenUpdatedAt"] as DateTime?,
                                };

                                persona.Decrypt();
                                _Personas.Add(persona);
                            }
                        }

                        foreach (var personaSinImagen in _Personas)
                        {
                            string path = ResourceNames.GetPersonaImagePath(personaSinImagen.Id);
                            if (!File.Exists(path) && personaSinImagen.ImagenUpdatedAt != null)
                            {
                                sqlCommand.CommandText = $"SELECT Imagen FROM personas WHERE Id = {personaSinImagen.Id}";
                                using (reader = sqlCommand.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        personaSinImagen.Imagen = Core.Encryption.Cipher.Decrypt((reader["Imagen"] as byte[]));
                                        using (Image image = Image.FromStream(new MemoryStream(personaSinImagen.Imagen)))
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
                                DateTime updatedTime = (personaSinImagen.ImagenUpdatedAt ?? DateTime.Now.AddYears(-100));
                                if (DateTime.Compare(lastWriteTime, updatedTime) < 0)
                                {
                                    sqlCommand.CommandText = $"SELECT Imagen FROM personas WHERE Id = {personaSinImagen.Id}";
                                    using (reader = sqlCommand.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            personaSinImagen.Imagen = Core.Encryption.Cipher.Decrypt((reader["Imagen"] as byte[]));
                                            using (Image image = Image.FromStream(new MemoryStream(personaSinImagen.Imagen)))
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

                                personaSinImagen.Imagen = ImageToByteArray(new Bitmap(ResourceNames.GetPersonaImagePath(personaSinImagen.Id)));
                            }
                        }

                        //Debug.StopWatch("-----PERSONAS----");
                        //Debug.StartWatch();

                        sqlCommand.CommandText = "SELECT Id, Nombre, Nif, Apellidos, FechaDeNacimiento, ComoConocioAGama, NivelAcademico, " +
                            "Ocupacion, Provincia, Municipio, Localidad, CodigoPostal, Calle, Numero, Portal, Piso, Puerta, " +
                            "TelefonoFijo, TelefonoMovil, TelefonoAlternativo, Email, EmailAlternativo, Linkedin, Twitter, Facebook, Observaciones, ImagenUpdatedAt, " +
                            "CreatedAt, UpdatedAt " +
                            "FROM asistentes";
                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var asistente = new Asistente()
                                {
                                    Id = (int)reader["Id"],
                                    Nombre = reader["Nombre"].ToString(),
                                    Nif = reader["Nif"].ToString(),
                                    Apellidos = reader["Apellidos"].ToString(),
                                    FechaDeNacimiento = reader["FechaDeNacimiento"] as DateTime?,
                                    //Imagen = reader["Imagen"] as byte[],

                                    ComoConocioAGama = reader["ComoConocioAGama"].ToString(),
                                    NivelAcademico = reader["NivelAcademico"].ToString(),
                                    Ocupacion = reader["Ocupacion"].ToString(),

                                    Provincia = reader["Provincia"].ToString(),
                                    Municipio = reader["Municipio"].ToString(),
                                    Localidad = reader["Localidad"].ToString(),
                                    CodigoPostal = reader["CodigoPostal"].ToString(),
                                    Calle = reader["Calle"].ToString(),
                                    Numero = reader["Numero"].ToString(),
                                    Portal = reader["Portal"].ToString(),
                                    Piso = reader["Piso"].ToString(),
                                    Puerta = reader["Puerta"].ToString(),

                                    TelefonoFijo = reader["TelefonoFijo"].ToString(),
                                    TelefonoMovil = reader["TelefonoMovil"].ToString(),
                                    TelefonoAlternativo = reader["TelefonoAlternativo"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    EmailAlternativo = reader["EmailAlternativo"].ToString(),
                                    LinkedIn = reader["LinkedIn"].ToString(),
                                    Twitter = reader["Twitter"].ToString(),
                                    Facebook = reader["Facebook"].ToString(),
                                    Observaciones = reader["Observaciones"].ToString(),
                                    ImagenUpdatedAt = reader["ImagenUpdatedAt"] as DateTime?,
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    UpdatedAt = reader["UpdatedAt"] as DateTime?,
                                };

                                asistente.Decrypt();
                                _Asistentes.Add(asistente);
                            }
                        }

                        AtencionesResources.TodosLosNifDeAsistentes = _Asistentes.Select(x => x.Nif).ToList();

                        foreach (var asistenteSinImagen in _Asistentes)
                        {
                            string path = ResourceNames.GetAsistenteImagePath(asistenteSinImagen.Id);
                            if (!File.Exists(path) && asistenteSinImagen.ImagenUpdatedAt != null)
                            {
                                sqlCommand.CommandText = $"SELECT Imagen FROM asistentes WHERE Id = {asistenteSinImagen.Id}";
                                using (reader = sqlCommand.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        asistenteSinImagen.Imagen = Core.Encryption.Cipher.Decrypt((reader["Imagen"] as byte[]));
                                        using (Image image = Image.FromStream(new MemoryStream(asistenteSinImagen.Imagen)))
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
                                DateTime updatedTime = (asistenteSinImagen.ImagenUpdatedAt ?? DateTime.Now.AddYears(-100));
                                if (DateTime.Compare(lastWriteTime, updatedTime) < 0)
                                {
                                    sqlCommand.CommandText = $"SELECT Imagen FROM personas WHERE Id = {asistenteSinImagen.Id}";
                                    using (reader = sqlCommand.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            asistenteSinImagen.Imagen = Core.Encryption.Cipher.Decrypt((reader["Imagen"] as byte[]));
                                            using (Image image = Image.FromStream(new MemoryStream(asistenteSinImagen.Imagen)))
                                            {
                                                // image.Save(path, ImageFormat.Png);  // Or Png
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

                                asistenteSinImagen.Imagen = ImageToByteArray(new Bitmap(ResourceNames.GetAsistenteImagePath(asistenteSinImagen.Id)));
                            }
                        }
                        //Debug.StopWatch("-----ASISTENTES----");
                        //Debug.StartWatch();

                        sqlCommand.CommandText = "SELECT * FROM citas";
                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var cita = new Cita()
                                {
                                    Id = (int)reader["Id"],
                                    Fecha = (DateTime)reader["Fecha"],
                                    Hora = (int)reader["Hora"],
                                    Minutos = (int)reader["Minutos"],
                                    Sala = reader["Sala"].ToString(),
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    UpdatedAt = reader["UpdatedAt"] as DateTime?,
                                };

                                cita.Decrypt();
                                _Citas.Add(cita);

                                persona = _Personas.Where(p => p.Id == (int)reader["Persona_id"]).Single();
                                var asistente = _Asistentes.Where(a => a.Id == (int)reader["Asistente_id"]).Single();
                                asistente.Citas.Add(cita);
                                cita.Asistente = asistente;
                                persona.AddCita(cita);
                            }
                        }
                        //Debug.StopWatch("-----CITAS----");
                        //Debug.StartWatch();

                        sqlCommand.CommandText = "SELECT * FROM atenciones";
                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var atencion = new Atencion()
                                {
                                    Id = (int)reader["Id"],
                                    Fecha = (DateTime)reader["Fecha"],
                                    Seguimiento = (string)reader["Seguimiento"],
                                    EsSocial = (bool)reader["EsSocial"],
                                    EsJuridica = (bool)reader["EsJuridica"],
                                    EsPsicologica = (bool)reader["EsPsicologica"],
                                    EsDeAcogida = (bool)reader["EsDeAcogida"],
                                    EsDeOrientacionLaboral = (bool)reader["EsDeOrientacionLaboral"],
                                    EsDePrevencionParaLaSalud = (bool)reader["EsDePrevencionParaLaSalud"],
                                    EsDeFormacion = (bool)reader["EsDeFormacion"],
                                    EsDeParticipacion = (bool)reader["EsDeParticipacion"],
                                    EsOtra = (bool)reader["EsOtra"],
                                    Otra = (string)reader["Otra"],
                                };

                                atencion.Decrypt();
                                _Atenciones.Add(atencion);

                                var cita = _Citas.Where(c => c.Id == (int)reader["Cita_id"]).Single();
                                cita.Atencion = atencion;
                                atencion.Cita = cita;
                            }
                        }
                        //Debug.StopWatch("-----ATENCIONES----");
                        //Debug.StartWatch();

                        sqlCommand.CommandText = "SELECT * FROM derivaciones";
                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var derivacion = new Derivacion()
                                {
                                    Id = (int)reader["Id"],
                                    EsSocial = (bool)reader["EsSocial"],
                                    EsJuridica = (bool)reader["EsJuridica"],
                                    EsPsicologica = (bool)reader["EsPsicologica"],
                                    EsDeFormacion = (bool)reader["EsDeFormacion"],
                                    EsDeOrientacionLaboral = (bool)reader["EsDeOrientacionLaboral"],
                                    EsExterna = (bool)reader["EsExterna"],
                                    Externa = (string)reader["Externa"],

                                    EsSocial_Realizada = (bool)reader["EsSocial_Realizada"],
                                    EsJuridica_Realizada = (bool)reader["EsJuridica_Realizada"],
                                    EsPsicologica_Realizada = (bool)reader["EsPsicologica_Realizada"],
                                    EsDeFormacion_Realizada = (bool)reader["EsDeFormacion_Realizada"],
                                    EsDeOrientacionLaboral_Realizada = (bool)reader["EsDeOrientacionLaboral_Realizada"],
                                    EsExterna_Realizada = (bool)reader["EsExterna_Realizada"],
                                    Externa_Realizada = (string)reader["Externa_Realizada"],

                                };

                                _Derivaciones.Add(derivacion);
                                var atencion = _Atenciones.Where(a => a.Id == (int)reader["Atencion_id"]).Single();
                                derivacion.Atencion = atencion;
                                atencion.Derivacion = derivacion;
                            }
                        }
                        //Debug.StopWatch("-----DERIVACIONES----");
                        //Debug.StartWatch();

                        mysqlConnection.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            _PersonaRepository.Personas = _Personas;
            _CitaRepository.Citas = _Citas;
            _AtencionRepository.Atenciones = _Atenciones;
            _AsistenteRepository.Asistentes = _Asistentes;
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
                if (AtencionesResources.ClientService != null)
                    AtencionesResources.ClientService.Desconectar();

                var preferencias = _Preferencias;
                if (preferencias.DoBackupOnClose)
                {
                    string connectionString =
                        ConfigurationManager.ConnectionStrings["GamaAtencionesMySql"].ConnectionString;
                    DBHelper.Backup(
                        connectionString: connectionString,
                        fileName: preferencias.AutomaticBackupPath + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + " - atenciones backup.sql");

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
