using Gama.Common.CustomControls;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.ViewModels;
using Gama.Cooperacion.Wpf.Wrappers;
using Moq;
using NHibernate;
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
    public class InformacionDeActividadViewModelTests
    {
        private InformacionDeActividadViewModel _vm;
        private List<Cooperante> _cooperantes;

        public InformacionDeActividadViewModelTests()
        {
            _cooperantes = new FakeCooperanteRepository().GetAll();

            var actividadRepositoryMock = new Mock<IActividadRepository>();
            var cooperanteRepositoryMock = new Mock<ICooperanteRepository>();
            var eventAggregatorMock = new Mock<IEventAggregator>();
            var sessionMock = new Mock<ISession>();

            cooperanteRepositoryMock.Setup(cr => cr.GetAll()).Returns(_cooperantes);

            //_vm = new InformacionDeActividadViewModel(
            //    actividadRepositoryMock.Object,
            //    cooperanteRepositoryMock.Object,
            //    eventAggregatorMock.Object
            //    //sessionMock.Object
            //    );
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.NotNull(_vm.Actividad);
            Assert.Empty(_vm.Actividad.Titulo);
            Assert.NotNull(_vm.CooperantesDisponibles != null);
            Assert.Equal(_vm.CooperantesDisponibles.Count, _cooperantes.Count);
            Assert.True(_vm.CooperantesDisponibles.All(c => _cooperantes.Contains(c.Model)));
            Assert.Equal(
                ((ObservableCollection<LookupItem>)_vm.ResultadoDeBusqueda).Count,
                                          _vm.CooperantesDisponibles.Count);
            Assert.True(
                ((ObservableCollection<LookupItem>)_vm.ResultadoDeBusqueda).All
                (lookupItem => _vm.CooperantesDisponibles.Any(cw => lookupItem.Id == cw.Id)));
            Assert.True(_vm.EdicionHabilitada);
            Assert.False(_vm.PopupEstaAbierto);
            Assert.Null(_vm.CooperanteBuscado);
            Assert.Null(_vm.CooperanteEmergenteSeleccionado);
            Assert.Null(_vm.CooperantePreviamenteSeleccionado);
            Assert.Equal(((List<string>)_vm.MensajeDeEspera)[0], "Espera por favor...");
            Assert.Equal(_vm.Actividad.Cooperantes.Count, 1); // Cooperante Dummy
            Assert.Equal(_vm.Actividad.Cooperantes.First().Id, 0);
            Assert.Null(_vm.Actividad.Cooperantes.First().Nombre);
            Assert.Equal(_vm.Actividad.Coordinador.Id, 0);
            Assert.Null(_vm.Actividad.Coordinador.Nombre);
        }

        [Fact]
        private void ShouldLoad()
        {
            var wrapper = new ActividadWrapper(
                new Actividad()
                {
                    Id = 1,
                    Titulo = "Título de la actividad",
                    Coordinador = _cooperantes.First(),
                });

            _vm.Load(wrapper);

            Assert.Equal(_vm.Actividad.Cooperantes.Count, 0); 
            //Assert.Equal(_vm.Actividad.Cooperantes.First().Id, 0);
            //Assert.Null(_vm.Actividad.Cooperantes.First().Nombre);
            Assert.Equal(_vm.Actividad.Coordinador.Id, _cooperantes.First().Id);
            Assert.NotNull(_vm.Actividad.Coordinador.Nombre);
        }

        [Fact]
        private void ShouldAddCoordinador()
        {
            _vm.CoordinadorBuscado = new LookupItem()
            {
                DisplayMember1 = string.Format("{0} {1}", _cooperantes[0].Nombre, _cooperantes[0].Apellido),
                DisplayMember2 = _cooperantes[0].Dni,
                Id = _cooperantes[0].Id
            };

            _vm.SelectCoordinadorCommand.Execute(null);
            Assert.Equal(_vm.CoordinadorBuscado.Id, _vm.Actividad.Coordinador.Id);
        }

        [Fact]
        private void ShouldNotifyWhenCoordinadorChanges()
        {
            bool fired = false;

            _vm.Actividad.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(Actividad.Coordinador))
                {
                    fired = true;
                }
            };

            _vm.Actividad.Coordinador = new CooperanteWrapper(_cooperantes[0]);
            Assert.True(fired);
        }

        [Fact]
        private void ShouldAddCoordinadoresEnOrdenArbitrario()
        {
            var coordinadorSeleccionado = _cooperantes[0];
            _vm.CooperanteEmergenteSeleccionado =
                _vm.CooperantesDisponibles.Single(cd => cd.Id == coordinadorSeleccionado.Id);
            _vm.AbrirPopupCommand.Execute(null); // Al pasar 'null' se indica que es un coordinador
            _vm.NuevoCooperanteCommand.Execute(null);
            Assert.Equal(coordinadorSeleccionado.Id, _vm.Actividad.Coordinador.Id);

            // Cuando se selecciona desde el SearchBox
            var nuevoCoordinadorSeleccionado = _cooperantes[1];
            _vm.CoordinadorBuscado = new LookupItem()
            {
                DisplayMember1 = string.Format("{0} {1}",
                    nuevoCoordinadorSeleccionado.Nombre,
                    nuevoCoordinadorSeleccionado.Apellido),
                DisplayMember2 = nuevoCoordinadorSeleccionado.Dni,
                Id = nuevoCoordinadorSeleccionado.Id
            };
            _vm.SelectCoordinadorCommand.Execute(null);
            Assert.Equal(_vm.CoordinadorBuscado.Id, _vm.Actividad.Coordinador.Id);

            // Volvemos a seleccionar uno desde el Popup emergente
            nuevoCoordinadorSeleccionado = _cooperantes[2];
            _vm.CooperanteEmergenteSeleccionado =
                _vm.CooperantesDisponibles.Where(cd => cd.Id == nuevoCoordinadorSeleccionado.Id).First();
            _vm.AbrirPopupCommand.Execute(null);
            _vm.NuevoCooperanteCommand.Execute(null);
            Assert.Equal(nuevoCoordinadorSeleccionado.Id, _vm.Actividad.Coordinador.Id);

            // Volvemos a seleccionar uno desde el SearchBox
            nuevoCoordinadorSeleccionado = _cooperantes[3];
            _vm.CoordinadorBuscado = new LookupItem()
            {
                DisplayMember1 = string.Format("{0} {1}",
                    nuevoCoordinadorSeleccionado.Nombre,
                    nuevoCoordinadorSeleccionado.Apellido),
                DisplayMember2 = nuevoCoordinadorSeleccionado.Dni,
                Id = nuevoCoordinadorSeleccionado.Id
            };
            _vm.SelectCoordinadorCommand.Execute(null);
            Assert.Equal(_vm.CoordinadorBuscado.Id, _vm.Actividad.Coordinador.Id);
        }

        [Fact]
        private void ShouldQuitarCoordinador()
        {
            Assert.False(_vm.QuitarCoordinadorCommand.CanExecute(null));
            var coordinadorInicial = new CooperanteWrapper(_cooperantes[0]);
            _vm.Actividad.Coordinador = coordinadorInicial;
            Assert.True(_vm.QuitarCoordinadorCommand.CanExecute(null));
            _vm.QuitarCoordinadorCommand.Execute(null);
            Assert.Null(_vm.Actividad.Coordinador.Nombre);
            Assert.False(_vm.QuitarCoordinadorCommand.CanExecute(null));
        }

        [Fact]
        private void ShouldAddCooperantes()
        {
            var cooperanteSeleccionado = _cooperantes[0];
            var cooperanteWrapper = new CooperanteWrapper(cooperanteSeleccionado);
            var cooperanteDummy = _vm.Actividad.Cooperantes.First();

            //
            // Adición del primer cooperante
            //
            Assert.Equal(_vm.CooperantesDisponibles.Count, _cooperantes.Count);

            _vm.CooperanteEmergenteSeleccionado =
                _vm.CooperantesDisponibles.Single(cd => cd.Id == cooperanteSeleccionado.Id);
            _vm.AbrirPopupCommand.Execute(cooperanteDummy);
            _vm.NuevoCooperanteCommand.Execute(null);

            var cooperanteNuevo = _vm.Actividad.Cooperantes.Single(c => c.Id == cooperanteSeleccionado.Id);
            Assert.Equal(cooperanteSeleccionado.Id, cooperanteNuevo.Id);
            Assert.Equal(_vm.CooperantesDisponibles.Count, _cooperantes.Count - 1);

            //
            // Nuevo cooperante desde añadido desde el SearchBox
            //
            var nuevoCooperanteSeleccionado = _cooperantes[1];
            _vm.CooperanteBuscado = new LookupItem()
            {
                DisplayMember1 = string.Format("{0} {1}",
                    nuevoCooperanteSeleccionado.Nombre,
                    nuevoCooperanteSeleccionado.Apellido),
                DisplayMember2 = nuevoCooperanteSeleccionado.Dni,
                Id = nuevoCooperanteSeleccionado.Id
            };
            _vm.SelectCooperanteEventCommand.Execute(cooperanteDummy);
            cooperanteNuevo = _vm.Actividad.Cooperantes.Single(c => c.Id == nuevoCooperanteSeleccionado.Id);
            Assert.Equal(_vm.CooperanteBuscado.Id, cooperanteNuevo.Id);
            Assert.Equal(_vm.CooperantesDisponibles.Count, _cooperantes.Count - 2);

            //
            // Volvemos a seleccionar uno desde el Popup emergente
            //
            nuevoCooperanteSeleccionado = _cooperantes[2];
            _vm.CooperanteEmergenteSeleccionado =
                _vm.CooperantesDisponibles.Single(cd => cd.Id == nuevoCooperanteSeleccionado.Id);
            _vm.AbrirPopupCommand.Execute(cooperanteDummy);
            _vm.NuevoCooperanteCommand.Execute(null);

            cooperanteNuevo = _vm.Actividad.Cooperantes.Single(c => c.Id == nuevoCooperanteSeleccionado.Id);
            Assert.NotNull(cooperanteNuevo);
            Assert.Equal(nuevoCooperanteSeleccionado.Id, cooperanteNuevo.Id);
            Assert.Equal(_vm.CooperantesDisponibles.Count, _cooperantes.Count - 3);
        }

        [Fact]
        private void ShouldQuitarCooperantes()
        {
            var cooperante0 = _vm.CooperantesDisponibles[0];
            var cooperante1 = _vm.CooperantesDisponibles[1];
            _vm.Actividad.Cooperantes.Add(cooperante0);
            _vm.Actividad.Cooperantes.Add(cooperante1);
            _vm.CooperantesDisponibles.Remove(cooperante0);
            _vm.CooperantesDisponibles.Remove(cooperante1);
            Assert.Equal(_vm.CooperantesDisponibles.Count, _cooperantes.Count - 2);
            Assert.Equal(_vm.Actividad.Cooperantes.Where(c => c.Nombre != null).ToList().Count, 2);

            _vm.QuitarCooperanteCommand.Execute(cooperante0);
            Assert.Equal(_vm.CooperantesDisponibles.Count, _cooperantes.Count - 1);
            Assert.Equal(_vm.Actividad.Cooperantes.Where(c => c.Nombre != null).ToList().Count, 1);

            _vm.QuitarCooperanteCommand.Execute(cooperante1);
            Assert.Equal(_vm.CooperantesDisponibles.Count, _cooperantes.Count);
            Assert.Equal(_vm.Actividad.Cooperantes.Where(c => c.Nombre != null).ToList().Count, 0);
        }
    }
}
