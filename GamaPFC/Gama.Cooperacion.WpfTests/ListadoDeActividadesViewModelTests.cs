using Gama.Cooperacion.Business;
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

        public ListadoDeActividadesViewModelTests()
        {
            _actividades = new List<Actividad> { // 4 x 10 elementos
                new Actividad(), new Actividad(), new Actividad(), new Actividad(),
                new Actividad(), new Actividad(), new Actividad(), new Actividad(),
                new Actividad(), new Actividad(), new Actividad(), new Actividad(),
                new Actividad(), new Actividad(), new Actividad(), new Actividad(),
                new Actividad(), new Actividad(), new Actividad(), new Actividad(),
                new Actividad(), new Actividad(), new Actividad(), new Actividad(),
                new Actividad(), new Actividad(), new Actividad(), new Actividad(),
                new Actividad(), new Actividad(), new Actividad(), new Actividad(),
                new Actividad(), new Actividad(), new Actividad(), new Actividad(),
                new Actividad(), new Actividad(), new Actividad(), new Actividad(),};

            var eventAggregatorMock = new Mock<IEventAggregator>();
            var activdadRepositoryMock = new Mock<IActividadRepository>();
            var userConfigMock = new Mock<ICooperacionUserConfiguration>();
            var sessionMock = new Mock<ISession>();

            userConfigMock.SetupProperty(uc => uc.ListadoDeActividadesItemsPerPage, _itemsPerPage);

            activdadRepositoryMock.Setup(pr => pr.GetAll())
                .Returns(_actividades);

            _vm = new ListadoDeActividadesViewModel(
                eventAggregatorMock.Object,
                activdadRepositoryMock.Object,
                userConfigMock.Object);
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
    }
}
