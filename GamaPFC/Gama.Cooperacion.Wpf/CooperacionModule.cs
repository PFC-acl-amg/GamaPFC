﻿using Core;
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Gama.Cooperacion.Wpf
{
    // Clase para guardar los estados de las actividades que se usara en el Dashboard ViewModel de Cooperacion.
    public static class GestionErrores
    {
        public static int ErrorDuplicateKey = 0;
    }
    public static class ColeccionEstadosActividades
    {
        
        public static Dictionary<string,int> EstadosActividades { get; set; }
        // EstadosActividades tiene que inicializarse para que cuando de compare por primera vez no falle por ser nulo
        // Esta inicializacion se realiza en Bootstrapper.cs de Cooperacion
        
        public static void ContandoEstadosActividades(string Estado)
        {
            if (!EstadosActividades.ContainsKey(Estado))    // Si es estado no esta en el Diccionario se añade y cuenta 1.
            {
                EstadosActividades.Add(Estado, 1);
            }
            else                                            // Si esta en el diccionario se incrementa en 1.
            {
                EstadosActividades[Estado]++;
            }
        }
    }
        public class CooperacionModule : ModuleBase
    {
        private DateTime _FechaTest;

        public CooperacionModule(IUnityContainer container, IRegionManager regionManager)
           : base(container, regionManager)
        {

        }

        public override void Initialize()
        {
            EstablecerFecha();
            RegisterViews();
            RegisterViewModels();
            InitializeNavigation();
            ActualizarEstadosActividades();
        }

        private void RegisterViews()
        {
            Container.RegisterType<object, ActividadesContentView>("ActividadesContentView");
            Container.RegisterType<object, CooperanteView>("CooperantesContentView");
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
            Container.RegisterType<DashboardViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ActividadesContentViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<EditarActividadViewModel>();
            Container.RegisterType<InformacionDeActividadViewModel>();
            Container.RegisterType<TareasDeActividadViewModel>();
            Container.RegisterType<ListadoDeActividadesViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CooperantesContentViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CalendarioDeActividadesViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<NuevaActividadViewModel>();
            Container.RegisterType<PanelSwitcherViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<StatusBarViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ToolbarViewModel>(new ContainerControlledLifetimeManager());
        }

        private void ActualizarEstadosActividades()
        {
            var actividadRepository = new NHibernateOneSessionRepository<Actividad, int>();// Container.Resolve<IActividadRepository>();
            var session = Container.Resolve<ISession>();
            actividadRepository.Session = session;
            ListaCompletaActividades = new ObservableCollection<Actividad>(actividadRepository.GetAll());
            foreach (var ActividadSeleccionada in ListaCompletaActividades)
            {
                // _FechaTest = 02.05.2017
                int CompararFecha = DateTime.Compare(_FechaTest, ActividadSeleccionada.FechaDeInicio); // Comprabando si comenzo el proyecto
                int SemanaFin = DateTime.Compare(_FechaTest, ActividadSeleccionada.FechaDeFin.AddDays(-7)); // Comprabando si comenzo el proyecto
                int FueraPlazo = DateTime.Compare(_FechaTest, ActividadSeleccionada.FechaDeFin);
                if (FueraPlazo >= 0)  // FechaTest despues FechaFin => 1 Si no esta finalizada la actividad esta fuera de plazo
                {
                    if (ActividadSeleccionada.Estado != Estado.Finalizado)
                    {
                        ActividadSeleccionada.Estado = Estado.FueraPlazo;                                       // Se modifica el estado
                        string EstadoActividadSeleccionada = ActividadSeleccionada.Estado.ToString();           // Obtiene estado Actual Actividad
                        ColeccionEstadosActividades.ContandoEstadosActividades(EstadoActividadSeleccionada);    // Contabiliza el estado de la Actividad
                        actividadRepository.Update(ActividadSeleccionada);                                      // Actualiza el estado en BBDD.
                    }
                    else
                    {
                        string EstadoActividadSeleccionada = ActividadSeleccionada.Estado.ToString();           // Obtiene estado Actual Actividad
                        ColeccionEstadosActividades.ContandoEstadosActividades(EstadoActividadSeleccionada);    // Contabiliza el estado.Finalizado de la Actividad
                    }
                }
                else
                {
                    if (CompararFecha < 0)  // FechaTest es antes Fecha Inicio => -1 NoComenzado
                    {
                        ActividadSeleccionada.Estado = Estado.NoComenzado;                                      // Se modifica el estado
                        string EstadoActividadSeleccionada = ActividadSeleccionada.Estado.ToString();           // Contabiliza el estado de la Actividad
                        ColeccionEstadosActividades.ContandoEstadosActividades(EstadoActividadSeleccionada);
                        actividadRepository.Update(ActividadSeleccionada);                                      // Actualiza el estado en BBDD.
                    }
                    else        // FechaTest es Igual o posterior a la FechaInicio => 0/1 Comenzado
                    {
                        if ((CompararFecha >= 0) && (SemanaFin < 0))     // FechaTest tiene que ser anterior en una semana para que sea Comenzado
                        {
                            ActividadSeleccionada.Estado = Estado.Comenzado;                                      // Se modifica el estado
                            string EstadoActividadSeleccionada = ActividadSeleccionada.Estado.ToString();
                            ColeccionEstadosActividades.ContandoEstadosActividades(EstadoActividadSeleccionada);
                            actividadRepository.Update(ActividadSeleccionada);                                    // Actualiza el estado en BBDD.
                        }
                        else
                        {
                            if (SemanaFin >= 0)     // Esta a una semana o menos de finalizar el plazo de entrega
                            {
                                ActividadSeleccionada.Estado = Estado.ProximasFinalizaciones;                           // Se modifica el estado
                                string EstadoActividadSeleccionada = ActividadSeleccionada.Estado.ToString();
                                ColeccionEstadosActividades.ContandoEstadosActividades(EstadoActividadSeleccionada);
                                actividadRepository.Update(ActividadSeleccionada);                                      // Actualiza el estado en BBDD.
                            }
                        }
                    }
                }
            }
        }
        public void EstablecerFecha()
        {
            // Fecha para realizar puebas. Las Fechas de Inicio y Fin de las actividades estan en el Mes Mayo
            _FechaTest = new DateTime(2017, 05, 02);
        }
        private void InitializeNavigation()
        {
            //RegionManager.RegisterViewWithRegion(RegionNames.ToolbarRegion, typeof(ToolbarView));
            //RegionManager.RegisterViewWithRegion(RegionNames.StatusBarRegion, typeof(StatusBarView));
            //RegionManager.RequestNavigate(RegionNames.ContentRegion, "ActividadesContentView");
            //RegionManager.AddToRegion(RegionNames.ActividadesTabContentRegion, new ListadoDeActividadesView());
            //RegionManager.RequestNavigate(RegionNames.ActividadesTabContentRegion, "ListadoDeActividadesView");
            //RegionManager.RequestNavigate(RegionNames.ContentRegion, "DashboardView");
            //RegionManager.AddToRegion(RegionNames.ContentRegion, Container.Resolve<CooperantesContentView>());
            //RegionManager.AddToRegion(RegionNames.ContentRegion, Container.Resolve<ActividadesContentView>());
            //RegionManager.AddToRegion(RegionNames.ActividadesTabContentRegion, Container.Resolve<ListadoDeActividadesView>());
            //RegionManager.RegisterViewWithRegion(RegionNames.RightCommandsRegion, typeof(RightCommandsView));

        }
        public ObservableCollection<Actividad> ListaCompletaActividades { get; private set; }
    }
}
