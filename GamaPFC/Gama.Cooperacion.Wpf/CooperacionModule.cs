using Core;
using Core.DataAccess;
using Gama.Common;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.DataAccess;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.ViewModels;
using Gama.Cooperacion.Wpf.Views;
using Microsoft.Practices.Unity;
using NHibernate;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Gama.Cooperacion.Wpf
{
    public class CooperacionModule : ModuleBase
    {
        private DateTime _FechaHoy;
        public CooperacionModule(IUnityContainer container, IRegionManager regionManager)
           : base(container, regionManager)
        {
            this.Entorno = Entorno.Desarrollo;
            this.SeedDatabase = false; // A falso no entra en el if this.UseFaker y no crea nada mas
        }

        public override void Initialize()
        {
            EstablecerFecha();
            RegisterViews();
            RegisterViewModels();
            RegisterServices();
            ActualizarEstadosActividades();
            
            // Aqui se pueden poner las comprobaciones de estado de las actividades.

            //ILoggerFacade log = Container.Resolve<ILoggerFacade>();
            //log.Log("ok", Category.Exception, Priority.None);

            if (this.SeedDatabase)
            {
                var session = Container.Resolve<ISession>();
                var cooperanteRepository = Container.Resolve<ICooperanteRepository>();
                cooperanteRepository.Session = session;
                var cooperantesDummy = new FakeCooperanteRepository().GetAll().Take(2);

                foreach (var cooperante in cooperantesDummy) // Crea tambien mas cooperantes de forma automatica
                {
                    cooperanteRepository.Create(cooperante);
                }

                var actividadRepository = Container.Resolve<IActividadRepository>();
                //var eventoRepository = Container.Resolve<IEventoRepository>();
     
                actividadRepository.Session = session;
                cooperanteRepository.Session = session;
                //eventoRepository.Session = session;

                foreach (var cooperante in cooperantesDummy)
                {
                    //cooperanteRepository.Create(cooperante); // para crear cooperantes nuevos forma automatica
                }

                //var cooperanteRepository = Container.Resolve<ICooperanteRepository>();
                //var actividadRepository = Container.Resolve<IActividadRepository>();
                //var session = Container.Resolve<ISession>();
                //actividadRepository.Session = session;
                //cooperanteRepository.Session = session;

                var coordinador = cooperanteRepository.GetAll().First();
                var actividadesFake = new FakeActividadRepository().GetAll();

                foreach (var actividad in actividadesFake.Take(1))
                {
                    var eventosFake = new FakeEventoRepository().GetAll();
                    var foroFake = new FakeForoRepository().GetAll();
                    var mensajeForoFake = new FakeMensajeRepository().GetAll();
                    var tareaFake = new FakeTareaRepository().GetAll();
                    var seguimientoFake = new FakeSeguimientoRepository().GetAll();
                    var incidenciaFake = new FakeIncidenciaRepository().GetAll();
                    //foreach (var tarea in tareaFake)
                    //{
                    //    var seguimientoFake = new FakeSeguimientoRepository().GetAll();
                    //    var incidenciaFake = new FakeIncidenciaRepository().GetAll();
                    //    int j = 0;
                    //    int k = 0;
                    //    int l = 0;
                    //    foreach (var seguimiento in seguimientoFake)
                    //    {
                    //        tarea.Seguimiento.Insert(j, seguimiento);
                    //        j++;
                    //    }
                    //    foreach (var incidencia in incidenciaFake)
                    //    {
                    //        tarea.Incidencias.Insert(l, incidencia);
                    //        l++;
                    //    }
                    //    actividad.Tareas.Insert(k, tarea);
                    //    k++;
                    //}
                    actividad.Coordinador = coordinador;
                    //foreach (var InsertandoTareas in tareaFake)
                    //{
                    //    foreach (var InsertandoSeguimientos in seguimientoFake)
                    //    {
                    //        InsertandoTareas.AddSeguimiento(InsertandoSeguimientos);
                    //    }
                    //    foreach(var InsertandoIncidencias in incidenciaFake)
                    //    {
                    //        InsertandoTareas.AddIncidencia(InsertandoIncidencias);
                    //    }
                    //    InsertandoTareas.Responsable = coordinador;
                    //    actividad.AddTarea(InsertandoTareas);
                    //}
                    //foreach (var InsertandoEvento in eventosFake)
                    //{
                    //    actividad.AddEvento(InsertandoEvento);
                    //}
                    //foreach (var InsertandoForos in foroFake)
                    //{
                    //    foreach (var InsertandoMensajesForos in mensajeForoFake)
                    //    {
                    //        InsertandoForos.AddMensaje(InsertandoMensajesForos);
                    //    }
                    //    actividad.AddForo(InsertandoForos);
                    //}
                    actividadRepository.Create(actividad);
                }
            }

                InitializeNavigation();
        }

        private void RegisterViews()
        {
            Container.RegisterType<object, ActividadesContentView>("ActividadesContentView");
            Container.RegisterType<object, DashboardView>("DashboardView");
            Container.RegisterType<object, EditarActividadView>("EditarActividadView");
            Container.RegisterType<object, InformacionDeActividadView>("InformacionDeActividadView");
            Container.RegisterType<object, ListadoDeActividadesView>("ListadoDeActividadesView");
            Container.RegisterType<object, NuevaActividadView>("NuevaActividadView");
            Container.RegisterType<object, PanelSwitcherView>("PanelSwitcherView");
            Container.RegisterType<object, StatusBarView>("StatusBarView");
            Container.RegisterType<object, ToolbarView>("ToolbarView");
            Container.RegisterType<object, TareasDeActividad>("TareasDeActividad");
        }

        private void RegisterViewModels()
        {
            Container.RegisterType<ActividadesContentViewModel>();
            Container.RegisterType<DashboardViewModel>();
            Container.RegisterType<EditarActividadViewModel>();
            Container.RegisterType<InformacionDeActividadViewModel>();
            Container.RegisterType<ListadoDeActividadesViewModel>();
            Container.RegisterType<NuevaActividadViewModel>();
            Container.RegisterType<PanelSwitcherViewModel>();
            Container.RegisterType<StatusBarViewModel>();
            Container.RegisterType<ToolbarViewModel>();
            Container.RegisterType<TareasDeActividadViewModel>();
        }

        private void RegisterServices()
        {
            Container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactory());
            Container.RegisterType<ISession>(new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenSession()));
            Container.RegisterType<IActividadRepository, ActividadRepository>();
            Container.RegisterType<ICooperanteRepository, CooperanteRepository>();
            Container.RegisterType<IForoRepository, ForoRepository>();
            Container.RegisterType<ITareaRepository, TareaRepository>();
            // Añido para eventos
            //Container.RegisterType<IEventoRepository, EventoRepository>();
            //Container.RegisterType<IIncidenciaRepository, IncidenciaRepository>();
            //Container.RegisterType<ITareaRepository, TareaRepository>();
            //Container.RegisterType<ISeguimientoRepository, SeguimientoRepository>();

            Container.RegisterInstance<ICooperacionSettings>(
                new CooperacionSettings());

            //
            // Fake
            //
            //Container.RegisterInstance<ISession>(new Mock<ISession>().Object);
            //Container.RegisterType<IActividadRepository, FakeActividadRepository>();
            //Container.RegisterType<ICooperanteRepository, FakeCooperanteRepository>();
            //Container.RegisterInstance<ICooperacionSettings>(new CooperacionSettings());
        }
        private void ActualizarEstadosActividades()
        {
            var actividadRepository = Container.Resolve<IActividadRepository>();
            var session = Container.Resolve<ISession>();
            actividadRepository.Session = session;
            ListaCompletaActividades = new ObservableCollection<Actividad>(actividadRepository.GetAll());
            //foreach (var ActividadSeleccionada in ListaCompletaActividades)
            //{
            //    int cont;
            //    int CompararFecha = DateTime.Compare(ActividadSeleccionada.FechaDeInicio, _FechaHoy);
            //    if (CompararFecha >= 0)
            //    {

            //    }
                    
            //}
            //if (vm.ModuloSeleccionado.Value == Modulos.Cooperacion) // Si se selecciona Cooperacion Actualizamos los estados de las actividades según si fecha
            //{
            //    DateTime dia1 = new DateTime(2017, 05, 26);
            //    DateTime dia2 = new DateTime(2017, 05, 27);
            //    string relationship;
            //    int result = DateTime.Compare(dia1, dia2);
            //    if (result < 0)
            //        relationship = "is earlier than";
            //    else if (result == 0)
            //        relationship = "is the same time as";
            //    else
            //        relationship = "is later than";

            //    Console.WriteLine("{0} {1} {2}", dia1, relationship, dia2);
            //}
        }
        private void EstablecerFecha()
        {
            //_FechaHoy = DateTime.Now; // Para asignar la fecha de hoy
            DateTime _FechaHoy = new DateTime(2017, 05, 02);
        }
        private void InitializeNavigation()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.PanelSwitcherRegion, typeof(PanelSwitcherView));
            RegionManager.RegisterViewWithRegion(RegionNames.ToolbarRegion, typeof(ToolbarView));
            RegionManager.RegisterViewWithRegion(RegionNames.StatusBarRegion, typeof(StatusBarView));
            //RegionManager.RequestNavigate(RegionNames.ContentRegion, "ActividadesContentView");
            //RegionManager.AddToRegion(RegionNames.ActividadesTabContentRegion, new ListadoDeActividadesView());
            //RegionManager.RequestNavigate(RegionNames.ActividadesTabContentRegion, "ListadoDeActividadesView");
            RegionManager.RequestNavigate(RegionNames.ContentRegion, "DashboardView");
            RegionManager.AddToRegion(RegionNames.ContentRegion, Container.Resolve<ActividadesContentView>());
            RegionManager.AddToRegion(RegionNames.ActividadesTabContentRegion, Container.Resolve<ListadoDeActividadesView>());
        }
        public ObservableCollection<Actividad> ListaCompletaActividades { get; private set; }
    }
}
