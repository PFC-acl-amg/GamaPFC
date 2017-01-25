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
        private Mock<IEventoRepository> _EventosRepositoryMock;
        private Mock<IForoRepository> _ForoRepositoryMock;
        private Mock<ITareaRepository> _TareaRepositoryMock;
        private TareasDeActividadViewModel _TareaDeActividadViewModelMock;

        public EditarActividadViewModelTests()
        {
            _actividadRepositoryMock = new Mock<IActividadRepository>();
            _cooperanteRepositoryMock = new Mock<ICooperanteRepository>();
            _EventosRepositoryMock = new Mock<IEventoRepository>();
            _ForoRepositoryMock = new Mock<IForoRepository>();
            _TareaRepositoryMock = new Mock<ITareaRepository>();
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
                _actividadRepositoryMock.Object,
                _cooperanteRepositoryMock.Object,
                _eventAggregatorMock.Object
                //sessionMock.Object
                );

            _TareaDeActividadViewModelMock = new TareasDeActividadViewModel(
               _actividadRepositoryMock.Object,
               _cooperanteRepositoryMock.Object,
               _ForoRepositoryMock.Object,
               _TareaRepositoryMock.Object,
               _eventAggregatorMock.Object
               //sessionMock.Object
                );


            _vm = new EditarActividadViewModel(
                _actividadRepositoryMock.Object,
                _cooperanteRepositoryMock.Object,
                _eventAggregatorMock.Object,
                _informacionDeActividadViewModelMock,
                _TareaDeActividadViewModelMock,
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
            Assert.NotNull(_vm.InformacionDeActividadViewModel);
            Assert.NotNull(_vm.Title);
            Assert.NotEmpty(_vm.Title);
            Assert.False(_vm.ActualizarCommand.CanExecute(null));
            Assert.True(_vm.HabilitarEdicionCommand.CanExecute(null));
            Assert.False(_vm.CancelarEdicionCommand.CanExecute(null));
        }

        [Fact]
        private void ShouldNotifyWhenActividadChanges()
        {
            bool fired = false;

            _vm.InformacionDeActividadViewModel.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(_vm.InformacionDeActividadViewModel.Actividad))
                {
                    fired = true;
                }
            };

            var actividad = new Actividad();

            _vm.InformacionDeActividadViewModel.Actividad = new ActividadWrapper(actividad);
            Assert.True(fired);
        }

        [Fact]
        private void ShoudEnableAndDisableCommandsOnEdicionHabilitada()
        {
            _vm.HabilitarEdicionCommand.Execute(null);
            Assert.False(_vm.ActualizarCommand.CanExecute(null));
            Assert.True(_vm.CancelarEdicionCommand.CanExecute(null));
            Assert.False(_vm.HabilitarEdicionCommand.CanExecute(null));

            _vm.Actividad.Titulo = "Otro título";
            Assert.True(_vm.ActualizarCommand.CanExecute(null));
            Assert.True(_vm.CancelarEdicionCommand.CanExecute(null));
            Assert.False(_vm.HabilitarEdicionCommand.CanExecute(null));
        }

        [Fact]
        private void ShouldReturnActividadToOriginalStateIfEdicionIsCanceled()
        {
            _vm.HabilitarEdicionCommand.Execute(null);
            _vm.Actividad.Titulo = "Otro título";
            _vm.Actividad.Coordinador = new CooperanteWrapper(new Cooperante());
            Assert.True(_vm.ActualizarCommand.CanExecute(null));
            Assert.True(_vm.CancelarEdicionCommand.CanExecute(null));

            _vm.CancelarEdicionCommand.Execute(null);

            Assert.Equal(_vm.Actividad.Titulo, "Título de la actividad");
            Assert.Equal(_vm.Actividad.Coordinador.Id, _cooperantes.First().Id);
            Assert.Equal(_vm.Actividad.Coordinador.Nombre, _cooperantes.First().Nombre);
            Assert.False(_vm.ActualizarCommand.CanExecute(null));
            Assert.False(_vm.CancelarEdicionCommand.CanExecute(null));
            Assert.True(_vm.HabilitarEdicionCommand.CanExecute(null));
        }

        [Fact]
        private void ShouldReturnToOriginalStateAfterUpdating()
        {
            _vm.HabilitarEdicionCommand.Execute(null);
            _vm.Actividad.Titulo = "Otro título";
            _vm.Actividad.Coordinador = new CooperanteWrapper(new Cooperante());

            _vm.ActualizarCommand.Execute(null);

            Assert.False(_vm.ActualizarCommand.CanExecute(null));
            Assert.False(_vm.CancelarEdicionCommand.CanExecute(null));
            Assert.True(_vm.HabilitarEdicionCommand.CanExecute(null));
            Assert.Equal("Otro título", _vm.Actividad.Titulo);
            Assert.False(_vm.Actividad.IsChanged);
        }

        [Fact]
        private void ShouldDevolverCooperantesAEstadoInicialAlCancelar()
        {
            _vm.HabilitarEdicionCommand.Execute(null);
            _vm.Actividad.Cooperantes.Add(new CooperanteWrapper(_cooperantes[0]));
            _vm.Actividad.Cooperantes.Add(new CooperanteWrapper(_cooperantes[1]));

            _vm.ActualizarCommand.Execute(null);

            Assert.Equal(2, _vm.Actividad.Cooperantes.Count);
            Assert.True(_vm.Actividad.Cooperantes.All(c => c.Nombre != null));
            Assert.False(_vm.Actividad.IsChanged);

            _vm.HabilitarEdicionCommand.Execute(null);
            _vm.Actividad.Cooperantes.RemoveAt(0);

            _vm.ActualizarCommand.Execute(null);

            Assert.Equal(1, _vm.Actividad.Cooperantes.Count);
            Assert.True(_vm.Actividad.Cooperantes.All(c => c.Nombre != null));
            Assert.False(_vm.Actividad.IsChanged);

            _vm.HabilitarEdicionCommand.Execute(null);
            _vm.Actividad.Cooperantes.RemoveAt(0);
            _vm.Actividad.Cooperantes.Add(new CooperanteWrapper(_cooperantes[3]));
            _vm.Actividad.Cooperantes.Add(new CooperanteWrapper(_cooperantes[4]));

            _vm.CancelarEdicionCommand.Execute(null);

            Assert.Equal(1, _vm.Actividad.Cooperantes.Count);
            Assert.True(_vm.Actividad.Cooperantes.All(c => c.Nombre != null));
            Assert.False(_vm.Actividad.IsChanged);
        }

        [Fact]
        private void ShouldPublishActividadActualizadaEventAfterUpdating()
        {
            _vm.HabilitarEdicionCommand.Execute(null);
            _vm.Actividad.Titulo = "Otro título";

            _vm.ActualizarCommand.Execute(null);
            _actividadActualizadaEvent.Verify(e => e.Publish(1), Times.Once);
        }
    }
}
