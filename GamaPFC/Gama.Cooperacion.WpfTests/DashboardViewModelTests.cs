using Core;
using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.ViewModels;
using Moq;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.Cooperacion.WpfTests
{
    public class DashboardViewModelTests
    {
        Mock<IEventAggregator> _eventAggregatorMock;
        Mock<IActividadRepository> _actividadRepositoryMock;
        Mock<ICooperanteRepository> _cooperanteRepositoryMock;
        Mock<Preferencias> _settingsMock;
        Mock<ISession> _sessionMock;
        Mock<ActividadCreadaEvent> _nuevaActividadEventMock;
        Mock<ActividadSeleccionadaEvent> _actividadSeleccionadaEventMock;
        Mock<ActividadActualizadaEvent> _actividadActualizadaEventMock;

        List<Actividad> _actividades;
        List<Cooperante> _cooperantes;
        DashboardViewModel _vm;

        public DashboardViewModelTests()
        {
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _actividadRepositoryMock = new Mock<IActividadRepository>();
            _cooperanteRepositoryMock = new Mock<ICooperanteRepository>();
            _settingsMock = new Mock<Preferencias>();
            _sessionMock = new Mock<ISession>();
            _nuevaActividadEventMock = new Mock<ActividadCreadaEvent>();
            _actividadSeleccionadaEventMock = new Mock<ActividadSeleccionadaEvent>();
            _actividadActualizadaEventMock = new Mock<ActividadActualizadaEvent>();

            _eventAggregatorMock.Setup(ea => ea.GetEvent<ActividadSeleccionadaEvent>())
                .Returns(_actividadSeleccionadaEventMock.Object);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<ActividadCreadaEvent>())
                .Returns(_nuevaActividadEventMock.Object);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<ActividadActualizadaEvent>())
                .Returns(_actividadActualizadaEventMock.Object);

            //_settingsMock.SetupGet(s => s.DashboardActividadesAMostrar).Returns(25);
            //_settingsMock.SetupGet(s => s.DashboardCooperantesAMostrar).Returns(30);
            //_settingsMock.SetupGet(s => s.DashboardActividadesLongitudDeTitulos).Returns(45);
            //_settingsMock.SetupGet(s => s.DashboardMesesAMostrarDeActividadesNuevas).Returns(6);
            //_settingsMock.SetupGet(s => s.DashboardMesesAMostrarDeCooperantesNuevos).Returns(6);

            _actividades = new FakeActividadRepository().GetAll();
            _cooperantes = new FakeCooperanteRepository().GetAll();

            _actividadRepositoryMock.Setup(a => a.GetAll()).Returns(_actividades);
            //_actividadRepositoryMock.Setup(a => a.GetActividadesNuevasPorMes(It.IsAny<int>())).
            //    Returns(new List<int>(_settingsMock.Object.DashboardMesesAMostrarDeActividadesNuevas));
            //_cooperanteRepositoryMock.Setup(c => c.GetAll()).Returns(_cooperantes);
            //_cooperanteRepositoryMock.Setup(c => c.GetCooperantesNuevosPorMes(It.IsAny<int>())).
            //    Returns(new List<int>(_settingsMock.Object.DashboardMesesAMostrarDeCooperantesNuevos));

            //_vm = new DashboardViewModel(
            //    _actividadRepositoryMock.Object,
            //    _cooperanteRepositoryMock.Object,
            //    _eventAggregatorMock.Object,
            //    _settingsMock.Object,
            //    _sessionMock.Object);
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            _actividadRepositoryMock.Verify(ar => ar.GetAll(), Times.Once);
            _cooperanteRepositoryMock.Verify(cr => cr.GetAll(), Times.Once);
            //Assert.Equal(_vm.UltimasActividades.Count, _settingsMock.Object.DashboardActividadesAMostrar);
            //Assert.Equal(_vm.UltimosCooperantes.Count, _settingsMock.Object.DashboardCooperantesAMostrar);
            //Assert.NotNull(_vm.SelectActividadCommand);
            //Assert.NotNull(_vm.SelectCooperanteCommand);
            //Assert.True(_vm.UltimasActividades.All(
            //    a => a.DisplayMember1.Length <= _settingsMock.Object.DashboardActividadesLongitudDeTitulos ^ 
            //    a.DisplayMember1.EndsWith("...")));
        }

        [Fact]
        private void ShouldPublishActividadSeleccionadaEventWhenUnaActividadIsSelected()
        {
            _vm.SelectActividadCommand.Execute(new LookupItem { Id = 1 });
            
            _actividadSeleccionadaEventMock.Verify(e => e.Publish(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        private void NuevaActividadShouldSetLaActividadEnPrimeraPosicionDeLasActividadesMostradas()
        {
            var actividad = new Actividad() { Id = int.MaxValue, Titulo = "Titulo" };
            _actividadRepositoryMock.Setup(ar => ar.GetById(It.IsAny<int>()))
                .Returns(actividad);

            var eventAggregator = new EventAggregator();

            // Necesitamos hacer un Publish real, así que le pasamos el EventAggregator
            // de PRISM, que se asume que está well tested
            //var vm = new DashboardViewModel(
            //    _actividadRepositoryMock.Object,
            //    _cooperanteRepositoryMock.Object,
            //    eventAggregator,
            //    _settingsMock.Object,
            //    _sessionMock.Object);

            eventAggregator.GetEvent<ActividadCreadaEvent>().Publish(actividad.Id);

            //Assert.Equal(actividad.Id, vm.UltimasActividades.First().Id);
        }

        [Fact]
        private void ShouldActualizarLaActividadEnviadaAlLanzarseElEventoDeActividadActualizada()
        {
            var actividad = _actividades[2];
            _actividadRepositoryMock.Setup(ar => ar.GetById(It.IsAny<int>()))
                .Returns(actividad);

            var eventAggregator = new EventAggregator();

            // Necesitamos hacer un Publish real, así que le pasamos el EventAggregator
            // de PRISM, que se asume que está well tested
            //var vm = new DashboardViewModel(
            //    _actividadRepositoryMock.Object,
            //    _cooperanteRepositoryMock.Object,
            //    eventAggregator,
            //    _settingsMock.Object,
            //    _sessionMock.Object);

            actividad.Titulo = "Nuevo título";

            eventAggregator.GetEvent<ActividadActualizadaEvent>().Publish(actividad);

            //string str = vm.UltimasActividades.Single(a => a.Id == actividad.Id).DisplayMember1;
            //if (str.EndsWith("..."))
            //    str = str.Substring(0, str.Length - 1 - "...".Length);

            //Assert.True(actividad.Titulo.StartsWith(str));
        }
    }
}
