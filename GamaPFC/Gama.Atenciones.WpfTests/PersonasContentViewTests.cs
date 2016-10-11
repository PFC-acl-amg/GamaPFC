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
    public class PersonasContentViewTests
    {
        private Mock<IEventAggregator> _EventAggregatorMock;
        private Mock<IRegionManager> _RegionManagerMock;
        PersonasContentViewModel _Vm;

        public PersonasContentViewTests()
        {
            _RegionManagerMock = new Mock<IRegionManager>();
            _EventAggregatorMock = new Mock<IEventAggregator>();
            _Vm = new PersonasContentViewModel(
                _EventAggregatorMock.Object,
                _RegionManagerMock.Object);
        }
    }
}
