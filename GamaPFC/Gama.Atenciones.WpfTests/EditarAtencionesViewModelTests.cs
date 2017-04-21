using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.ViewModels;
using Moq;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.Atenciones.WpfTests
{
    public class EditarAtencionesViewModelTests
    {
        private Mock<IAtencionRepository> _AtencionRepositoryMock;
        private Mock<IEventAggregator> _EventAggregatorMock;
        private Mock<IPersonaRepository> _PersonaRepositoryMock;
        private Mock<ICitaRepository> _CitaRepositoryMock;
        private Mock<IRegionManager> _RegionManagerMock;
        private EditarAtencionesViewModel _Vm;

        public EditarAtencionesViewModelTests()
        {
            _AtencionRepositoryMock = new Mock<IAtencionRepository>();
            _PersonaRepositoryMock = new Mock<IPersonaRepository>();
            _EventAggregatorMock = new Mock<IEventAggregator>();
            _RegionManagerMock = new Mock<IRegionManager>();
            _Vm = new EditarAtencionesViewModel(_AtencionRepositoryMock.Object,
                _EventAggregatorMock.Object, _PersonaRepositoryMock.Object,
                _CitaRepositoryMock.Object,
                _RegionManagerMock.Object);
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.True(_Vm.EdicionHabilitada);
            Assert.NotNull(_Vm.Persona);
        }
    }
}
