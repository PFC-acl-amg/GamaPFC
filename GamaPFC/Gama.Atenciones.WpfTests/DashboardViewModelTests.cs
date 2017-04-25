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

            _Personas = new FakePersonaRepository().GetAll();
            _Citas = new FakeCitaRepository().GetAll();
            _Atenciones = new FakeAtencionRepository().GetAll();

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
            Assert.Null(_Vm.FechaDeInicio);
            Assert.Null(_Vm.FechaDeFin);

            _PersonaRepositoryMock.Verify(p => p.GetAll(), Times.Once);
            _CitaRepositoryMock.Verify(c => c.GetAll(), Times.Once);
            _AtencionRepositoryMock.Verify(a => a.GetAll(), Times.Once);

            Assert.NotNull(_Vm.SelectPersonaCommand);
            Assert.NotNull(_Vm.SelectCitaCommand);
            Assert.NotNull(_Vm.SelectAtencionCommand);
        }

        [Fact]
        private void ShouldPublishPersonaSeleccionadaEventWhenUnaPersonaIsSelected()
        {
            _Vm.SelectPersonaCommand.Execute(new LookupItem { Id = 1 });
            _PersonaSeleccionadaEventMock.Verify(e => e.Publish(1), Times.Once);
        }

        [Fact]
        private void ShouldPublishCitaSeleccionadaEventWhenUnaCitaIsSelected()
        {
            _Vm.SelectCitaCommand.Execute(new LookupItem { Id = 1 });
            _CitaSeleccionadaEventMock.Verify(e => e.Publish(1), Times.Once);
        }

        [Fact]
        private void ShouldPublishAtencionSeleccionadaEventWhenUnaAtencionIsSelected()
        {
            _Vm.SelectAtencionCommand.Execute(new LookupItem { Id = 1 });
            _AtencionSeleccionadaEventMock.Verify(e => e.Publish(
                new IdentificadorDeModelosPayload
                {
                    AtencionId = 1,
                    PersonaId = It.IsAny<int>(),
                     CitaId = It.IsAny<int?>(),
                }), Times.Once);
        }

        [Fact]
        private void NuevaPersonaShouldSetLaPersonaEnPrimeraPosicionDeLasUltimasPersonasMostradas()
        {
            var persona = new Persona { Id = int.MaxValue, Nombre = "Nombre", Nif = "" };
            _PersonaRepositoryMock.Setup(p => p.GetById(It.IsAny<int>())).Returns(persona);

            var eventAggregator = new EventAggregator();

            var vm = new DashboardViewModel(
                _PersonaRepositoryMock.Object,
                _CitaRepositoryMock.Object,
                _AtencionRepositoryMock.Object, 
                eventAggregator, 
                _Settings,
                _SessionMock.Object);

            eventAggregator.GetEvent<PersonaCreadaEvent>().Publish(persona.Id);

            Assert.Equal(persona.Id, vm.Personas.First().Id);
        }

        [Fact]
        private void NuevaAtencionShouldSetLaAtencionEnPrimeraPosicionDeLasUltimasAtencionesMostradas()
        {
            var atencion = new Atencion { Id = int.MaxValue, Fecha = DateTime.Now, Seguimiento = "Seguimiento" };
            _AtencionRepositoryMock.Setup(p => p.GetById(It.IsAny<int>())).Returns(atencion);

            var eventAggregator = new EventAggregator();

            var vm = new DashboardViewModel(
                _PersonaRepositoryMock.Object,
                _CitaRepositoryMock.Object,
                _AtencionRepositoryMock.Object,
                eventAggregator,
                _Settings,
                _SessionMock.Object);

            eventAggregator.GetEvent<AtencionCreadaEvent>().Publish(atencion.Id);

            Assert.Equal(atencion.Id, vm.Atenciones.First().Id);
        }

        [Fact]
        private void NuevaCitaShouldSetLaCitaEnPrimeraPosicionDeLasProximasCitasMostradas()
        {
            //
            // Arrange
            //
            var persona = new Persona
            {
                Id = int.MaxValue,
                Nombre = "Nombre",
                Nif = ""
            };

            _PersonaRepositoryMock.Setup(p => p.GetById(It.IsAny<int>())).Returns(persona);

            var cita = new Cita {
                Id = int.MaxValue,
                Fecha = DateTime.Now.AddYears(-10),
                Sala = "Sala B",
                Persona = persona,
            };

            _CitaRepositoryMock.Setup(p => p.GetById(It.IsAny<int>())).Returns(cita);

            var eventAggregator = new EventAggregator();

            var vm = new DashboardViewModel(
                _PersonaRepositoryMock.Object,
                _CitaRepositoryMock.Object,
                _AtencionRepositoryMock.Object,
                eventAggregator,
                _Settings,
                _SessionMock.Object);

            //
            // Act
            //
            eventAggregator.GetEvent<CitaCreadaEvent>().Publish(cita.Id);

            //
            // Assert
            //
            Assert.Equal(cita.Id, vm.ProximasCitas.First().Id);
        }
    }
}
