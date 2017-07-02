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

namespace Gama.Atenciones.Wpf
{
    public class Bootstrapper : UnityBootstrapperBase
    {
        public Bootstrapper(string title = "SERVICIO DE ATENCIONES") : base(title)
        {
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
        }

        protected override void GenerateDatabaseConfiguration()
        {
            #region Seeding
            if (_CLEAR_DATABASE || _SEED_DATABASE)
            {
                var sessionFactory = Container.Resolve<INHibernateSessionFactory>();

                // NOTA: No utilizamos los servicios directamente porque añaden código que afecta al resto de la aplicación
                //, a través del EventAggregator por ejemplo. Sólo requerimos la funcionalidad de base de datos.
                var personaRepository = new NHibernateOneSessionRepository<Persona, int>();// Container.Resolve<IPersonaRepository>();
                var citaRepository = new NHibernateOneSessionRepository<Cita, int>();// Container.Resolve<ICitaRepository>();
                var asistenteRepository = new NHibernateOneSessionRepository<Asistente, int>();// Container.Resolve<IAsistenteRepository>();
                var atencionRepository = new NHibernateOneSessionRepository<Atencion, int>();// Container.Resolve<IAtencionRepository>();
                var derivacionRepository = new NHibernateOneSessionRepository<Derivacion, int>();

                var session = sessionFactory.OpenSession();

                personaRepository.Session = session;
                citaRepository.Session = session;
                asistenteRepository.Session = session;
                atencionRepository.Session = session;
                derivacionRepository.Session = session;

                /// INICIALIZACIÓN 
                /// En este caso sí necesitamos crear el servicio tal cual porque accedemos a una función espsecífica
                var asisRep = Container.Resolve<IAsistenteRepository>();
                asisRep.Session = session;
                AtencionesResources.TodosLosNifDeAsistentes = asisRep.GetNifs();

                if (_CLEAR_DATABASE)
                {
                    citaRepository.DeleteAll();
                    asistenteRepository.DeleteAll();
                    personaRepository.DeleteAll();
                    atencionRepository.DeleteAll();
                    derivacionRepository.DeleteAll();
                }

                try
                {
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

                        foreach (var asistente in asistentes)
                            asistenteRepository.Create(asistente);

                        foreach (var persona in personas)
                            personaRepository.Create(persona);

                        foreach (var cita in citas)
                        {
                            var persona = personas[random.Next(0, personas.Count - 1)];
                            persona.AddCita(cita);
                            cita.Asistente = asistentes[random.Next(0, asistentes.Count - 1)];
                            citaRepository.Create(cita);
                        }

                        int i = 0;
                        foreach (var atencion in atenciones)
                        {
                            atencion.Cita = citas[i++];
                            atencion.Derivacion = FakeDerivacionRepository.Next(atencion);

                            atencionRepository.Create(atencion);
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
            //InitializeCollections();
        }

        private void InitializeCollections()
        {
            Debug.StartWatch();
            var session = Container.Resolve<ISession>();
            _PersonaRepository = Container.Resolve<IPersonaRepository>();
            _CitaRepository = Container.Resolve<ICitaRepository>();
            _AtencionRepository = Container.Resolve<IAtencionRepository>();
            _AsistenteRepository = Container.Resolve<IAsistenteRepository>();
            Gama.Common.Debug.Debug.StopWatch("Resolve ISession y repository");

            Debug.StartWatch(); _PersonaRepository.Session = session; Debug.StopWatch("Personas Session");
            Debug.StartWatch(); _CitaRepository.Session = session; Debug.StopWatch("Citas Session");
            Debug.StartWatch(); _AtencionRepository.Session = session; Debug.StopWatch("Atenciones Session");
            Debug.StartWatch(); _AsistenteRepository.Session = session; Debug.StopWatch("Asistentes Session");


            Debug.StartWatch(); var personas = _PersonaRepository.Personas; Debug.StopWatch("Personas");
            Debug.StartWatch(); _CitaRepository.Citas = _PersonaRepository.Personas.SelectMany(p => p.Citas).ToList(); Debug.StopWatch("Citas");
            Debug.StartWatch(); _AtencionRepository.Atenciones = _CitaRepository.Citas.Select(c => c.Atencion).Where(a => a != null).ToList(); Debug.StopWatch("Atenciones");
        }



        private List<Cita> _Citas = new List<Cita>();
        private List<Atencion> _Atenciones = new List<Atencion>();
        private List<Derivacion> _Derivaciones = new List<Derivacion>();
        private List<Asistente> _Asistentes = new List<Asistente>();
        private List<Persona> _Personas = new List<Persona>();
        private IAsistenteRepository _AsistenteRepository;
        private IPersonaRepository _PersonaRepository;
        private ICitaRepository _CitaRepository;
        private IAtencionRepository _AtencionRepository;

        private void DoThings()
        {
            Debug.StartWatch();
            var session = Container.Resolve<ISession>();
            _PersonaRepository = Container.Resolve<IPersonaRepository>();
            _CitaRepository = Container.Resolve<ICitaRepository>();
            _AtencionRepository = Container.Resolve<IAtencionRepository>();
            _AsistenteRepository = Container.Resolve<IAsistenteRepository>();

            Persona persona; MySqlDataReader reader;
            try
            {
                using (MySqlConnection mysqlConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["GamaAtencionesMySql"].ConnectionString))
                {
                    using (MySqlCommand sqlCommand = new MySqlCommand())
                    {
                        sqlCommand.Connection = mysqlConnection;
                        mysqlConnection.Open();
                        //UIServices.SetBusyState();

                        sqlCommand.CommandText = "SELECT * FROM personas";
                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //System.Console.WriteLine(reader["Id"].ToString());
                                //System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["Nombre"].ToString()));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["Nif"].ToString()));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["Nif"].ToString()));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["ComoConocioAGama"].ToString()));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["DireccionPostal"].ToString()));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["Email"].ToString()));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["EstadoCivil"].ToString()));
                                //    System.Console.WriteLine((DateTime?)reader["FechaDeNacimiento"]);
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["Facebook"].ToString()));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["IdentidadSexual"].ToString()));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["LinkedIn"].ToString()));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["Nacionalidad"].ToString()));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["NivelAcademico"].ToString()));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["Ocupacion"].ToString()));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["OrientacionSexual"].ToString()));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["Telefono"].ToString()));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["Twitter"].ToString()));
                                //    //System.Console.WriteLinCore.Encryption.Cipher.Decrypt(( reader["Imagen"].GetType() == (typeof(DBNull)) ? null : reader["Imagen"] as byte[]);
                                //    //System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["Imagen"] as byte[]));
                                //    System.Console.WriteLine(Core.Encryption.Cipher.Decrypt(reader["ViaDeAccesoAGama"].ToString()));
                                //    System.Console.WriteLine("Created At: " + ((DateTime)reader["CreatedAt"]).ToString());
                                //    System.Console.WriteLine("Updated At: " + (reader["UpdatedAt"] as DateTime?).ToString());
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
                                    //Imagen = reader["Imagen"].GetType() == (typeof(DBNull)) ? null : reader["Imagen"] as byte[],
                                    Imagen = reader["Imagen"] as byte[],
                                    ViaDeAccesoAGama = reader["ViaDeAccesoAGama"].ToString(),
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    UpdatedAt = reader["UpdatedAt"] as DateTime?,
                                };

                                //var ok = reader["Imagen"] as byte[];
                                //if (ok != null)
                                //{
                                //    var type = ok.GetType();
                                //    var type2 = (new byte[10]).GetType();
                                //}

                                persona.Decrypt();
                                _Personas.Add(persona);
                            }
                        }

                        sqlCommand.CommandText = "SELECT * FROM asistentes";
                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var asistente = new Asistente()
                                {
                                    Id = (int)reader["Id"],
                                    Nombre = reader["Nombre"].ToString(),
                                };
                                asistente.Decrypt();
                                _Asistentes.Add(asistente);
                            }
                        }

                        sqlCommand.CommandText = "SELECT * FROM citas";
                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var cita = new Cita()
                                {
                                    Id = (int)reader["Id"],
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

                        sqlCommand.CommandText = "SELECT * FROM atenciones";
                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var atencion = new Atencion()
                                {
                                    Id = (int)reader["Id"],
                                };
                                atencion.Decrypt();
                                _Atenciones.Add(atencion);

                                var cita = _Citas.Where(c => c.Id == (int)reader["Cita_id"]).Single();
                                cita.Atencion = atencion;
                                atencion.Cita = cita;
                            }
                        }

                        sqlCommand.CommandText = "SELECT * FROM derivaciones";
                        using (reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var derivacion = new Derivacion()
                                {
                                    Id = (int)reader["Id"],
                                };
                                _Derivaciones.Add(derivacion);

                                var atencion = _Atenciones.Where(a => a.Id == (int)reader["Atencion_id"]).Single();
                                derivacion.Atencion = atencion;
                                atencion.Derivacion = derivacion;
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

            _PersonaRepository.Personas = _Personas;
            _CitaRepository.Citas = _Citas;
            _AtencionRepository.Atenciones = _Atenciones;
            _AsistenteRepository.Asistentes = _Asistentes;

            _PersonaRepository.Session = session;
            _CitaRepository.Session = session;
            _AtencionRepository.Session = session;
            _AsistenteRepository.Session = session;

            Debug.StopWatch("RAW SQL");

            Debug.StartWatch();
            //_Personas = base.GetAll();
            Debug.StopWatch("NHIBERNATE SQL");
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
