using Gama.Atenciones.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.Atenciones.WpfTests
{
    public class ToolbarViewModelTests
    {
        ToolbarViewModel _Vm;

        public ToolbarViewModelTests()
        {
            //_Vm = new ToolbarViewModel();
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.NotNull(_Vm.NuevaPersonaCommand);
            Assert.NotNull(_Vm.ExportarCommand);
        }
    }
}
