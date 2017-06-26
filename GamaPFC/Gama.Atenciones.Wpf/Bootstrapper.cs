using Prism.Modularity;
using Prism.Unity;
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
using Gama.Atenciones.Wpf.ViewModels;
using Gama.Atenciones.Wpf.Views;
using System.Linq;
using Prism.Events;
using Core.Util;
using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Business;

namespace Gama.Atenciones.Wpf
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            Container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactory());
            Container.RegisterType<ISession>(
                new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenSession()));
            Container.RegisterType<IPersonaRepository, PersonaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICitaRepository, CitaRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAtencionRepository, AtencionRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAsistenteRepository, AsistenteRepository>(new ContainerControlledLifetimeManager());

            OK();

            var session = Container.Resolve<ISession>();
            var personaRepository = Container.Resolve<IPersonaRepository>();
            var asistenteRepository = Container.Resolve<IAsistenteRepository>();
            personaRepository.Session = session;
            asistenteRepository.Session = session;

            AtencionesResources.TodosLosNifDeAsistentes = asistenteRepository.GetNifs();

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
            return Container.Resolve<Shell>();
        }

        private void OK()
        {
            var sessionFactory = Container.Resolve<INHibernateSessionFactory>();

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

            if (true)
            {
                citaRepository.DeleteAll();
                asistenteRepository.DeleteAll();
                personaRepository.DeleteAll();
                atencionRepository.DeleteAll();
                derivacionRepository.DeleteAll();
            }

            #region Database Seeding
            try
            {
                if (true)
                {
                    var personas = new FakePersonaRepository().GetAll(); //personaRepository.GetAll();
                    var citas = new FakeCitaRepository().GetAll();
                    var atenciones = new FakeAtencionRepository().GetAll();
                    var asistentes = new FakeAsistenteRepository().Asistentes;

                    //personas.ForEach(p => p.Id = 0);
                    //citas.ForEach(c => c.Id = 0);
                    //atenciones.ForEach(a => a.Id = 0);

                    var random = new Random();
                    var opciones = new bool[] { true, false, true, false, true, true, false, true, false };

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
                        atencion.Derivacion = new Derivacion
                        {
                            Id = 0,
                            Atencion = atencion,
                            EsDeFormacion = opciones[random.Next(0, 8)],
                            EsDeFormacion_Realizada = opciones[random.Next(0, 8)],
                            EsDeOrientacionLaboral = opciones[random.Next(0, 8)],
                            EsDeOrientacionLaboral_Realizada = opciones[random.Next(0, 8)],
                            EsExterna = opciones[random.Next(0, 8)],
                            EsExterna_Realizada = opciones[random.Next(0, 8)],
                            EsJuridica = opciones[random.Next(0, 8)],
                            EsJuridica_Realizada = opciones[random.Next(0, 8)],
                            EsPsicologica = opciones[random.Next(0, 8)],
                            EsPsicologica_Realizada = opciones[random.Next(0, 8)],
                            EsSocial = opciones[random.Next(0, 8)],
                            EsSocial_Realizada = opciones[random.Next(0, 8)],
                            Externa = "Externa",
                            Externa_Realizada = "Externa realizada",
                            Tipo = "",
                        };

                        atencionRepository.Create(atencion);
                    }


                    //for (int i = 0; i < -1; i++)
                    //{
                    //    var persona = personas[i];
                    //    var cita = citas[i];
                    //    var atencion = atenciones[i];
                    //    var asistente = asistentes[random.Next(0, 14)];

                    //    cita.Asistente = asistente;
                    //    persona.AddCita(cita);
                    //    atencion.Cita = cita;
                    //    //atencion.Derivacion = derivacion;
                    //    //derivacion.Atencion = atencion;

                    //    int b = i;
                    //    personaRepository.Create(persona);
                    //    citaRepository.Create(cita);
                    //    atencionRepository.Create(atencion);
                    //}
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }
            #endregion
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            string title = "";
            BitmapImage icon = new BitmapImage();

            title = "SERVICIO DE ATENCIONES";
            icon = new BitmapImage(new Uri("pack://application:,,,/Gama.Common;component/Resources/Images/icono_modulo_atenciones.png"));

            ((ShellViewModel)((FrameworkElement)Shell).DataContext).Title = title;
            ((ShellViewModel)((FrameworkElement)Shell).DataContext).IconSource = icon;

            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow.Show();
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
