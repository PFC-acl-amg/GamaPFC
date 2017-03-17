using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Socios.Wpf.ViewModels;
using Gama.Socios.Business;
using NHibernate;
using Gama.Socios.Wpf.Services;
using Prism.Events;
using Moq;
using Gama.Socios.Wpf.FakeServices;
using Xunit;
using Gama.Socios.Wpf.Eventos;

namespace Gama.Socios.WpfTests
{
    public class ListadoDeSociosViewModelTest
    {
        private ListadoDeSociosViewModel _Vm;
        private List<Socio> _Socios;
        private int _ItemsPerPage = 30;
        private Mock<ISession> _SessionMock;
        private Mock<ISocioRepository> _SocioRepositoryMock;
        private Mock<IEventAggregator> _EventAggregatorMock;
        private Mock<IPreferenciasDeSocios> _SettingsMock;
        private Mock<SocioActualizadoEvent> _SocioActualizadoEventMock;
        private Mock<SocioCreadoEvent> _SocioCreadoEventMock;
        private Mock<SocioSeleccionadoEvent> _SocioSeleccionadoEventMock;

        public ListadoDeSociosViewModelTest()
        {
            _Socios = new FakeSocioRepository().GetAll().Take(40).ToList();

            _EventAggregatorMock = new Mock<IEventAggregator>();
            _SocioRepositoryMock = new Mock<ISocioRepository>();
            _SettingsMock = new Mock<IPreferenciasDeSocios>();
            _SessionMock = new Mock<ISession>();

            _SocioCreadoEventMock = new Mock<SocioCreadoEvent>();
            _SocioActualizadoEventMock = new Mock<SocioActualizadoEvent>();
            _SocioSeleccionadoEventMock = new Mock<SocioSeleccionadoEvent>();

            _EventAggregatorMock.Setup(x => x.GetEvent<SocioCreadoEvent>()).Returns(
                _SocioCreadoEventMock.Object);
            _EventAggregatorMock.Setup(x => x.GetEvent<SocioActualizadoEvent>()).Returns(
                _SocioActualizadoEventMock.Object);
            _EventAggregatorMock.Setup(x => x.GetEvent<SocioSeleccionadoEvent>()).Returns(
                _SocioSeleccionadoEventMock.Object);

            _SettingsMock.SetupProperty(uc => uc.ListadoDeSociosItemsPerPage, _ItemsPerPage);

            _SocioRepositoryMock.Setup(pr => pr.GetAll())
                .Returns(_Socios);

            _Vm = new ListadoDeSociosViewModel(
                _SocioRepositoryMock.Object,
                _EventAggregatorMock.Object,
                _SettingsMock.Object,
                _SessionMock.Object);
        }

        [Fact]
        public void ShouldStartInPageOne()
        {
            Assert.True(_Vm.Socios.CurrentPage == 1);
        }

        [Fact]
        private void ShouldPublishSocioSeleccionadoEventWhenASocioIsSelected()
        {
            _Vm.SeleccionarSocioCommand.Execute(new Socio { Id = 1 });
            _SocioSeleccionadoEventMock.Verify(x => x.Publish(1), Times.Once);
        }

        [Fact]
        public void ShouldChangeInnerCollectionCurrentPageOnNextOrPreviousPageCommand()
        {
            Assert.True(_Vm.Socios.Count == _ItemsPerPage);

            _Vm.PaginaSiguienteCommand.Execute(null);
            Assert.True(_Vm.Socios.CurrentPage == 2);
            Assert.True(_Vm.Socios.Count == 10); // 40 - 30; Total - #ItemsPagina1(_itemsPerPage)

            _Vm.PaginaAnteriorCommand.Execute(null);
            Assert.True(_Vm.Socios.CurrentPage == 1);
            Assert.True(_Vm.Socios.Count == _ItemsPerPage); // De nuevo en la primera página
        }

        [Fact]
        public void ShouldChangeItemsShownWhenItemsPerPageChanges()
        {
            _Vm.ElementosPorPagina = 35;
            Assert.True(_Vm.Socios.Count == 35);

            _Vm.ElementosPorPagina = 50;
            Assert.True(_Vm.Socios.Count == 40); // 40 es el total
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
        public void ShouldUpdateInnerSocioWhenItIsUpdated()
        {
            var socio = _Socios[3];
            socio.Nombre = "Otro nombre";

            _SocioRepositoryMock.Setup(ar => ar.GetById(socio.Id))
                 .Returns(socio);

            var eventAggregator = new EventAggregator();
            var vm = new ListadoDeSociosViewModel(_SocioRepositoryMock.Object,
                eventAggregator, _SettingsMock.Object, _SessionMock.Object);

            eventAggregator.GetEvent<SocioActualizadoEvent>().Publish(socio);

            Assert.True(((List<Socio>)_Vm.Socios.SourceCollection)
                .Single(x => x.Id == socio.Id).Nombre == socio.Nombre);
        }
    }
}