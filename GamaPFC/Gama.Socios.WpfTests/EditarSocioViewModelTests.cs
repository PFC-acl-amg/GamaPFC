using Gama.Socios.Business;
using Gama.Socios.Wpf.Eventos;
using Gama.Socios.Wpf.Services;
using Gama.Socios.Wpf.ViewModels;
using Gama.Socios.Wpf.Wrappers;
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

namespace Gama.Socios.WpfTests
{
    public class EditarSocioViewModelTests
    {
        private Mock<IEventAggregator> _EventAggregatorMock;
        private SocioWrapper _Socio;
        private Mock<SocioActualizadoEvent> _SocioActualizadoEventMock;
        private Mock<ISocioRepository> _SocioRepositoryMock;
        private SocioViewModel _SocioViewModelMock;
        private EditarSocioViewModel _Vm;
        private Mock<ISession> _SessionMock;
        private EditarCuotasViewModel _EditarCuotasViewModelMock;
        private EditarPeriodosDeAltaViewModel _PeriodoDeAltaViewModelMock;
        private Mock<IRegionManager> _RegionManagerMock;

        public EditarSocioViewModelTests()
        {
            _SocioRepositoryMock = new Mock<ISocioRepository>();
            _EventAggregatorMock = new Mock<IEventAggregator>();
            _SocioActualizadoEventMock = new Mock<SocioActualizadoEvent>();
            _SessionMock = new Mock<ISession>();
            _RegionManagerMock = new Mock<IRegionManager>();

            _EventAggregatorMock.Setup(ea => ea.GetEvent<SocioActualizadoEvent>())
                .Returns(_SocioActualizadoEventMock.Object);

            _Socio = new SocioWrapper(
                new Socio()
                {
                    Id = 1,
                    Nombre = "Nombre de la Socio",
                    Nif = "0000000T",
                });

            _SocioRepositoryMock.Setup(ar => ar.GetById(It.IsAny<int>()))
                .Returns(_Socio.Model);

            _SocioViewModelMock = new SocioViewModel();
            _EditarCuotasViewModelMock = new EditarCuotasViewModel();
            //_PeriodoDeAltaViewModelMock = new EditarPeriodosDeAltaViewModel(
            //    _SocioRepositoryMock.Object,
            //    _EventAggregatorMock.Object,
            //    new PreferenciasDeSocios());

            //_Vm = new EditarSocioViewModel(
            //    _EventAggregatorMock.Object,
            //    _SocioRepositoryMock.Object,
            //    _SocioViewModelMock,
            //    _EditarCuotasViewModelMock,
            //    _PeriodoDeAltaViewModelMock,
            //    _SessionMock.Object);

            _Vm.OnNavigatedTo(_Socio.Id);
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.NotNull(_Vm.Socio);
            Assert.NotNull(_Vm.SocioVM);
            Assert.NotNull(_Vm.Title);
            Assert.NotEmpty(_Vm.Title);
            Assert.True(_Vm.Socio.Nombre.StartsWith(
                _Vm.Title.Substring(0, _Vm.Title.Length - "...".Length)));
            Assert.False(_Vm.ActualizarCommand.CanExecute(null));
            Assert.True(_Vm.HabilitarEdicionCommand.CanExecute(null));
            Assert.False(_Vm.CancelarEdicionCommand.CanExecute(null));
        }

        [Fact]
        private void ShouldNotifyWhenSocioChanges()
        {
            bool fired = false;

            _Vm.SocioVM.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(_Vm.SocioVM.Socio))
                {
                    fired = true;
                }
            };

            var Socio = new Socio();

            _Vm.SocioVM.Socio = new SocioWrapper(Socio);
            Assert.True(fired);
        }

        [Fact]
        private void ShoudEnableAndDisableCommandsOnEdicionHabilitada()
        {
            _Vm.HabilitarEdicionCommand.Execute(null);
            Assert.False(_Vm.ActualizarCommand.CanExecute(null));
            Assert.True(_Vm.CancelarEdicionCommand.CanExecute(null));
            Assert.False(_Vm.HabilitarEdicionCommand.CanExecute(null));

            _Vm.Socio.Nombre = "Otro nombre";
            Assert.True(_Vm.ActualizarCommand.CanExecute(null));
            Assert.True(_Vm.CancelarEdicionCommand.CanExecute(null));
            Assert.False(_Vm.HabilitarEdicionCommand.CanExecute(null));
        }

        [Fact]
        private void ShouldReturnSocioToOriginalStateIfEdicionIsCanceled()
        {
            _Vm.HabilitarEdicionCommand.Execute(null);
            _Vm.Socio.Nombre = "Otro nombre";
            Assert.True(_Vm.ActualizarCommand.CanExecute(null));
            Assert.True(_Vm.CancelarEdicionCommand.CanExecute(null));

            _Vm.CancelarEdicionCommand.Execute(null);

            Assert.Equal(_Vm.Socio.Nombre, "Nombre de la Socio");
            Assert.False(_Vm.ActualizarCommand.CanExecute(null));
            Assert.False(_Vm.CancelarEdicionCommand.CanExecute(null));
            Assert.True(_Vm.HabilitarEdicionCommand.CanExecute(null));
        }

        [Fact]
        private void ShouldReturnToOriginalStateAfterUpdating()
        {
            _Vm.HabilitarEdicionCommand.Execute(null);
            _Vm.Socio.Nombre = "Otro nombre";

            _Vm.ActualizarCommand.Execute(null);

            Assert.False(_Vm.ActualizarCommand.CanExecute(null));
            Assert.False(_Vm.CancelarEdicionCommand.CanExecute(null));
            Assert.True(_Vm.HabilitarEdicionCommand.CanExecute(null));
            Assert.Equal("Otro nombre", _Vm.Socio.Nombre);
            Assert.False(_Vm.Socio.IsChanged);
        }

        [Fact]
        private void ShouldPublishSocioActualizadoEventAfterUpdating()
        {
            _Vm.HabilitarEdicionCommand.Execute(null);
            _Vm.Socio.Nombre = "Otro nombre";

            _Vm.ActualizarCommand.Execute(null);
            _SocioActualizadoEventMock.Verify(e => e.Publish(new Socio { Id = _Vm.Socio.Id }), Times.Once);
        }
    }
}
