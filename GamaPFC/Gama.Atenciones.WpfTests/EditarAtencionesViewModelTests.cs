using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.ViewModels;
using Moq;
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
        private EditarAtencionesViewModel _Vm;

        public EditarAtencionesViewModelTests()
        {
            _AtencionRepositoryMock = new Mock<IAtencionRepository>();
            _Vm = new EditarAtencionesViewModel(_AtencionRepositoryMock.Object);
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.True(_Vm.EdicionHabilitada);
            Assert.NotNull(_Vm.Persona);
        }
    }
}
