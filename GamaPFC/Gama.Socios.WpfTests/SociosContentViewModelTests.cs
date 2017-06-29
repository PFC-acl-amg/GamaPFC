using Gama.Socios.Wpf.Eventos;
using Gama.Socios.Wpf.ViewModels;
using Moq;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.Socios.WpfTests
{
    public class SociosContentViewModelTests
    {
        private Mock<IEventAggregator> _EventAggregatorMock;
        private Mock<IRegionManager> _RegionManagerMock;
        SociosContentViewModel _Vm;
        //private Mock<SocioCreadoEvent> _SocioCreadoEventMock;
        //private Mock<SocioSeleccionadoEvent> _SocioSeleccionadoEventMock;

        public SociosContentViewModelTests()
        {
            _RegionManagerMock = new Mock<IRegionManager>();
            _EventAggregatorMock = new Mock<IEventAggregator>();

            //_Vm = new SociosContentViewModel(
            //    new EventAggregator(),
            //    _RegionManagerMock.Object);
        }

        [Fact]
        private void ShoulRequestNavigateWhenASocioIsSelected()
        {
            var eventAggregator = new EventAggregator();

            //var vm = new SociosContentViewModel(eventAggregator, _RegionManagerMock.Object);

            //eventAggregator.GetEvent<SocioSeleccionadoEvent>().Publish(3);

            //_RegionManagerMock.Verify(x => x.RequestNavigate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NavigationParameters>()), Times.Exactly(2));
        }
    }
}
