using Prism.Modularity;
using Prism.Unity;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Unity;
using Gama.Common.BaseClasses;
using System.IO;
using Gama.Common;
using Gama.Socios.Wpf.Services;
using System.Runtime.Serialization.Formatters.Binary;
using Core.DataAccess;
using Gama.Socios.DataAccess;
using NHibernate;
using Gama.Socios.Wpf.FakeServices;
using Gama.Socios.Business;
using System.Collections.Generic;
using Gama.Common.Debug;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Configuration;
using System.Linq;

namespace Gama.Socios.Wpf
{
    public class Bootstrapper : UnityBootstrapperBase
    {
        private List<Socio> _Socios = new List<Socio>();
        private List<PeriodoDeAlta> _PeriodoDeAlta = new List<PeriodoDeAlta>();
        private List<Cuota> _Cuotas = new List<Cuota>();
        private ISocioRepository _SocioRepository;
        private IPeriodoDeAltaRepository _PeriodoDeAltaRepository;
        private ICuotaRepository _CuotaRepository;
        private ISession _Session;

        public Bootstrapper(string title = "GESTIÓN DE SOCIOS") : base(title)
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

            ((ShellViewModel)((FrameworkElement)Shell).DataContext).Title = "GESTIÓN DE SOCIOS";
            ((ShellViewModel)((FrameworkElement)Shell).DataContext).IconSource = icon;

            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow.ShowActivated = true;
            Application.Current.MainWindow.Show();

            TerminarPreload();
        }

        protected override void InitializeDirectories()
        {
            if (!Directory.Exists(ResourceNames.AppDataFolder))
                Directory.CreateDirectory(ResourceNames.AppDataFolder);

            if (!Directory.Exists(ResourceNames.SociosFolder))
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
                new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenSession()));// Desde preferencias de socios llega aqui y boora la BBDD
            Container.RegisterType<ISocioRepository, SocioRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IPeriodoDeAltaRepository, PeriodoDeAltaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICuotaRepository, CuotaRepository>(new ContainerControlledLifetimeManager());

           
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
                var socioRepository = new NHibernateOneSessionRepository<Socio, int>();
                var periodoDeAltaRepository = new NHibernateOneSessionRepository<PeriodoDeAlta, int>();
                var cuotaRepository = new NHibernateOneSessionRepository<Cuota, int>();

                socioRepository.Session = session;
                periodoDeAltaRepository.Session = session;
                cuotaRepository.Session = session;

                try
                {
                    if (_CLEAR_DATABASE)
                        socioRepository.DeleteAll();

                    if (_SEED_DATABASE)
                    {
                        var socios = new FakeSocioRepository().GetAll();
                        var periodosDeAlta = new FakePeriodoDeAltaRepository().GetAll();
                        var cuotas = new FakeCuotaRepository().GetAll();

                        int i = 0;
                        foreach (var socio in socios)
                        {
                            socioRepository.Create(socio);

                            Console.Write($"Socio #{++i};");
                            if (i % 10 == 0)
                                Console.WriteLine("");
                        }

                        i = 0;
                        foreach (var periodoDeAlta in periodosDeAlta)
                        {
                            periodoDeAlta.Socio = socios[i];
                            periodoDeAltaRepository.Create(periodoDeAlta);

                            Console.Write($"Periodo #{++i};");
                            if (i % 10 == 0)
                                Console.WriteLine("");
                        }

                        i = 0;
                        int j = 0;
                        foreach (var cuota in cuotas)
                        {
                            cuota.PeriodoDeAlta = periodosDeAlta[j++];
                            if (i != 0 && i % (periodosDeAlta.Count - 1) == 0)
                                j = 0;

                            cuotaRepository.Create(cuota);

                            Console.Write($"Cuota #{++i};");
                            if (i % 10 == 0)
                                Console.WriteLine("");
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
            _SocioRepository = Container.Resolve<ISocioRepository>();
            _PeriodoDeAltaRepository = Container.Resolve<IPeriodoDeAltaRepository>();
            _CuotaRepository = Container.Resolve<ICuotaRepository>();

            DoRawThings();

            Debug.StopWatch("-----RAW SQL----");
        }
        private void DoRawThings()
        {
            Debug.StartWatch();
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



        protected override void ConfigureModuleCatalog()
        {
            Type atencionesModuleType = typeof(SociosModule);
            ModuleCatalog.AddModule(new ModuleInfo()
            {
                ModuleName = atencionesModuleType.Name,
                ModuleType = atencionesModuleType.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });
        }
    }
}
