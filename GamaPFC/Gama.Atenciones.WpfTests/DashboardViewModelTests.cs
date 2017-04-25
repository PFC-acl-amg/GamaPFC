using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.ViewModels;
using Moq;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Gama.Atenciones.Business;
using Gama.Common.CustomControls;
using Gama.Atenciones.Wpf.Eventos;
using System.IO.Packaging;
using Gama.Atenciones.Wpf.UIEvents;

namespace Gama.Atenciones.WpfTests
{
    public class DashboardViewModelTests : BaseTestClass
    {
        private List<Persona> _Personas;
        private List<Cita> _Citas;
        private List<Atencion> _Atenciones;
        private Mock<IAtencionRepository> _AtencionRepositoryMock;
        private Mock<ICitaRepository> _CitaRepositoryMock;
        private Mock<IEventAggregator> _EventAggregatorMock;
        private Mock<IPersonaRepository> _PersonaRepositoryMock;
        private Mock<ISession> _SessionMock;
        private PreferenciasDeAtenciones _Settings;
        DashboardViewModel _Vm;

        private Mock<PersonaCreadaEvent> _PersonaCreadaEventMock;
        private Mock<PersonaActualizadaEvent> _PersonaActualizadaEventMock;
        private Mock<PersonaEliminadaEvent> _PersonaEliminadaEventMock;
        private Mock<PersonaEnBusquedaEvent> _PersonaEnBusquedaEventMock;
        private Mock<PersonaSeleccionadaEvent> _PersonaSeleccionadaEventMock;

        private Mock<CitaCreadaEvent> _CitaCreadaEventMock;
        private Mock<CitaActualizadaEvent> _CitaActualizadaEventMock;
        private Mock<CitaEliminadaEvent> _CitaEliminadaEventMock;
        private Mock<CitaSeleccionadaEvent> _CitaSeleccionadaEventMock;

        private Mock<AtencionCreadaEvent> _AtencionCreadaEventMock;
        private Mock<AtencionActualizadaEvent> _AtencionActualizadaEventMock;
        private Mock<AtencionEliminadaEvent> _AtencionEliminadaEventMock;
        private Mock<AtencionSeleccionadaEvent> _AtencionSeleccionadaEventMock;

        public DashboardViewModelTests()
        {
            _PersonaRepositoryMock = new Mock<IPersonaRepository>();
            _CitaRepositoryMock = new Mock<ICitaRepository>();
            _AtencionRepositoryMock = new Mock<IAtencionRepository>();
            _EventAggregatorMock = new Mock<IEventAggregator>();
            _Settings = new PreferenciasDeAtenciones();
            _SessionMock = new Mock<ISession>();

            var fakeRepository = new FakeRepository();

            _Personas = fakeRepository.Personas;
            _Citas = fakeRepository.Citas;
            _Atenciones = fakeRepository.Atenciones;

            _PersonaRepositoryMock.Setup(p => p.GetAll()).Returns(_Personas);
            _CitaRepositoryMock.Setup(c => c.GetAll()).Returns(_Citas);
            _AtencionRepositoryMock.Setup(a => a.GetAll()).Returns(_Atenciones);

            _PersonaCreadaEventMock = new Mock<PersonaCreadaEvent>();
            _PersonaActualizadaEventMock = new Mock<PersonaActualizadaEvent>();
            _PersonaEliminadaEventMock = new Mock<PersonaEliminadaEvent>();
            _PersonaEnBusquedaEventMock = new Mock<PersonaEnBusquedaEvent>();
            _PersonaSeleccionadaEventMock = new Mock<PersonaSeleccionadaEvent>();

            _CitaCreadaEventMock = new Mock<CitaCreadaEvent>();
            _CitaActualizadaEventMock = new Mock<CitaActualizadaEvent>();
            _CitaEliminadaEventMock = new Mock<CitaEliminadaEvent>();
            _CitaSeleccionadaEventMock = new Mock<CitaSeleccionadaEvent>();

            _AtencionCreadaEventMock = new Mock<AtencionCreadaEvent>();
            _AtencionActualizadaEventMock = new Mock<AtencionActualizadaEvent>();
            _AtencionEliminadaEventMock = new Mock<AtencionEliminadaEvent>();
            _AtencionSeleccionadaEventMock = new Mock<AtencionSeleccionadaEvent>();

            _EventAggregatorMock.Setup(e => e.GetEvent<PersonaCreadaEvent>()).Returns(_PersonaCreadaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<PersonaActualizadaEvent>()).Returns(_PersonaActualizadaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<PersonaEliminadaEvent>()).Returns(_PersonaEliminadaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<PersonaSeleccionadaEvent>()).Returns(_PersonaSeleccionadaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<PersonaEnBusquedaEvent>()).Returns(_PersonaEnBusquedaEventMock.Object);

            _EventAggregatorMock.Setup(e => e.GetEvent<CitaCreadaEvent>()).Returns(_CitaCreadaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<CitaActualizadaEvent>()).Returns(_CitaActualizadaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<CitaEliminadaEvent>()).Returns(_CitaEliminadaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<CitaSeleccionadaEvent>()).Returns(_CitaSeleccionadaEventMock.Object);
     
            _EventAggregatorMock.Setup(e => e.GetEvent<AtencionCreadaEvent>()).Returns(_AtencionCreadaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<AtencionActualizadaEvent>()).Returns(_AtencionActualizadaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<AtencionEliminadaEvent>()).Returns(_AtencionEliminadaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<AtencionSeleccionadaEvent>()).Returns(_AtencionSeleccionadaEventMock.Object);
     
            _Vm = new DashboardViewModel(
                personaRepository: _PersonaRepositoryMock.Object,
                citaRepository:_CitaRepositoryMock.Object,
                atencionRepository:_AtencionRepositoryMock.Object,
                eventAggregator:_EventAggregatorMock.Object,
                settings: _Settings,
                session:_SessionMock.Object);
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            /// 
            /// Assert
            ///

            Assert.Null(_Vm.FechaDeInicio);
            Assert.Null(_Vm.FechaDeFin);
            Assert.NotNull(_Vm.SelectPersonaCommand);
            Assert.NotNull(_Vm.SelectCitaCommand);
            Assert.NotNull(_Vm.SelectAtencionCommand);

            _PersonaRepositoryMock.Verify(p => p.GetAll(), Times.Once);
            _CitaRepositoryMock.Verify(c => c.GetAll(), Times.Once);
            _AtencionRepositoryMock.Verify(a => a.GetAll(), Times.Once);
        }

        /// 
        ///  TESTS DE LOS COMANDOS
        /// 

        [Fact]
        private void ShouldFiltrarPorPersonaSeleccionadaYSoloMostrarAtencionesYCitasSuyas()
        {
            ///
            /// Arrange
            /// 
            int expectedId = 1;
            int expectedCount = 1;

            ///
            /// Act
            /// 
            _Vm.FiltrarPorPersonaCommand.Execute(_Personas.Find(x => x.Id == expectedId));

            /// 
            /// Assert
            /// 
            Assert.Equal(_Vm.Personas.Count, expectedCount);
            Assert.Equal(_Vm.Personas.First().Id, expectedId);
            Assert.True(_Vm.ProximasCitas.All(x => x.Persona.Id == expectedId));
            Assert.True(_Vm.Atenciones.All(x => x.Cita.Persona.Id == expectedId));

            // Si se vuelve a ejecutar ese comando, desactivará el filtro, por lo que 
            // se debe volver al estado inicial. Pasamos null como parámetro para que de
            // error en caso de meterse por donde no debe. En este caso, al volverse
            // al estado inicial, no debe accederse a ese parámetro.

            ///
            /// Arrange
            ///
            expectedCount = _Personas.Count;

            ///
            /// Act
            /// 
            _Vm.FiltrarPorPersonaCommand.Execute(null);

            /// 
            /// Assert
            /// 
            Assert.Equal(_Vm.Personas.Count, expectedCount);
        }


        [Fact]
        private void ShouldPublishPersonaSeleccionadaEventWhenUnaPersonaIsSelected()
        {
            /// Arrange
            Persona persona = _Personas.First();

            /// Act
            _Vm.SelectPersonaCommand.Execute(persona);

            /// Assert
            _PersonaSeleccionadaEventMock.Verify(e => e.Publish(persona.Id), Times.Once);
        }

        [Fact]
        private void ShouldPublishCitaSeleccionadaEventWhenUnaCitaIsSelected()
        {
            /// Arrange
            Cita cita = _Citas.First();

            /// Act
            _Vm.SelectCitaCommand.Execute(cita);

            /// Assert
            _CitaSeleccionadaEventMock.Verify(e => e.Publish(cita.Persona.Id), Times.Once);
        }

        [Fact]
        private void ShouldPublishAtencionSeleccionadaEventWhenUnaAtencionIsSelected()
        {
            /// Arrange
            Atencion atencion = _Atenciones.First();

            /// Act
            _Vm.SelectAtencionCommand.Execute(atencion);

            /// Assert
            _AtencionSeleccionadaEventMock.Verify(e => e.Publish(It.IsAny<IdentificadorDeModelosPayload>()),
                Times.Once);
        }

        [Fact]
        private void NuevaPersonaShouldAddLaPersona()
        {
            ///
            /// Arrange
            ///
            var persona = new Persona { Id = int.MaxValue };
            _PersonaRepositoryMock.Setup(p => p.GetById(persona.Id)).Returns(persona);

            var eventAggregator = new EventAggregator();

            var vm = new DashboardViewModel(
                _PersonaRepositoryMock.Object,
                _CitaRepositoryMock.Object,
                _AtencionRepositoryMock.Object, 
                eventAggregator, 
                _Settings,
                _SessionMock.Object);

            int expectedCount = _Personas.Count + 1;

            /// 
            /// Act
            /// 
            eventAggregator.GetEvent<PersonaCreadaEvent>().Publish(persona.Id);

            ///
            /// Assert
            /// 
            Assert.Equal(vm.Personas.Count, expectedCount);
            Assert.True(vm.Personas.Where(x => x.Id == persona.Id).ToList().Count == 1);
        }

        [Fact]
        private void NuevaAtencionShouldAddLaAtencion()
        {
            ///
            /// Arrange
            ///
            var atencion = new Atencion { Id = int.MaxValue };
            _AtencionRepositoryMock.Setup(p => p.GetById(atencion.Id)).Returns(atencion);

            var eventAggregator = new EventAggregator();

            var vm = new DashboardViewModel(
                _PersonaRepositoryMock.Object,
                _CitaRepositoryMock.Object,
                _AtencionRepositoryMock.Object,
                eventAggregator,
                _Settings,
                _SessionMock.Object);

            int expectedCount = _Atenciones.Count + 1;

            /// 
            /// Act
            /// 
            eventAggregator.GetEvent<AtencionCreadaEvent>().Publish(atencion.Id);

            ///
            /// Assert
            /// 
            Assert.Equal(vm.Atenciones.Count, expectedCount);
            Assert.True(vm.Atenciones.Where(x => x.Id == atencion.Id).ToList().Count == 1);
        }

        [Fact]
        private void NuevaCitaShouldAddLaCita()
        {
            //
            // Arrange
            //
            var cita = new Cita { Id = int.MaxValue };

            _CitaRepositoryMock.Setup(p => p.GetById(It.IsAny<int>())).Returns(cita);

            var eventAggregator = new EventAggregator();

            var vm = new DashboardViewModel(
                _PersonaRepositoryMock.Object,
                _CitaRepositoryMock.Object,
                _AtencionRepositoryMock.Object,
                eventAggregator,
                _Settings,
                _SessionMock.Object);

            int expectedCount = vm.ProximasCitas.Count + 1;

            //
            // Act
            //
            eventAggregator.GetEvent<CitaCreadaEvent>().Publish(cita.Id);

            //
            // Assert
            //
            Assert.Equal(expectedCount, vm.ProximasCitas.Count);
            Assert.True(vm.ProximasCitas.Where(x => x.Id == cita.Id).ToList().Count == 1);
        }
    }
}
