using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.ViewModels;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.Atenciones.WpfTests
{
    public class StatusBarViewModelTests 
    {
        private Mock<IEventAggregator> _EventAggregatorMock;
        private Mock<PersonaActualizadaEvent> _PersonaActualizadaEventMock;
        private StatusBarViewModel _Vm;

        public StatusBarViewModelTests()
        {
            _EventAggregatorMock = new Mock<IEventAggregator>();

            _PersonaActualizadaEventMock = new Mock<PersonaActualizadaEvent>();

            _EventAggregatorMock.Setup(e => e.GetEvent<PersonaActualizadaEvent>())
                .Returns(_PersonaActualizadaEventMock.Object);

            _Vm = new StatusBarViewModel(_EventAggregatorMock.Object);
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.Equal(_Vm.Mensaje, StatusBarViewModel.DefaultMensaje);
            Assert.False(_Vm.ActivarFondo);
        }

        [Fact]
        private void ShouldGiveFeedbackWhenUnaPersonaHasBeenUpdated()
        {
            var eventAggregator = new EventAggregator();

            var vm = new StatusBarViewModel(eventAggregator);

            eventAggregator.GetEvent<PersonaActualizadaEvent>().Publish(It.IsAny<int>());

            Assert.True(vm.ActivarFondo);
            Assert.NotEqual(vm.Mensaje, StatusBarViewModel.DefaultMensaje);
           
        }
    }
}
