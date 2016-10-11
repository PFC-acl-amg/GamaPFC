﻿using Core;
using Core.DataAccess;
using Gama.Common;
using Gama.Cooperacion.DataAccess;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.ViewModels;
using Gama.Cooperacion.Wpf.Views;
using Microsoft.Practices.Unity;
using NHibernate;
using Prism.Logging;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf
{
    public class CooperacionModule : ModuleBase
    {
        public CooperacionModule(IUnityContainer container, IRegionManager regionManager)
           : base(container, regionManager)
        {
            this.Entorno = Entorno.Desarrollo;
            this.UseFaker = true; // A falso no entra en el if this.UseFaker y no crea nada mas
        }

        public override void Initialize()
        {
            RegisterViews();
            RegisterViewModels();
            RegisterServices();
            InitializeNavigation();

            ILoggerFacade log = Container.Resolve<ILoggerFacade>();
            log.Log("ok", Category.Exception, Priority.None);

            if (this.UseFaker)
            {
                var cooperantesDummy = new FakeCooperanteRepository().GetAll();
                var cooperanteRepository = Container.Resolve<ICooperanteRepository>();
                var actividadRepository = Container.Resolve<IActividadRepository>();
                var eventoRepository = Container.Resolve<IEventoRepository>();
                //var eventoRepository = new FakeEventoRepository().GetAll();    // Creando eventos no BBDD
                //var eventoRepository = new FakeEventoRepository().GetAll(); // Crea una lista con todos los eventos disponibles en la BBDD
                var session = Container.Resolve<ISession>();

                actividadRepository.Session = session;
                cooperanteRepository.Session = session;
                eventoRepository.Session = session;

                foreach (var cooperante in cooperantesDummy)
                {
                    cooperanteRepository.Create(cooperante);
                }

                //var cooperanteRepository = Container.Resolve<ICooperanteRepository>();
                //var actividadRepository = Container.Resolve<IActividadRepository>();
                //var session = Container.Resolve<ISession>();
                //actividadRepository.Session = session;
                //cooperanteRepository.Session = session;

                var coordinador = cooperanteRepository.GetById(1);
                var actividadesFake = new FakeActividadRepository().GetAll();

                foreach (var actividad in actividadesFake.Take(1))
                {
                    var eventosFake = new FakeEventoRepository().GetAll();
                    var tareaFake = new FakeTareaRepository().GetAll();
                    foreach(var tarea in tareaFake)
                    {
                        var seguimientoFake = new FakeSeguimientoRepository().GetAll();
                        int j = 0;
                        int k = 0;
                        foreach (var seguimiento in seguimientoFake)
                        {
                            tarea.Historial.Insert(j, seguimiento);
                            j++;
                        }
                        actividad.Tareas.Insert(k, tarea);
                        k++;
                    }
                    actividad.Coordinador = coordinador;
                    
                    foreach (var InsertandoEvento in eventosFake)
                    {
                        actividad.AddEvento(InsertandoEvento);
                    }
                    actividadRepository.Create(actividad);
                }
            }
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
            //Container.RegisterInstance(typeof(INHibernateHelper), new NHibernateHelper());
            //Container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactory());

            //Container.RegisterInstance(typeof(ISessionHelper),
            //    new SessionHelper(Container.Resolve<INHibernateHelper>()));
            Container.RegisterInstance<INHibernateSessionFactory>(new NHibernateSessionFactory());
            Container.RegisterType<ISession>(
                new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenSession()));
            //Container.RegisterType<IStatelessSession>(
            //    new InjectionFactory(c => Container.Resolve<INHibernateSessionFactory>().OpenStatelessSession()));

            Container.RegisterType<IActividadRepository, ActividadRepository>();

            //Container.RegisterInstance(typeof(IActividadRepository),
            //    new ActividadRepository(Container.Resolve<ISessionHelper>()));
            Container.RegisterType<ICooperanteRepository, CooperanteRepository>();

            // Añido para eventos
            Container.RegisterType<IEventoRepository, EventoRepository>();

            Container.RegisterInstance<ICooperacionSettings>(
                new CooperacionSettings());

            //Container.RegisterInstance(typeof(IActividadRepository), new FakeActividadRepository());
            //Container.RegisterInstance(typeof(ICooperanteRepository), new FakeCooperanteRepository());
        }

        private void InitializeNavigation()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.PanelSwitcherRegion, typeof(PanelSwitcherView));
            RegionManager.RegisterViewWithRegion(RegionNames.ToolbarRegion, typeof(ToolbarView));
            RegionManager.RegisterViewWithRegion(RegionNames.StatusBarRegion, typeof(StatusBarView));
            RegionManager.RequestNavigate(RegionNames.ContentRegion, "ActividadesContentView");
            RegionManager.RequestNavigate(RegionNames.ActividadesTabContentRegion, "ListadoDeActividadesView");
            RegionManager.RequestNavigate(RegionNames.ContentRegion, "DashboardView");
        }
    }
}
