using Gama.Socios.Wpf.ViewModels;
using Moq;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.Socios.WpfTests
{
    public class PanelSwitcherViewModelTests
    {
        private Mock<IRegionManager> _RegionManagerMock;
        PanelSwitcherViewModel _Vm;

        public PanelSwitcherViewModelTests()
        {
            _RegionManagerMock = new Mock<IRegionManager>();

            _Vm = new PanelSwitcherViewModel(_RegionManagerMock.Object);
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.NotNull(_Vm.NavigateCommand);
        }
    }
}
