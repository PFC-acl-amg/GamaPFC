using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.ViewModels;
using Moq;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.Atenciones.WpfTests
{
    public class NuevaPersonaViewModelTests
    {
        private Mock<IEventAggregator> _EventAggregatorMock;
        private Mock<PersonaCreadaEvent> _NuevaPersonaEvent;
        private Mock<IPersonaRepository> _PersonaRepositoryMock;
        private PersonaViewModel _PersonaViewModelMock;
        private Mock<ISession> _SessionMock;
        NuevaPersonaViewModel _Vm;

        public NuevaPersonaViewModelTests()
        {
            _PersonaRepositoryMock = new Mock<IPersonaRepository>();
            _EventAggregatorMock = new Mock<IEventAggregator>();
            _NuevaPersonaEvent = new Mock<PersonaCreadaEvent>();
            _SessionMock = new Mock<ISession>();

            _EventAggregatorMock.Setup(e => e.GetEvent<PersonaCreadaEvent>())
                .Returns(_NuevaPersonaEvent.Object);

            _EventAggregatorMock.Setup(e => e.GetEvent<PersonaCreadaEvent>()
                .Publish(It.IsAny<int>())).Verifiable();

            //_PersonaViewModelMock = new PersonaViewModel();

            _Vm = new NuevaPersonaViewModel(
                _PersonaRepositoryMock.Object,
                _EventAggregatorMock.Object,
                _PersonaViewModelMock,
                _SessionMock.Object);
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.Null(_Vm.Cerrar);
            Assert.NotNull(_Vm.PersonaVM);
            Assert.False(_Vm.AceptarCommand.CanExecute(null));
            Assert.True(_Vm.CancelarCommand.CanExecute(null));
        }

        [Fact]
        private void ShouldSetCreatedAtFieldToToday()
        {
            Assert.True(_Vm.Persona.CreatedAt.Year < 1950);

            _Vm.AceptarCommand.Execute(null);

            Assert.True(_Vm.Persona.CreatedAt.Date == DateTime.Now.Date);
        }

        [Fact]
        private void ShouldCallMethodCreateWithInnerModelWhenAPersonaIsCreated()
        {
            _Vm.AceptarCommand.Execute(null);

            _PersonaRepositoryMock.Verify(p => p.Create(_Vm.Persona.Model), Times.Once);
        }

        [Fact]
        private void ShouldPublishNuevaPersonaEventWhenAPersonaIsCreated()
        {
            _Vm.AceptarCommand.Execute(null);

            _EventAggregatorMock.Verify(e => e.GetEvent<PersonaCreadaEvent>()
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
