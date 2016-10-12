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
        private Mock<NuevaPersonaEvent> _NuevaPersonaEvent;
        private Mock<IPersonaRepository> _PersonaRepositoryMock;
        private PersonaViewModel _PersonaViewModelMock;
        private Mock<ISession> _SessionMock;
        NuevaPersonaViewModel _Vm;

        public NuevaPersonaViewModelTests()
        {
            _PersonaRepositoryMock = new Mock<IPersonaRepository>();
            _EventAggregatorMock = new Mock<IEventAggregator>();
            _NuevaPersonaEvent = new Mock<NuevaPersonaEvent>();
            _SessionMock = new Mock<ISession>();

            _EventAggregatorMock.Setup(e => e.GetEvent<NuevaPersonaEvent>())
                .Returns(_NuevaPersonaEvent.Object);

            _PersonaViewModelMock = new PersonaViewModel();

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
        private void ShouldCancelar()
        {
            Assert.Null(_Vm.Cerrar);
            _Vm.CancelarCommand.Execute(null);
            Assert.True(_Vm.Cerrar);
        }
    }
}
