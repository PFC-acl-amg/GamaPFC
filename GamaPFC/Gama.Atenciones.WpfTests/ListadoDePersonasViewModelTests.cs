using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Eventos;
using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Wpf.Services;
using Gama.Atenciones.Wpf.ViewModels;
using Gama.Common.CustomControls;
using Moq;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.Atenciones.WpfTests
{
    class ListadoDePersonasViewModelTests
    {
        private ListadoDePersonasViewModel _Vm;
        private List<Persona> _Personas;
        private int _ItemsPerPage = 30;
        private Mock<ISession> _SessionMock;
        private Mock<IAtencionesSettings> _SettingsMock;
        private Mock<IPersonaRepository> _PersonaRepositoryMock;
        private EventAggregator _EventAggregatorMock;

        public ListadoDePersonasViewModelTests()
        {
            _Personas = new FakePersonaRepository().GetAll().Take(40).ToList();

            _EventAggregatorMock = new EventAggregator();
            _PersonaRepositoryMock = new Mock<IPersonaRepository>();
            _SettingsMock = new Mock<IAtencionesSettings>();
            _SessionMock = new Mock<ISession>();

            _SettingsMock.SetupProperty(uc => uc.ListadoDePersonasItemsPerPage, _ItemsPerPage);

            _PersonaRepositoryMock.Setup(pr => pr.GetAll())
                .Returns(_Personas);

            _Vm = new ListadoDePersonasViewModel(
                _EventAggregatorMock,
                _PersonaRepositoryMock.Object,
                _SettingsMock.Object,
                _SessionMock.Object);
        }

        [Fact]
        public void ShouldStartInPageOne()
        {
            Assert.True(_Vm.Personas.CurrentPage == 1);
        }

        [Fact]
        public void ShouldChangeInnerCollectionCurrentPageOnNextOrPreviousPageCommand()
        {
            Assert.True(_Vm.Personas.Count == _ItemsPerPage);

            _Vm.PaginaSiguienteCommand.Execute(null);
            Assert.True(_Vm.Personas.CurrentPage == 2);
            Assert.True(_Vm.Personas.Count == 10); // 40 - 30; Total - #ItemsPagina1(_itemsPerPage)

            _Vm.PaginaAnteriorCommand.Execute(null);
            Assert.True(_Vm.Personas.CurrentPage == 1);
            Assert.True(_Vm.Personas.Count == _ItemsPerPage); // De nuevo en la primera página
        }

        [Fact]
        public void ShouldChangeItemsShownWhenItemsPerPageChanges()
        {
            _Vm.ElementosPorPagina = 35;
            Assert.True(_Vm.Personas.Count == 35);

            _Vm.ElementosPorPagina = 50;
            Assert.True(_Vm.Personas.Count == 40); // 40 es el total
        }

        [Fact]
        public void ShouldRaisePropertyChangedOnElementosPorPaginaChanged()
        {
            var fired = false;

            _Vm.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(_Vm.ElementosPorPagina))
                {
                    fired = true;
                }
            };

            _Vm.ElementosPorPagina = 35;
            Assert.True(fired);
        }

        [Fact]
        public void ShouldUpdateInnerPersonaWhenItIsUpdated()
        {
            var persona = _Personas[3];
            persona.Nombre = "Otro nombre";

            _PersonaRepositoryMock.Setup(ar => ar.GetById(persona.Id))
                 .Returns(persona);

            _EventAggregatorMock.GetEvent<PersonaActualizadaEvent>().Publish(persona.Id);

            Assert.True(((List<LookupItem>)_Vm.Personas.SourceCollection)
                .Single(a => a.Id == persona.Id).DisplayMember1 == persona.Nombre);
        }
    }
}
