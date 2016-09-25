using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.ViewModels;
using Gama.Cooperacion.Wpf.Views;
using Gama.Cooperacion.Wpf.Wrappers;
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

namespace Gama.Cooperacion.WpfTests
{
    public class EditarActividadViewModelTests
    {
        private ActividadWrapper _actividad;
        private List<Cooperante> _cooperantes;
        EditarActividadViewModel _vm;
        private Mock<IActividadRepository> _actividadRepositoryMock;
        private Mock<ICooperanteRepository> _cooperanteRepositoryMock;
        private Mock<IEventAggregator> _eventAggregatorMock;
        private InformacionDeActividadViewModel _informacionDeActividadViewModelMock;
        private Mock<ActividadActualizadaEvent> _actividadActualizadaEvent;

        public EditarActividadViewModelTests()
        {
            _actividadRepositoryMock = new Mock<IActividadRepository>();
            _cooperanteRepositoryMock = new Mock<ICooperanteRepository>();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _actividadActualizadaEvent = new Mock<ActividadActualizadaEvent>();
            var sessionMock = new Mock<ISession>();

            _eventAggregatorMock.Setup(ea => ea.GetEvent<ActividadActualizadaEvent>())
                .Returns(_actividadActualizadaEvent.Object);

            _cooperantes = new FakeCooperanteRepository().GetAll();
            _cooperanteRepositoryMock.Setup(cr => cr.GetAll()).Returns(_cooperantes);

            _actividad = new ActividadWrapper(
                new Actividad()
                {
                    Id = 1,
                    Titulo = "Título de la actividad",
                    Coordinador = _cooperantes.First(),
                });

            _actividadRepositoryMock.Setup(ar => ar.GetById(It.IsAny<int>()))
                .Returns(_actividad.Model);

            _informacionDeActividadViewModelMock = new InformacionDeActividadViewModel(
                _cooperanteRepositoryMock.Object,
                _eventAggregatorMock.Object,
                sessionMock.Object);

            _vm = new EditarActividadViewModel(
                _actividadRepositoryMock.Object,
                _cooperanteRepositoryMock.Object,
                _eventAggregatorMock.Object,
                _informacionDeActividadViewModelMock,
                sessionMock.Object);

            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Id", _actividad.Id);
            var uri = new Uri("test", UriKind.Relative);

            var navigationJournalMock = new Mock<IRegionNavigationJournal>();
            var navigationServiceMock = new Mock<IRegionNavigationService>();

            IRegion region = new Region();
            navigationServiceMock.SetupGet(n => n.Region).Returns(region);
            navigationServiceMock.SetupGet(x => x.Journal).Returns(navigationJournalMock.Object);

            var context = new NavigationContext(navigationServiceMock.Object, uri,
                navigationParameters);

            _vm.OnNavigatedTo(context);
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.NotNull(_vm.Actividad);
            Assert.NotNull(_vm.ActividadVM);
            Assert.NotNull(_vm.Title);
            Assert.NotEmpty(_vm.Title);
            Assert.False(_vm.GuardarInformacionCommand.CanExecute(null));
            Assert.True(_vm.HabilitarEdicionCommand.CanExecute(null));
            Assert.False(_vm.CancelarEdicionCommand.CanExecute(null));
        }

        [Fact]
        private void ShouldNotifyWhenActividadChanges()
        {
            bool fired = false;

            _vm.ActividadVM.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(_vm.ActividadVM.Actividad))
                {
                    fired = true;
                }
            };

            var actividad = new Actividad();

            _vm.ActividadVM.Actividad = new ActividadWrapper(actividad);
            Assert.True(fired);
        }


    }
}
