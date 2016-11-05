using Gama.Atenciones.Wpf.ViewModels;
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
        private EditarAtencionesViewModel _Vm;

        public EditarAtencionesViewModelTests()
        {
            _Vm = new EditarAtencionesViewModel();
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.True(_Vm.EdicionHabilitada);
            Assert.NotNull(_Vm.Persona);
        }
    }
}
