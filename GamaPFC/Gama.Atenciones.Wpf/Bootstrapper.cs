using Prism.Modularity;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Unity;
using Core.DataAccess;
using NHibernate;
using Gama.Atenciones.Wpf.Services;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Gama.Atenciones.DataAccess;
using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Business;
using Gama.Common;
using Gama.Common.BaseClasses;
using System.Linq;
using System.ComponentModel;
using Gama.Common.Debug;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Configuration;
using Gama.Atenciones.Wpf.Converters;
using System.Drawing;
using System.Drawing.Imaging;
using Gama.Atenciones.Wpf.Controls;

namespace Gama.Atenciones.Wpf
{
    public class Bootstrapper : UnityBootstrapperBase
    {
        private List<Cita> _Citas = new List<Cita>();
        private List<Atencion> _Atenciones = new List<Atencion>();
        private List<Derivacion> _Derivaciones = new List<Derivacion>();
        private List<Asistente> _Asistentes = new List<Asistente>();
        private List<Persona> _Personas = new List<Persona>();
        private IAsistenteRepository _AsistenteRepository;
        private IPersonaRepository _PersonaRepository;
        private ICitaRepository _CitaRepository;
        private IAtencionRepository _AtencionRepository;
        private ISession _Session;

        public Bootstrapper(string title = "SERVICIO DE ATENCIONES") : base(title)
        {
            NHibernateSessionFactory._EXECUTE_DDL = false;
            _CLEAR_DATABASE = false;
            _SEED_DATABASE = true;
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            ((ShellViewModel)((FrameworkElement)Shell).DataContext).Title = "SERVICIO DE ATENCIONES";
            ((ShellViewModel)((FrameworkElement)Shell).DataContext).IconSource =
                new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/icono_modulo_atenciones.png"));

            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow.ShowActivated = true;
            Application.Current.MainWindow.Show();

            TerminarPreload();
        }

        protected override void InitializeDirectories()
        {
            if (!Directory.Exists(ResourceNames.AppDataFolder))
                Directory.CreateDirectory(ResourceNames.AppDataFolder);

            if (!Directory.Exists(ResourceNames.PersonasFolder))
                Directory.CreateDirectory(ResourceNames.PersonasFolder);

            if (!Directory.Exists(ResourceNames.AsistentesFolder))
                Directory.CreateDirectory(ResourceNames.AsistentesFolder);

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
            Container.RegisterType<IPersonaRepository, PersonaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICitaRepository, CitaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAtencionRepository, AtencionRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAsistenteRepository, AsistenteRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CustomCalendar>();
        }

        protected override void GenerateDatabaseConfiguration()
        {
            var sessionFactory = Container.Resolve<INHibernateSessionFactory>();
            var session = sessionFactory.OpenSession();

            #region Seeding
            if (_CLEAR_DATABASE || _SEED_DATABASE)
            {

                // NOTA: No utilizamos los servicios directamente porque añaden código que afecta al resto de la aplicación
                //, a través del EventAggregator por ejemplo. Sólo requerimos la funcionalidad de base de datos.
                var personaRepository = new NHibernateOneSessionRepository<Persona, int>();// Container.Resolve<IPersonaRepository>();
                var citaRepository = new NHibernateOneSessionRepository<Cita, int>();// Container.Resolve<ICitaRepository>();
                var asistenteRepository = new NHibernateOneSessionRepository<Asistente, int>();// Container.Resolve<IAsistenteRepository>();
                var atencionRepository = new NHibernateOneSessionRepository<Atencion, int>();// Container.Resolve<IAtencionRepository>();
                var derivacionRepository = new NHibernateOneSessionRepository<Derivacion, int>();

                personaRepository.Session = session;
                citaRepository.Session = session;
                asistenteRepository.Session = session;
                atencionRepository.Session = session;
                derivacionRepository.Session = session;

                try
                {
                    if (_CLEAR_DATABASE)
                        personaRepository.DeleteAll();
                        
                    if (_SEED_DATABASE)
                    {
                        var personas = new FakePersonaRepository().GetAll(); //personaRepository.GetAll();
                        var citas = new FakeCitaRepository().GetAll();
                        var atenciones = new FakeAtencionRepository().GetAll();
                        var asistentes = new FakeAsistenteRepository().Asistentes;

                        //personas.ForEach(p => p.Id = 0);
                        //citas.ForEach(c => c.Id = 0);
                        //atenciones.ForEach(a => a.Id = 0);

                        var random = new Random();

                        int i = 0;
                        foreach (var asistente in asistentes)
                        {
                            asistenteRepository.Create(asistente);
                            Console.Write($"Asistente número {++i};");
                        }

                        i = 0;
                        foreach (var persona in personas)
                        {
                            personaRepository.Create(persona);
                            Console.Write($"Persona número {++i};");
                        }

                        i = 0;
                        foreach (var cita in citas)
                        {
                            var persona = personas[random.Next(0, personas.Count - 1)];
                            persona.AddCita(cita);
                            cita.Asistente = asistentes[random.Next(0, asistentes.Count - 1)];
                            citaRepository.Create(cita);
                            Console.Write($"Cita número {++i};");
                        }

                        i = 0;
                        foreach (var atencion in atenciones)
                        {
                            atencion.Cita = citas[i++];
                            atencion.Derivacion = FakeDerivacionRepository.Next(atencion);

                            atencionRepository.Create(atencion);
                            Console.Write($"Atención número {i};");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            #endregion

            DoThings();
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
        }

        private void DoThings()
        {
            _Session = Container.Resolve<ISession>();
            _PersonaRepository = Container.Resolve<IPersonaRepository>();
            _CitaRepository = Container.Resolve<ICitaRepository>();
            _AtencionRepository = Container.Resolve<IAtencionRepository>();
            _AsistenteRepository = Container.Resolve<IAsistenteRepository>();

            DoRawThings();

            Debug.StopWatch("-----RAW SQL----");
        }

        private void DoRawThings()
        {
            Debug.StartWatch();
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

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ONDo();
        }

        private void ONDo()
        {
            //Debug.StartStopWatch();
            //var asistentes = asistenteRepository.Asistentes;
            //Debug.StopWatch("Asistentes");
        }

        protected override void ConfigureModuleCatalog()
        {
            Type atencionesModuleType = typeof(AtencionesModule);
            ModuleCatalog.AddModule(new ModuleInfo()
            {
                ModuleName = atencionesModuleType.Name,
                ModuleType = atencionesModuleType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });
        }
    }
}
