using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.ViewModels;
using Gama.Cooperacion.Wpf.Wrappers;
using Moq;
using Prism.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.Cooperacion.WpfTests
{
    public class NuevaActividadViewModelTests
    {
        private NuevaActividadViewModel _vm;
        private List<Cooperante> _cooperantes;

        public NuevaActividadViewModelTests()
        { 
            _cooperantes = new FakeCooperanteRepository().GetAll();

            var actividadRepositoryMock = new Mock<IActividadRepository>();
            var cooperanteRepositoryMock = new Mock<ICooperanteRepository>();
            var eventAggregatorMock = new Mock<IEventAggregator>();

            cooperanteRepositoryMock.Setup(cr => cr.GetAll()).Returns(_cooperantes);

            _vm = new NuevaActividadViewModel(
                actividadRepositoryMock.Object,
                cooperanteRepositoryMock.Object,
                eventAggregatorMock.Object);
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.NotNull(_vm.Actividad);
            Assert.Null(_vm.Actividad.Titulo);
            Assert.NotNull(_vm.CooperantesDisponibles != null);
            Assert.Equal(_vm.CooperantesDisponibles.Count, _cooperantes.Count);
            Assert.True(_vm.CooperantesDisponibles.All(c => _cooperantes.Contains(c.Model)));
            Assert.Equal(
                ((ObservableCollection<LookupItem>)_vm.ResultadoDeBusqueda).Count,
                                          _vm.CooperantesDisponibles.Count);
            Assert.True(
                ((ObservableCollection<LookupItem>)_vm.ResultadoDeBusqueda).All
                (lookupItem => _vm.CooperantesDisponibles.Any(cw => lookupItem.Id == cw.Id)));
            Assert.False(_vm.PopupEstaAbierto);
            Assert.False(_vm.Cerrar);
            Assert.Null(_vm.SelectedCooperante);
            Assert.Null(_vm.CooperanteSeleccionado);
            Assert.Null(_vm.CooperantePreviamenteSeleccionado);
            Assert.Equal(((List<string>)_vm.MensajeDeEspera)[0], "Espera por favor...");
            Assert.Equal(_vm.Actividad.Cooperantes.Count, 1);
            Assert.Null(_vm.Actividad.Cooperantes.First().Id);
            Assert.Null(_vm.Actividad.Cooperantes.First().Nombre);
            Assert.Null(_vm.Actividad.Coordinador);
        }
    }
}
