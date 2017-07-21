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

            ColeccionEstadosActividades.EstadosActividades = new System.Collections.Generic.Dictionary<string, int>();
            ColeccionEstadosActividades.EstadosActividades.Add("Comenzado", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("NoComenzado", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("FueraPlazo", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("ProximasFinalizaciones", 0);
            ColeccionEstadosActividades.EstadosActividades.Add("Finalizado", 0);

            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow.Show();

            TerminarPreload();
        }

        protected override void InitializeDirectories()
        {
            if (!Directory.Exists(ResourceNames.AppDataFolder))
                Directory.CreateDirectory(ResourceNames.AppDataFolder);

            if (!Directory.Exists(ResourceNames.CooperanteFolder))
                Directory.CreateDirectory(ResourceNames.CooperanteFolder);

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
            Container.RegisterType<IMensajeRepository, MensajeRepository>(new ContainerControlledLifetimeManager());

            Container.RegisterInstance<Preferencias>(new Preferencias());
        }

        protected override void GenerateDatabaseConfiguration()
        {
            if (_CLEAR_DATABASE)
            {
                var session = Container.Resolve<ISession>();
                var actividadRepository = Container.Resolve<IActividadRepository>();
                actividadRepository.Session = session;

                actividadRepository.DeleteAll();
            }

            if (_SEED_DATABASE)
            {
                var session = Container.Resolve<ISession>();
                var cooperanteRepository = new NHibernateOneSessionRepository<Cooperante, int>();
                var actividadRepository = new NHibernateOneSessionRepository<Actividad, int>();
                cooperanteRepository.Session = session;
                actividadRepository.Session = session;

                string[] Nombres = new string[] { "Jose", "Antonio", "Alberto", "Paco","Luis"};
                string[] Apellidos = new string[] { "Martin", "Garcia", "Cardona", "Carreras" };
                string[] DNIS = new string[] { "78200200-A", "78100100-B", "78300300-C", "78400400-D" };
                string[] TA = new string[] { "Actividad 1", "Actividad 2", "Actividad 3", "Actividad 4" };
                string[] DA = new string[] { "Descripcion 1", "Descripcion 2", "Descripcion 3", "Descripcion 4" };
                string[] TF = new string[] { "Foro 1", "Foro 2", "Foro 3", "Foro 4" };
                string[] TT = new string[] { "Tarea 1", "Tarea 2", "Tarea 3", "Tarea 4" };
                string[] MF = new string[] { "MensajeForo 1", "MensajeForo 2", "MensajeForo 3", "MensajeForo 4" };
                string[] MI = new string[] { "MensajeIncidencia 1", "MensajeIncidencia 2", "MensajeIncidencia 3", "MensajeIncidencia 4" };
                string[] MS = new string[] { "MensajeSeguimiento 1", "MensajeSeguimiento 2", "MensajeSeguimiento 3", "MensajeSeguimiento 4" };

                
                var coop = new Cooperante();
                for (int i = 0; i < 4; i++)
                {
                    coop.Nombre = Nombres[i];
                    coop.Apellido = Apellidos[i];
                    coop.Dni = DNIS[i];
                    cooperanteRepository.Create(coop);
                }
                var cooperantes = cooperanteRepository.GetAll();
                int k = 3;
                for (int i = 0; i < 4; i++)
                {
                    var act = new Actividad();
                    var tarea = new Tarea();
                    var foro = new Foro();
                    var mf = new Mensaje();
                    var mi = new Incidencia();
                    var ms = new Seguimiento();
                    tarea.Descripcion= TT[i];
                    tarea.Responsable = cooperantes[i];
                    tarea.FechaDeFinalizacion = new DateTime(2017, 07, 21);
                    foro.Titulo = TF[i];
                    mf.Titulo = MF[i];
                    mi.Descripcion = MI[i];
                    ms.Descripcion = MS[i];
                    foro.AddMensaje(mf);
                    tarea.AddIncidencia(mi);
                    tarea.AddSeguimiento(ms);
                    act.Titulo = TA[i];
                    act.Descripcion = DA[i];
                    act.FechaDeInicio = new DateTime(2017, 07, 21);
                    act.FechaDeFin = new DateTime(2017, 07, 21).AddDays(7);
                    act.AddForo(foro);
                    act.AddTarea(tarea);
                    act.Coordinador = cooperantes[i];
                    act.Cooperantes.Add(cooperantes[k]);
                    k--;
                    actividadRepository.Create(act);
                    act = null;
                    tarea = null;
                    foro = null;
                    mf = null;
                    mi = null;
                    ms = null;
                }
                
                

                //var cooperantesFake = new FakeCooperanteRepository().GetAll().Take(30).ToList();
                //var actividadesFake = new FakeActividadRepository().GetAll();

                //foreach (var cooperante in cooperantesFake)
                //    cooperanteRepository.Create(cooperante);

                //var coordinador = cooperanteRepository.GetAll().First();

                //for (int i = 0; i < 30; i++)
                //{
                //    var actividad = actividadesFake[i];
                //    var eventosFake = new FakeEventoRepository().GetAll();
                //    var foroFake = new FakeForoRepository().GetAll();
                //    var mensajeForoFake = new FakeMensajeRepository().GetAll();
                //    var tareaFake = new FakeTareaRepository().GetAll();
                //    var seguimientoFake = new FakeSeguimientoRepository().GetAll();
                //    var incidenciaFake = new FakeIncidenciaRepository().GetAll();

                //    actividad.Coordinador = cooperantesFake[i];
                //    actividad.AddCooperantes(cooperantesFake.Where(x => x.Id != actividad.Coordinador.Id));

                //    tareaFake.ForEach(x => actividad.AddTarea(x));
                //    foroFake.ForEach(x => actividad.AddForo(x));
                //    eventosFake.ForEach(x => actividad.AddEvento(x));
                //    foroFake.ForEach(x => x.AddMensajes(mensajeForoFake));
                //    tareaFake.ForEach(x => x.AddIncidencias(incidenciaFake));
                //    tareaFake.ForEach(x => x.AddSeguimientos(seguimientoFake));
                //    tareaFake.ForEach(x => x.Responsable = cooperantesFake[0]);

                //    actividadRepository.Create(actividad);
                //}
            }

            SetMaxPacketSize();

            DoThings();
        }

        private void SetMaxPacketSize()
        {
            using (MySqlConnection mysqlConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["GamaCooperacionMySql"].ToString()))
            {
                using (MySqlCommand sqlCommand = new MySqlCommand())
                {
                    sqlCommand.Connection = mysqlConnection;
                    sqlCommand.CommandText = "SET GLOBAL max_allowed_packet = 1677721656;";
                    mysqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        private void DoThings()
        {
            _Session = Container.Resolve<ISession>();
            _ActividadRepository = Container.Resolve<IActividadRepository>();
            _CooperanteRepository = Container.Resolve<ICooperanteRepository>();
            _EventoRepository = Container.Resolve<IEventoRepository>();
            _ForoRepository = Container.Resolve<IForoRepository>();
            _IncidenciaRepository = Container.Resolve<IIncidenciaRepository>();
            _MensajeRepository = Container.Resolve<IMensajeRepository>();
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
