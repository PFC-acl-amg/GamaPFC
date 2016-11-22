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

namespace Gama.Atenciones.WpfTests
{
    public class DashboardViewModelTests
    {
        private List<Atencion> _Atenciones;
        private Mock<IAtencionRepository> _AtencionRepositoryMock;
        private Mock<AtencionSeleccionadaEvent> _AtencionSeleccionadaEventMock;
        private Mock<ICitaRepository> _CitaRepositoryMock;
        private List<Cita> _Citas;
        private Mock<CitaSeleccionadaEvent> _CitaSeleccionadaEventMock;
        private Mock<IEventAggregator> _EventAggregatorMock;
        private Mock<PersonaCreadaEvent> _NuevaPersonaEventMock;
        private Mock<IPersonaRepository> _PersonaRepositoryMock;
        private List<Persona> _Personas;
        private Mock<PersonaSeleccionadaEvent> _PersonaSeleccionadaEventMock;
        private Mock<ISession> _SessionMock;
        private AtencionesSettings _Settings;
        DashboardViewModel _Vm;
        private Mock<CitaCreadaEvent> _NuevaCitaEventMock;
        private Mock<NuevaAtencionEvent> _NuevaAtencionEventMock;

        public DashboardViewModelTests()
        {
            _PersonaRepositoryMock = new Mock<IPersonaRepository>();
            _CitaRepositoryMock = new Mock<ICitaRepository>();
            _AtencionRepositoryMock = new Mock<IAtencionRepository>();
            _EventAggregatorMock = new Mock<IEventAggregator>();
            _Settings = new AtencionesSettings();
            _SessionMock = new Mock<ISession>();

            _Personas = new FakePersonaRepository().GetAll();
            _Citas = new FakeCitaRepository().GetAll();
            _Atenciones = new FakeAtencionRepository().GetAll();

            _PersonaRepositoryMock.Setup(p => p.GetAll()).Returns(_Personas);
            _CitaRepositoryMock.Setup(c => c.GetAll()).Returns(_Citas);
            _AtencionRepositoryMock.Setup(a => a.GetAll()).Returns(_Atenciones);

            _PersonaSeleccionadaEventMock = new Mock<PersonaSeleccionadaEvent>();
            _CitaSeleccionadaEventMock = new Mock<CitaSeleccionadaEvent>();
            _AtencionSeleccionadaEventMock = new Mock<AtencionSeleccionadaEvent>();
            _NuevaPersonaEventMock = new Mock<PersonaCreadaEvent>();
            _NuevaCitaEventMock = new Mock<CitaCreadaEvent>();
            _NuevaAtencionEventMock = new Mock<NuevaAtencionEvent>();

            _EventAggregatorMock.Setup(e => e.GetEvent<PersonaSeleccionadaEvent>()).Returns(
                _PersonaSeleccionadaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<CitaSeleccionadaEvent>()).Returns(
                _CitaSeleccionadaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<AtencionSeleccionadaEvent>()).Returns(
                _AtencionSeleccionadaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<PersonaCreadaEvent>()).Returns(
                _NuevaPersonaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<CitaCreadaEvent>()).Returns(
                _NuevaCitaEventMock.Object);
            _EventAggregatorMock.Setup(e => e.GetEvent<NuevaAtencionEvent>()).Returns(
                _NuevaAtencionEventMock.Object);

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
            _PersonaRepositoryMock.Verify(p => p.GetAll(), Times.Once);
            _CitaRepositoryMock.Verify(c => c.GetAll(), Times.Once);
            _AtencionRepositoryMock.Verify(a => a.GetAll(), Times.Once);
            Assert.Equal(_Vm.UltimasPersonas.Count, _Settings.DashboardUltimasPersonas);
            Assert.Equal(_Vm.ProximasCitas.Count, _Settings.DashboardUltimasCitas);
            Assert.Equal(_Vm.UltimasAtenciones.Count, _Settings.DashboardUltimasAtenciones);
            Assert.NotNull(_Vm.SelectPersonaCommand);
            Assert.NotNull(_Vm.SelectCitaCommand);
            Assert.NotNull(_Vm.SelectAtencionCommand);
            Assert.NotNull(_Vm.PersonasNuevasPorMes);
            Assert.NotNull(_Vm.AtencionesNuevasPorMes);
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
            _AtencionSeleccionadaEventMock.Verify(e => e.Publish(1), Times.Once);
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

            Assert.Equal(persona.Id, vm.UltimasPersonas.First().Id);
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

            Assert.Equal(atencion.Id, vm.UltimasAtenciones.First().Id);
        }

        [Fact]
        private void NuevaCitaShouldSetLaCitaEnPrimeraPosicionDeLasProximasCitasMostradas()
        {
            var cita = new Cita { Id = int.MaxValue, Inicio = DateTime.Now.AddYears(-10), Sala = "Sala B"};
            _CitaRepositoryMock.Setup(p => p.GetById(It.IsAny<int>())).Returns(cita);

            var eventAggregator = new EventAggregator();

            var vm = new DashboardViewModel(
                _PersonaRepositoryMock.Object,
                _CitaRepositoryMock.Object,
                _AtencionRepositoryMock.Object,
                eventAggregator,
                _Settings,
                _SessionMock.Object);

            eventAggregator.GetEvent<CitaCreadaEvent>().Publish(cita.Id);
            Assert.Equal(cita.Id, vm.ProximasCitas.First().Id);
        }
    }
}
