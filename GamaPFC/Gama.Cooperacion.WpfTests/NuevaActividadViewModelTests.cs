using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.ViewModels;
using Gama.Cooperacion.Wpf.Wrappers;
using Moq;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Prism.Commands;
using System.Threading;

namespace Gama.Cooperacion.WpfTests
{
    public class NuevaActividadViewModelTests
    {
        private List<Cooperante> _cooperantes;
        private NuevaActividadViewModel _vm;
        private Mock<IActividadRepository> _actividadRepositoryMock;
        private Mock<ICooperanteRepository> _cooperanteRepositoryMock;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private InformacionDeActividadViewModel _informacionDeActividadViewModelMock;
        private Mock<NuevaActividadEvent> _nuevaActividadEventMock;

        public NuevaActividadViewModelTests()
        {
            _actividadRepositoryMock = new Mock<IActividadRepository>();
            _cooperanteRepositoryMock = new Mock<ICooperanteRepository>();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _nuevaActividadEventMock = new Mock<NuevaActividadEvent>();
            var sessionMock = new Mock<ISession>();

            _eventAggregatorMock.Setup(ea => ea.GetEvent<NuevaActividadEvent>())
                .Returns(_nuevaActividadEventMock.Object);

            _cooperantes = new FakeCooperanteRepository().GetAll();
            _cooperanteRepositoryMock.Setup(cr => cr.GetAll()).Returns(_cooperantes);

            _informacionDeActividadViewModelMock = new InformacionDeActividadViewModel(
                _cooperanteRepositoryMock.Object,
                _eventAggregatorMock.Object,
                sessionMock.Object);

            _vm = new NuevaActividadViewModel(
                _actividadRepositoryMock.Object,
                _eventAggregatorMock.Object,
                _informacionDeActividadViewModelMock,
                sessionMock.Object);
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.Null(_vm.Cerrar);
            Assert.NotNull(_vm.ActividadVM);
            Assert.False(_vm.AceptarCommand.CanExecute(null));
            Assert.True(_vm.CancelarCommand.CanExecute(null));
        }

        [Fact]
        private void ShouldNotifyWhenCoordinadorChanges()
        {
            bool fired = false;

            _vm.Actividad.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(_vm.Actividad.Coordinador))
                {
                    fired = true;
                }
            };

            _vm.Actividad.Coordinador = new CooperanteWrapper(_cooperantes[0]);
            Assert.True(fired);
        }

        [Fact]
        private void ShouldRaiseRaiseCanExecuteChangedOnAceptarCommandWhenCoordinadorChanges()
        {
            bool fired = false;
            var resetEvent = new AutoResetEvent(false);
            
            _vm.AceptarCommand.CanExecuteChanged += (s, e) => {
                fired = true;
                resetEvent.Set();
            };

            _vm.ActividadVM.Actividad.Coordinador = new CooperanteWrapper(_cooperantes.First());
            resetEvent.WaitOne(500);

            Assert.True(fired);
        }
        
        [Fact]
        private void ShouldAcceptNuevaActividad()
        {
            Assert.False(_vm.AceptarCommand.CanExecute(null));
            _vm.ActividadVM.Actividad.Coordinador = new CooperanteWrapper(
                new Cooperante()
                {
                    Nombre = "Coordinador"
                });
            Assert.False(_vm.AceptarCommand.CanExecute(null));

            _vm.ActividadVM.Actividad.Titulo = "Título de la actividad";
            Assert.True(_vm.AceptarCommand.CanExecute(null));

            _vm.AceptarCommand.Execute(null);

            // Que no haya ningún cooperante dummy
            Assert.True(_vm.ActividadVM.Actividad.Cooperantes.All(c => c.Nombre != null));

            _actividadRepositoryMock.Verify(ar => ar.Create(_vm.ActividadVM.Actividad.Model), Times.Once);
            _nuevaActividadEventMock.Verify(e => e.Publish(It.IsAny<int>()), Times.Once);
            Assert.True(_vm.Cerrar);
        }

        [Fact]
        private void ShouldCancelar()
        {
            Assert.Null(_vm.Cerrar);
            _vm.CancelarCommand.Execute(null);
            Assert.True(_vm.Cerrar);
        }
    }
}
