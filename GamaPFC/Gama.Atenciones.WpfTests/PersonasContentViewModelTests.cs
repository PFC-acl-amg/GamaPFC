using Gama.Atenciones.Wpf.ViewModels;
using Microsoft.Practices.Unity;
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
    public class PersonasContentViewModelTests
    {
        private Mock<IUnityContainer> _ContainerMock;
        private Mock<IEventAggregator> _EventAggregatorMock;
        private Mock<IRegionManager> _RegionManagerMock;
        PersonasContentViewModel _Vm;

        public PersonasContentViewModelTests()
        {
            _RegionManagerMock = new Mock<IRegionManager>();
            _EventAggregatorMock = new Mock<IEventAggregator>();
            _ContainerMock = new Mock<IUnityContainer>();
            //_Vm = new PersonasContentViewModel(
            //    null, 
            //    _EventAggregatorMock.Object,
            //    _RegionManagerMock.Object, 
            //    new Mock<IUnityContainer>().Object,
            //    null);
        }
    }
}
