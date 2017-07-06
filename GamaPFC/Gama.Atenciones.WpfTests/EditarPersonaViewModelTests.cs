using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.ViewModels;
using Gama.Atenciones.Wpf.Wrappers;
using Moq;
using NHibernate;
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
    public class EditarPersonaViewModelTests 
    {
        private Mock<IEventAggregator> _EventAggregatorMock;
        private PersonaWrapper _Persona;
        private Mock<PersonaActualizadaEvent> _PersonaActualizadaEventMock;
        private Mock<IPersonaRepository> _PersonaRepositoryMock;
        private PersonaViewModel _PersonaViewModelMock;
        private EditarPersonaViewModel _Vm;
        private Mock<ISession> _SessionMock;
        private Mock<IAtencionRepository> _AtencionRepositoryMock;
        private EditarAtencionesViewModel _AtencionViewModelMock;
        private Mock<ICitaRepository> _CitaRepositoryMock;
        private EditarCitasViewModel _CitaViewModelMock;
        private Mock<IRegionManager> _RegionManagerMock;

        public EditarPersonaViewModelTests()
        {
            _PersonaRepositoryMock = new Mock<IPersonaRepository>();
            _AtencionRepositoryMock = new Mock<IAtencionRepository>();
            _CitaRepositoryMock = new Mock<ICitaRepository>();
            _EventAggregatorMock = new Mock<IEventAggregator>();
            _PersonaActualizadaEventMock = new Mock<PersonaActualizadaEvent>();
            _SessionMock = new Mock<ISession>();
            _RegionManagerMock = new Mock<IRegionManager>();

            _EventAggregatorMock.Setup(ea => ea.GetEvent<PersonaActualizadaEvent>())
                .Returns(_PersonaActualizadaEventMock.Object);

            _Persona = new PersonaWrapper(
                new Persona()
                {
                    Id = 1,
                    Nombre = "Nombre de la persona",
                    Nif = "0000000T",
                });

            _PersonaRepositoryMock.Setup(ar => ar.GetById(It.IsAny<int>()))
                .Returns(_Persona.Model);

            //_PersonaViewModelMock = new PersonaViewModel();
            //_AtencionViewModelMock = new EditarAtencionesViewModel(_AtencionRepositoryMock.Object,
            //    _EventAggregatorMock.Object, _PersonaRepositoryMock.Object,
            //    _CitaRepositoryMock.Object,_RegionManagerMock.Object);
            //_CitaViewModelMock = new EditarCitasViewModel(_CitaRepositoryMock.Object, _EventAggregatorMock.Object);

            //_Vm = new EditarPersonaViewModel(
            //    _EventAggregatorMock.Object,
            //    _PersonaRepositoryMock.Object,
            //    _PersonaViewModelMock,
            //    _AtencionViewModelMock,
            //    _CitaViewModelMock,
            //    _SessionMock.Object);

            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Id", _Persona.Id);
            var uri = new Uri("test", UriKind.Relative);
            var navigationJournalMock = new Mock<IRegionNavigationJournal>();
            var navigationServiceMock = new Mock<IRegionNavigationService>();
            IRegion region = new Region();
            navigationServiceMock.SetupGet(n => n.Region).Returns(region);
            navigationServiceMock.SetupGet(x => x.Journal).Returns(navigationJournalMock.Object);

            var context = new NavigationContext(navigationServiceMock.Object, uri, navigationParameters);
            //_Vm.OnNavigatedTo(context);
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            //Assert.NotNull(_Vm.Persona);
            //Assert.NotNull(_Vm.PersonaVM);
            //Assert.NotNull(_Vm.Title);
            //Assert.NotEmpty(_Vm.Title);
            //Assert.True(_Vm.Persona.Nombre.StartsWith(
            //    _Vm.Title.Substring(0, _Vm.Title.Length - "...".Length)));
            Assert.False(_Vm.ActualizarCommand.CanExecute(null));
            Assert.True(_Vm.HabilitarEdicionCommand.CanExecute(null));
            Assert.False(_Vm.CancelarEdicionCommand.CanExecute(null));
        }

        [Fact]
        private void ShouldNotifyWhenPersonaChanges()
        {
            bool fired = false;

            _Vm.PersonaVM.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(_Vm.PersonaVM.Persona))
                {
                    fired = true;
                }
            };

            var persona = new Persona();

            _Vm.PersonaVM.Persona = new PersonaWrapper(persona);
            Assert.True(fired);
        }

        [Fact]
        private void ShoudEnableAndDisableCommandsOnEdicionHabilitada()
        {
            _Vm.HabilitarEdicionCommand.Execute(null);
            Assert.False(_Vm.ActualizarCommand.CanExecute(null));
            Assert.True(_Vm.CancelarEdicionCommand.CanExecute(null));
            Assert.False(_Vm.HabilitarEdicionCommand.CanExecute(null));

            //_Vm.Persona.Nombre = "Otro nombre";
            Assert.True(_Vm.ActualizarCommand.CanExecute(null));
            Assert.True(_Vm.CancelarEdicionCommand.CanExecute(null));
            Assert.False(_Vm.HabilitarEdicionCommand.CanExecute(null));
        }

        [Fact]
        private void ShouldReturnPersonaToOriginalStateIfEdicionIsCanceled()
        {
            _Vm.HabilitarEdicionCommand.Execute(null);
            //_Vm.Persona.Nombre = "Otro nombre";
            Assert.True(_Vm.ActualizarCommand.CanExecute(null));
            Assert.True(_Vm.CancelarEdicionCommand.CanExecute(null));

            _Vm.CancelarEdicionCommand.Execute(null);

            //Assert.Equal(_Vm.Persona.Nombre, "Nombre de la persona");
            Assert.False(_Vm.ActualizarCommand.CanExecute(null));
            Assert.False(_Vm.CancelarEdicionCommand.CanExecute(null));
            Assert.True(_Vm.HabilitarEdicionCommand.CanExecute(null));
        }

        [Fact]
        private void ShouldReturnToOriginalStateAfterUpdating()
        {
            _Vm.HabilitarEdicionCommand.Execute(null);
            //_Vm.Persona.Nombre = "Otro nombre";

            _Vm.ActualizarCommand.Execute(null);

            Assert.False(_Vm.ActualizarCommand.CanExecute(null));
            Assert.False(_Vm.CancelarEdicionCommand.CanExecute(null));
            Assert.True(_Vm.HabilitarEdicionCommand.CanExecute(null));
            //Assert.Equal("Otro nombre", _Vm.Persona.Nombre);
            //Assert.False(_Vm.Persona.IsChanged);
        }

        [Fact]
        private void ShouldPublishPersonaActualizadaEventAfterUpdating()
        {
            _Vm.HabilitarEdicionCommand.Execute(null);
            //_Vm.Persona.Nombre = "Otro nombre";

            _Vm.ActualizarCommand.Execute(null);
            _PersonaActualizadaEventMock.Verify(e => e.Publish(1), Times.Once);
        }
    }
}
