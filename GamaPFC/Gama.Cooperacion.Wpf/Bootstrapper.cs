using Prism.Modularity;
using Prism.Unity;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Unity;
using Gama.Common.BaseClasses;
using System.IO;
using Gama.Common;
using Gama.Cooperacion.Wpf.Services;
using System.Runtime.Serialization.Formatters.Binary;
using Core.DataAccess;
using Gama.Cooperacion.DataAccess;
using NHibernate;
using System.Linq;
using Gama.Cooperacion.Business;
using Gama.Common.Debug;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Gama.Cooperacion.Wpf
{
    public class Bootstrapper : UnityBootstrapperBase
    {
        private ISession _Session;
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

        public Bootstrapper(string title = "COOPERACIÓN") : base(title)
        {
            NHibernateSessionFactory._EXECUTE_DDL = false;
            _CLEAR_DATABASE = false;
            _SEED_DATABASE = false;
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/icono_modulo_cooperacion.png"));

            ((ShellViewModel)((FrameworkElement)Shell).DataContext).Title = "COOPERACIÓN";
            ((ShellViewModel)((FrameworkElement)Shell).DataContext).IconSource = icon;

            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow.Show();
            ColeccionEstadosActividades.EstadosActividades = new System.Collections.Generic.Dictionary<string, int>();
            ColeccionEstadosActividades.EstadosActividades.Add("Comenzado", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("NoComenzado", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("FueraPlazo", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("ProximasFinalizaciones", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("Finalizado", 0);

            TerminarPreload();
        }

        protected override void InitializeDirectories()
        {
            if (!Directory.Exists(ResourceNames.AppDataFolder))
                Directory.CreateDirectory(ResourceNames.AppDataFolder);

            if (!Directory.Exists(ResourceNames.PersonasFolder))
                Directory.CreateDirectory(ResourceNames.SociosFolder);

            if (!Directory.Exists(ResourceNames.IconsAndImagesFolder))
                Directory.CreateDirectory(ResourceNames.IconsAndImagesFolder);

            try
            {
                BitmapImage icon;
                BitmapEncoder encoder;

                // Default Search Icon
                if (!File.Exists(ResourceNames.DefaultSearchIconPath))
                {
                    icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/default_search_icon.png"));
                    encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(icon));

                    using (var fileStream =
                        new FileStream(ResourceNames.DefaultSearchIconPath, System.IO.FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }

                // Default User Icon
                if (!File.Exists(ResourceNames.DefaultUserIconPath))
                {
                    icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/default_user_icon.png"));
                    encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(icon));

                    using (var fileStream =
                        new FileStream(ResourceNames.DefaultUserIconPath, System.IO.FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }

                // Atención Icon
                if (!File.Exists(ResourceNames.AtencionIconPath))
                {
                    icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Atenciones.Wpf;component/Resources/Images/atencion_icon.png"));
                    encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(icon));

                    using (var fileStream =
                        new FileStream(ResourceNames.AtencionIconPath, System.IO.FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected override void ConfigurePreferences()
        {
            Preferencias preferencias;

            if (!Directory.Exists(Preferencias.PreferenciasPathFolder))
                Directory.CreateDirectory(Preferencias.PreferenciasPathFolder);

            if (File.Exists(Preferencias.PreferenciasPath))
            {
                var preferenciasFile = File.Open(Preferencias.PreferenciasPath, FileMode.Open);
                preferencias = (Preferencias)new BinaryFormatter().Deserialize(preferenciasFile);
                preferenciasFile.Close();
            }
            else
            {
                preferencias = new Preferencias();
                new BinaryFormatter().Serialize(File.Create(Preferencias.PreferenciasPath), preferencias);
            }

            Container.RegisterInstance(preferencias);
        }

        protected override void RegisterServices()
        {
            Container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactory());
            Container.RegisterType<ISession>(
                new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenSession()));
            Container.RegisterType<IActividadRepository, ActividadRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICooperanteRepository, CooperanteRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IForoRepository, ForoRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ITareaRepository, TareaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IEventoRepository, EventoRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IIncidenciaRepository, IncidenciaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ITareaRepository, TareaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISeguimientoRepository, SeguimientoRepository>(new ContainerControlledLifetimeManager());

            Container.RegisterInstance<Preferencias>(new Preferencias());
        }

        protected override void GenerateDatabaseConfiguration()
        {
            if (_CLEAR_DATABASE)
            {
                var session = Container.Resolve<ISession>();
                var cooperanteRepository = new NHibernateOneSessionRepository<Cooperante, int>();
                var actividadRepository = new NHibernateOneSessionRepository<Actividad, int>();
                cooperanteRepository.Session = session;
                actividadRepository.Session = session;

                actividadRepository.DeleteAll();
                cooperanteRepository.DeleteAll();
            }

            if (_SEED_DATABASE)
            {
                var session = Container.Resolve<ISession>();
                var cooperanteRepository = new NHibernateOneSessionRepository<Cooperante, int>();
                var actividadRepository = new NHibernateOneSessionRepository<Actividad, int>();
                cooperanteRepository.Session = session;
                actividadRepository.Session = session;

                var cooperantesFake = new FakeCooperanteRepository().GetAll().Take(10).ToList();
                var actividadesFake = new FakeActividadRepository().GetAll();

                foreach (var cooperante in cooperantesFake)
                    cooperanteRepository.Create(cooperante);

                var coordinador = cooperanteRepository.GetAll().First();

                for (int i = 0; i < 3; i++)
                {
                    var actividad = actividadesFake[i];
                    var eventosFake = new FakeEventoRepository().GetAll();
                    var foroFake = new FakeForoRepository().GetAll();
                    var mensajeForoFake = new FakeMensajeRepository().GetAll();
                    var tareaFake = new FakeTareaRepository().GetAll();
                    var seguimientoFake = new FakeSeguimientoRepository().GetAll();
                    var incidenciaFake = new FakeIncidenciaRepository().GetAll();
                    actividad.Coordinador = cooperantesFake[i];
                    actividad.AddCooperantes(cooperantesFake.Where(x => x.Id != actividad.Coordinador.Id));
                    actividadRepository.Create(actividad);
                }
            }


            DoThings();

        }

        private void DoThings()
        {
            _Session = Container.Resolve<ISession>();
            _ActividadRepository = Container.Resolve<IActividadRepository>();
            _CooperanteRepository = Container.Resolve<ICooperanteRepository>();
            _EventoRepository = Container.Resolve<IEventoRepository>();
            _ForoRepository = Container.Resolve<IForoRepository>();
            _IncidenciaRepository = Container.Resolve<IIncidenciaRepository>();
            //_MensajeRepository = Container.Resolve<IMensajeRepository>();
            _TareaRepository = Container.Resolve<ITareaRepository>();

            DoRawThings();

            Debug.StopWatch("-----RAW SQL----");
        }

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
                                        cooperanteSinImagen.Foto = reader["Foto"] as byte[];
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
                                            cooperanteSinImagen.Foto = reader["Foto"] as byte[];
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

                                cooperanteSinImagen.Foto = ImageToByteArray(new Bitmap(ResourceNames.GetSocioImagePath(cooperanteSinImagen.Id)));
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

                                _Actividades.Add(actividad);
                            }
                        }

                        //AtencionesResources.TodosLosNifDeAsistentes = _Asistentes.Select(x => x.Nif).ToList();

                        //foreach (var asistenteSinImagen in _Asistentes)
                        //{
                        //    string path = ResourceNames.GetAsistenteImagePath(asistenteSinImagen.Id);
                        //    if (!File.Exists(path) && asistenteSinImagen.ImagenUpdatedAt != null)
                        //    {
                        //        sqlCommand.CommandText = $"SELECT Imagen FROM asistentes WHERE Id = {asistenteSinImagen.Id}";
                        //        using (reader = sqlCommand.ExecuteReader())
                        //        {
                        //            if (reader.Read())
                        //            {
                        //                asistenteSinImagen.Imagen = Core.Encryption.Cipher.Decrypt((reader["Imagen"] as byte[]));
                        //                using (Image image = Image.FromStream(new MemoryStream(asistenteSinImagen.Imagen)))
                        //                {
                        //                    using (MemoryStream memory = new MemoryStream())
                        //                    {
                        //                        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                        //                        {
                        //                            image.Save(memory, ImageFormat.Jpeg);
                        //                            byte[] bytes = memory.ToArray();
                        //                            fs.Write(bytes, 0, bytes.Length);
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        // Si se ha actualizado la imagen de la persona
                        //        DateTime lastWriteTime = File.GetLastWriteTime(path);
                        //        DateTime updatedTime = (asistenteSinImagen.ImagenUpdatedAt ?? DateTime.Now.AddYears(-100));
                        //        if (DateTime.Compare(lastWriteTime, updatedTime) < 0)
                        //        {
                        //            sqlCommand.CommandText = $"SELECT Imagen FROM asistentes WHERE Id = {asistenteSinImagen.Id}";
                        //            using (reader = sqlCommand.ExecuteReader())
                        //            {
                        //                if (reader.Read())
                        //                {
                        //                    asistenteSinImagen.Imagen = Core.Encryption.Cipher.Decrypt((reader["Imagen"] as byte[]));
                        //                    using (Image image = Image.FromStream(new MemoryStream(asistenteSinImagen.Imagen)))
                        //                    {
                        //                        // image.Save(path, ImageFormat.Png);  // Or Png
                        //                        using (MemoryStream memory = new MemoryStream())
                        //                        {
                        //                            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                        //                            {
                        //                                image.Save(memory, ImageFormat.Jpeg);
                        //                                byte[] bytes = memory.ToArray();
                        //                                fs.Write(bytes, 0, bytes.Length);
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }

                        //        asistenteSinImagen.Imagen = ImageToByteArray(new Bitmap(ResourceNames.GetAsistenteImagePath(asistenteSinImagen.Id)));
                        //    }
                        //}
                        ////Debug.StopWatch("-----ASISTENTES----");
                        ////Debug.StartWatch();

                        //sqlCommand.CommandText = "SELECT * FROM citas";
                        //using (reader = sqlCommand.ExecuteReader())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var cita = new Cita()
                        //        {
                        //            Id = (int)reader["Id"],
                        //            Fecha = (DateTime)reader["Fecha"],
                        //            Hora = (int)reader["Hora"],
                        //            Minutos = (int)reader["Minutos"],
                        //            Sala = reader["Sala"].ToString(),
                        //            CreatedAt = (DateTime)reader["CreatedAt"],
                        //            UpdatedAt = reader["UpdatedAt"] as DateTime?,
                        //        };

                        //        cita.Decrypt();
                        //        _Citas.Add(cita);

                        //        persona = _Personas.Where(p => p.Id == (int)reader["Persona_id"]).Single();
                        //        var asistente = _Asistentes.Where(a => a.Id == (int)reader["Asistente_id"]).Single();
                        //        asistente.Citas.Add(cita);
                        //        cita.Asistente = asistente;
                        //        persona.AddCita(cita);
                        //    }
                        //}
                        ////Debug.StopWatch("-----CITAS----");
                        ////Debug.StartWatch();

                        //sqlCommand.CommandText = "SELECT * FROM atenciones";
                        //using (reader = sqlCommand.ExecuteReader())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var atencion = new Atencion()
                        //        {
                        //            Id = (int)reader["Id"],
                        //            Fecha = (DateTime)reader["Fecha"],
                        //            Seguimiento = (string)reader["Seguimiento"],
                        //            EsSocial = (bool)reader["EsSocial"],
                        //            EsJuridica = (bool)reader["EsJuridica"],
                        //            EsPsicologica = (bool)reader["EsPsicologica"],
                        //            EsDeAcogida = (bool)reader["EsDeAcogida"],
                        //            EsDeOrientacionLaboral = (bool)reader["EsDeOrientacionLaboral"],
                        //            EsDePrevencionParaLaSalud = (bool)reader["EsDePrevencionParaLaSalud"],
                        //            EsDeFormacion = (bool)reader["EsDeFormacion"],
                        //            EsDeParticipacion = (bool)reader["EsDeParticipacion"],
                        //            EsOtra = (bool)reader["EsOtra"],
                        //            Otra = (string)reader["Otra"],
                        //        };

                        //        atencion.Decrypt();
                        //        _Atenciones.Add(atencion);

                        //        var cita = _Citas.Where(c => c.Id == (int)reader["Cita_id"]).Single();
                        //        cita.Atencion = atencion;
                        //        atencion.Cita = cita;
                        //    }
                        //}
                        ////Debug.StopWatch("-----ATENCIONES----");
                        ////Debug.StartWatch();

                        //sqlCommand.CommandText = "SELECT * FROM derivaciones";
                        //using (reader = sqlCommand.ExecuteReader())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var derivacion = new Derivacion()
                        //        {
                        //            Id = (int)reader["Id"],
                        //            EsSocial = (bool)reader["EsSocial"],
                        //            EsJuridica = (bool)reader["EsJuridica"],
                        //            EsPsicologica = (bool)reader["EsPsicologica"],
                        //            EsDeFormacion = (bool)reader["EsDeFormacion"],
                        //            EsDeOrientacionLaboral = (bool)reader["EsDeOrientacionLaboral"],
                        //            EsExterna = (bool)reader["EsExterna"],
                        //            Externa = (string)reader["Externa"],

                        //            EsSocial_Realizada = (bool)reader["EsSocial_Realizada"],
                        //            EsJuridica_Realizada = (bool)reader["EsJuridica_Realizada"],
                        //            EsPsicologica_Realizada = (bool)reader["EsPsicologica_Realizada"],
                        //            EsDeFormacion_Realizada = (bool)reader["EsDeFormacion_Realizada"],
                        //            EsDeOrientacionLaboral_Realizada = (bool)reader["EsDeOrientacionLaboral_Realizada"],
                        //            EsExterna_Realizada = (bool)reader["EsExterna_Realizada"],
                        //            Externa_Realizada = (string)reader["Externa_Realizada"],

                        //        };

                        //        _Derivaciones.Add(derivacion);
                        //        var atencion = _Atenciones.Where(a => a.Id == (int)reader["Atencion_id"]).Single();
                        //        derivacion.Atencion = atencion;
                        //        atencion.Derivacion = derivacion;
                        //    }
                        //}
                        ////Debug.StopWatch("-----DERIVACIONES----");
                        ////Debug.StartWatch();

                        mysqlConnection.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            //_PersonaRepository.Personas = _Personas;
            //_CitaRepository.Citas = _Citas;
            //_AtencionRepository.Atenciones = _Atenciones;
            //_AsistenteRepository.Asistentes = _Asistentes;
        }


        protected override void ConfigureModuleCatalog()
        {
            Type atencionesModuleType = typeof(CooperacionModule);
            ModuleCatalog.AddModule(new ModuleInfo()
            {
                ModuleName = atencionesModuleType.Name,
                ModuleType = atencionesModuleType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
        }
    }

    public class CooperanteParticipaEnActividad
    {
        public int Actividad_id { get; set; }
        public int Cooperante_id { get; set; }

        public CooperanteParticipaEnActividad(int actividad_id, int cooperante_id)
        {
            Actividad_id = actividad_id;
            Cooperante_id = cooperante_id;
        }
    }
}
