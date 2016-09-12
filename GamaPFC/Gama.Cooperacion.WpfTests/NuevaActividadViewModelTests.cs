﻿using Gama.Common.CustomControls;
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
            Assert.Null(_vm.Cerrar);
            Assert.Null(_vm.SelectedCooperante);
            Assert.Null(_vm.CooperanteEmergenteSeleccionado);
            Assert.Null(_vm.CooperantePreviamenteSeleccionado);
            Assert.Equal(((List<string>)_vm.MensajeDeEspera)[0], "Espera por favor...");
            Assert.Equal(_vm.Actividad.Cooperantes.Count, 1);
            Assert.Equal(_vm.Actividad.Cooperantes.First().Id, 0);
            Assert.Null(_vm.Actividad.Cooperantes.First().Nombre);
            Assert.Equal(_vm.Actividad.Coordinador.Id, 0);
            Assert.Null(_vm.Actividad.Coordinador.Nombre);
        }

        [Fact]
        private void ShouldAddCoordinador()
        {
            _vm.CoordinadorSeleccionado = new LookupItem()
            {
                DisplayMember1 = string.Format("{0} {1}", _cooperantes[0].Nombre, _cooperantes[0].Apellido),
                DisplayMember2 = _cooperantes[0].Dni,
                Id = _cooperantes[0].Id
            };

            _vm.SelectCoordinadorCommand.Execute(null);
            Assert.Equal(_vm.CoordinadorSeleccionado.Id, _vm.Actividad.Coordinador.Id);

        }

        [Fact]
        private void ShouldAddCoordinadoresEnOrdenAleatorio()
        {
            _vm.CooperanteEmergenteSeleccionado = 
                _vm.CooperantesDisponibles.Where(cd => cd.Id == _cooperantes[0].Id).First();
            _vm.AbrirPopupCommand.Execute(null);
            _vm.NuevoCooperanteCommand.Execute(null);
            Assert.Equal(_cooperantes[0].Id, _vm.Actividad.Coordinador.Id);


            _vm.CoordinadorSeleccionado = new LookupItem()
            {
                DisplayMember1 = string.Format("{0} {1}", _cooperantes[1].Nombre, _cooperantes[1].Apellido),
                DisplayMember2 = _cooperantes[1].Dni,
                Id = _cooperantes[1].Id
            };
            _vm.SelectCoordinadorCommand.Execute(null);
            Assert.Equal(_vm.CoordinadorSeleccionado.Id, _vm.Actividad.Coordinador.Id);


            _vm.CooperanteEmergenteSeleccionado =
                _vm.CooperantesDisponibles.Where(cd => cd.Id == _cooperantes[2].Id).First();
            _vm.AbrirPopupCommand.Execute(null);
            _vm.NuevoCooperanteCommand.Execute(null);
            Assert.Equal(_cooperantes[2].Id, _vm.Actividad.Coordinador.Id);
        }
    }
}
