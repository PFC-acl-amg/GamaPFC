using Core;
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
        Mock<ICooperacionSettings> _settingsMock;
        Mock<ISession> _sessionMock;
        Mock<NuevaActividadEvent> _nuevaActividadEventMock;
        Mock<ActividadActualizadaEvent> _actividadActualizadaEventMock;
        List<Actividad> _actividades;
        List<Cooperante> _cooperantes;
        DashboardViewModel _vm;

        public DashboardViewModelTests()
        {
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _actividadRepositoryMock = new Mock<IActividadRepository>();
            _cooperanteRepositoryMock = new Mock<ICooperanteRepository>();
            _settingsMock = new Mock<ICooperacionSettings>();
            _sessionMock = new Mock<ISession>();

            _settingsMock.SetupGet(s => s.DashboardActividadesAMostrar).Returns(25);
            _settingsMock.SetupGet(s => s.DashboardCooperantesAMostrar).Returns(30);

            _actividades = new FakeActividadRepository().GetAll();
            _cooperantes = new FakeCooperanteRepository().GetAll();

            _actividadRepositoryMock.Setup(a => a.GetAll()).Returns(_actividades);
            _cooperanteRepositoryMock.Setup(c => c.GetAll()).Returns(_cooperantes);

            _vm = new DashboardViewModel(
                _actividadRepositoryMock.Object,
                _cooperanteRepositoryMock.Object,
                _eventAggregatorMock.Object,
                _settingsMock.Object,
                _sessionMock.Object);
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.Equal(_vm.UltimasActividades.Count, _settingsMock.Object.DashboardActividadesAMostrar);
            Assert.Equal(_vm.UltimosCooperantes.Count, _settingsMock.Object.DashboardCooperantesAMostrar);
            Assert.NotNull(_vm.SelectActividadCommand);
            Assert.NotNull(_vm.SelectCooperanteCommand);
        }
    }
}
