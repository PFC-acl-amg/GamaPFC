using Gama.Socios.Wpf.Eventos;
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
using Xunit;

namespace Gama.Socios.WpfTests
{
    public class NuevoSocioViewModelTests
    {
        private Mock<IEventAggregator> _EventAggregatorMock;
        private Mock<SocioCreadoEvent> _NuevoSocioEvent;
        private Mock<ISocioRepository> _SocioRepositoryMock;
        private SocioViewModel _SocioViewModelMock;
        private Mock<ISession> _SessionMock;
        NuevoSocioViewModel _Vm;

        public NuevoSocioViewModelTests()
        {
            _SocioRepositoryMock = new Mock<ISocioRepository>();
            _EventAggregatorMock = new Mock<IEventAggregator>();
            _NuevoSocioEvent = new Mock<SocioCreadoEvent>();
            _SessionMock = new Mock<ISession>();

            _EventAggregatorMock.Setup(e => e.GetEvent<SocioCreadoEvent>())
                .Returns(_NuevoSocioEvent.Object);

            _EventAggregatorMock.Setup(e => e.GetEvent<SocioCreadoEvent>()
                .Publish(It.IsAny<int>())).Verifiable();

            _SocioViewModelMock = new SocioViewModel();

            _Vm = new NuevoSocioViewModel(
                _SocioRepositoryMock.Object,
                _EventAggregatorMock.Object,
                _SocioViewModelMock,
                _SessionMock.Object);
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.Null(_Vm.Cerrar);
            Assert.NotNull(_Vm.SocioViewModel);
            Assert.False(_Vm.AceptarCommand.CanExecute(null));
            Assert.True(_Vm.CancelarCommand.CanExecute(null));
        }

        [Fact]
        private void ShouldSetCreatedAtFieldToToday()
        {
            Assert.True(_Vm.Socio.CreatedAt.Year < 1950);

            _Vm.AceptarCommand.Execute(null);

            Assert.True(_Vm.Socio.CreatedAt.Date == DateTime.Now.Date);
        }

        [Fact]
        private void ShouldCallMethodCreateWithInnerModelWhenASocioIsCreated()
        {
            _Vm.AceptarCommand.Execute(null);

            _SocioRepositoryMock.Verify(p => p.Create(_Vm.Socio.Model), Times.Once);
        }

        [Fact]
        private void ShouldPublishNuevoSocioEventWhenASocioIsCreated()
        {
            _Vm.AceptarCommand.Execute(null);

            _EventAggregatorMock.Verify(e => e.GetEvent<SocioCreadoEvent>()
                .Publish(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        private void ShouldCancelar()
        {
            Assert.Null(_Vm.Cerrar);
            _Vm.CancelarCommand.Execute(null);
            Assert.True(_Vm.Cerrar);
        }
    }
}
