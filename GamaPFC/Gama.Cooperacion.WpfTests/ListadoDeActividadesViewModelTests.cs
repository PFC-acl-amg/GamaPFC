using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.ViewModels;
using Moq;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.Cooperacion.WpfTests
{
    public class ListadoDeActividadesViewModelTests
    {
        private ListadoDeActividadesViewModel _vm;
        private List<Actividad> _actividades;
        private int _itemsPerPage = 30;
        private Mock<ISession> _sessionMock;
       // private Mock<ICooperacionSettings> _userConfigMock;
        private Mock<IActividadRepository> _activdadRepositoryMock;
        private EventAggregator _eventAggregatorMock;

        public ListadoDeActividadesViewModelTests()
        {
            _actividades = new FakeActividadRepository().GetAll().Take(40).ToList();

            _eventAggregatorMock = new EventAggregator();
            _activdadRepositoryMock = new Mock<IActividadRepository>();
            //_userConfigMock = new Mock<ICooperacionSettings>();
            _sessionMock = new Mock<ISession>();

            //_userConfigMock.SetupProperty(uc => uc.ListadoDeActividadesItemsPerPage, _itemsPerPage);

            _activdadRepositoryMock.Setup(pr => pr.GetAll())
                .Returns(_actividades);

            //_vm = new ListadoDeActividadesViewModel(
            //    _eventAggregatorMock,
            //    _activdadRepositoryMock.Object,
            //    _userConfigMock.Object,
            //    _sessionMock.Object);
        }

        [Fact]
        public void ShouldStartInPageOne()
        {
            Assert.True(_vm.Actividades.CurrentPage == 1);
        }

        [Fact]
        public void ShouldChangeInnerCollectionCurrentPageOnNextOrPreviousPageCommand()
        {
            Assert.True(_vm.Actividades.Count == _itemsPerPage);

            _vm.PaginaSiguienteCommand.Execute(null);
            Assert.True(_vm.Actividades.CurrentPage == 2);
            Assert.True(_vm.Actividades.Count == 10); // 40 - 30; Total - #ItemsPagina1(_itemsPerPage)

            _vm.PaginaAnteriorCommand.Execute(null);
            Assert.True(_vm.Actividades.CurrentPage == 1);
            Assert.True(_vm.Actividades.Count == _itemsPerPage); // De nuevo en la primera página
        }

        [Fact]
        public void ShouldChangeItemsShownWhenItemsPerPageChanges()
        {
            _vm.ElementosPorPagina = 35;
            Assert.True(_vm.Actividades.Count == 35);

            _vm.ElementosPorPagina = 50;
            Assert.True(_vm.Actividades.Count == 40); // 40 es el total
        }

        [Fact]
        public void ShouldRaisePropertyChangedOnElementosPorPaginaChanged()
        {
            var fired = false;

            _vm.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(_vm.ElementosPorPagina))
                {
                    fired = true;
                }
            };

            _vm.ElementosPorPagina = 35;
            Assert.True(fired);
        }

        [Fact]
        public void ShouldUpdateInnerActividadWhenItIsUpdated()
        {
            var actividad = _actividades[3];
            actividad.Titulo = "Otro título";

            _activdadRepositoryMock.Setup(ar => ar.GetById(actividad.Id))
                 .Returns(actividad);

            _eventAggregatorMock.GetEvent<ActividadActualizadaEvent>().Publish(actividad.Id);

            Assert.True(((List<LookupItem>)_vm.Actividades.SourceCollection)
                .Single(a => a.Id == actividad.Id).DisplayMember1 == actividad.Titulo);
        }
    }
}
