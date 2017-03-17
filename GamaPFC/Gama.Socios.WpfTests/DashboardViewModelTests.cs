using Gama.Socios.Wpf.FakeServices;
using Gama.Socios.Wpf.Services;
using Gama.Socios.Wpf.ViewModels;
using Moq;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Eventos;
using Xunit;

namespace Gama.Socios.WpfTests
{
    public class DashboardViewModelTests
    {
        private Mock<IEventAggregator> _EventAggregatorMock;
        private Mock<ISession> _SessionMock;
        private Mock<IPreferenciasDeSocios> _SettingsMock;
        private Mock<SocioCreadoEvent> _SocioCreadoEventMock;
        private Mock<SocioDadoDeBajaEvent> _SocioEliminadoEventMock;
        private Mock<SocioSeleccionadoEvent> _SocioSeleccionadoEventMock;
        private Mock<ISocioRepository> _SocioRepositoryMock;
        private List<Socio> _Socios;
        DashboardViewModel _Vm;

        public DashboardViewModelTests()
        {
            _Socios = new FakeSocioRepository().GetAll();

            _SocioRepositoryMock = new Mock<ISocioRepository>();
            _SettingsMock = new Mock<IPreferenciasDeSocios>();
            _EventAggregatorMock = new Mock<IEventAggregator>();
            _SessionMock = new Mock<ISession>();

            _SocioRepositoryMock.Setup(x => x.GetAll()).Returns(_Socios);

            _SocioCreadoEventMock = new Mock<SocioCreadoEvent>();
            _SocioEliminadoEventMock = new Mock<SocioDadoDeBajaEvent>();
            _SocioSeleccionadoEventMock = new Mock<SocioSeleccionadoEvent>();

            _EventAggregatorMock.Setup(x => x.GetEvent<SocioCreadoEvent>()).Returns(
                _SocioCreadoEventMock.Object);
            _EventAggregatorMock.Setup(x => x.GetEvent<SocioDadoDeBajaEvent>()).Returns(
                _SocioEliminadoEventMock.Object);
            _EventAggregatorMock.Setup(x => x.GetEvent<SocioSeleccionadoEvent>()).Returns(
                _SocioSeleccionadoEventMock.Object);

            _SettingsMock.SetupGet(x => x.DashboardMesesAMostrarDeSociosNuevos).Returns(6);
            _SettingsMock.SetupGet(x => x.DashboardUltimosSocios).Returns(15);

            _Vm = new DashboardViewModel(
                _SocioRepositoryMock.Object,
                _EventAggregatorMock.Object,
                new PreferenciasDeSocios(),
                _SessionMock.Object
                );
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            _SocioRepositoryMock.Verify(x => x.GetAll(), Times.Once);
            Assert.NotNull(_Vm.UltimosSocios);
            Assert.NotNull(_Vm.SociosCumpliendoBirthdays);
            Assert.NotNull(_Vm.SociosMorosos);
            Assert.True(_Vm.UltimosSocios.Count <= _SettingsMock.Object.DashboardUltimosSocios);
            Assert.NotNull(_Vm.SeleccionarSocioCommand);
            Assert.NotNull(_Vm.SociosNuevosPorMes);
        }

        [Fact]
        private void ShouldPublishSocioSeleccionadoEventWhenASocioIsSelected()
        {
            _Vm.SeleccionarSocioCommand.Execute(new Socio { Id = 1 });
            _SocioSeleccionadoEventMock.Verify(x => x.Publish(1), Times.Once);
        }

        [Fact]
        private void NuevoSocioShouldSetElSocioEnPrimeraPosicionDeLosUltimosSociosMostrados()
        {
            var Socio = new Socio { Id = int.MaxValue };
            _SocioRepositoryMock.Setup(p => p.GetById(It.IsAny<int>())).Returns(Socio);

            var eventAggregator = new EventAggregator();

            var vm = new DashboardViewModel(
                _SocioRepositoryMock.Object,
                eventAggregator,
                new PreferenciasDeSocios(),
                _SessionMock.Object);

            eventAggregator.GetEvent<SocioCreadoEvent>().Publish(Socio.Id);

            Assert.Equal(Socio.Id, vm.UltimosSocios.First().Id);
        }
    }
}
