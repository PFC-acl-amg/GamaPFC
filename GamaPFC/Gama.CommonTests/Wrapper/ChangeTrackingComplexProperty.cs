using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.CommonTests.Wrapper
{
    public class ChangeTrackingComplexCustomProperty
    {
        private Actividad _Actividad;
        private Cooperante _CoordinadorInicial;
        private Cooperante _CoordinadorNuevo;
        
        public ChangeTrackingComplexCustomProperty()
        {
            _CoordinadorInicial = new Cooperante { Id = 1 };
            _CoordinadorNuevo = new Cooperante { Id = 2 };

            _Actividad = new Actividad()
            {
                Coordinador = _CoordinadorInicial,
            };
        }

        [Fact]
        public void ShouldSetIsChangedOfActividadWrapper()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            wrapper.Coordinador = new CooperanteWrapper(_CoordinadorNuevo);
            Assert.True(wrapper.IsChanged);

            wrapper.Coordinador = new CooperanteWrapper(_CoordinadorInicial);
            Assert.False(wrapper.IsChanged);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForIsChangedPropertyOfActividadWrapper()
        {
            var fired = false;
            var wrapper = new ActividadWrapper(_Actividad);
            wrapper.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(wrapper.IsChanged))
                {
                    fired = true;
                }
            };

            wrapper.Coordinador = new CooperanteWrapper(_CoordinadorNuevo);
            Assert.True(fired);
        }

        [Fact]
        public void ShouldAcceptChanges()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            wrapper.Coordinador = new CooperanteWrapper(_CoordinadorNuevo);
            Assert.Equal(_CoordinadorInicial.Id, wrapper.CoordinadorOriginalValue.Id);

            wrapper.AcceptChanges();

            Assert.False(wrapper.IsChanged);
            Assert.Equal(_CoordinadorNuevo.Id, wrapper.Coordinador.Id);
            Assert.Equal(_CoordinadorNuevo.Id, wrapper.CoordinadorOriginalValue.Id);
        }

        [Fact]
        public void ShouldRejectChanges()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            wrapper.Coordinador = new CooperanteWrapper(_CoordinadorNuevo);
            Assert.Equal(_CoordinadorInicial.Id, wrapper.CoordinadorOriginalValue.Id);

            wrapper.RejectChanges();

            Assert.False(wrapper.IsChanged);
            Assert.Equal(_CoordinadorInicial.Id, wrapper.Coordinador.Id);
            Assert.Equal(_CoordinadorInicial.Id, wrapper.CoordinadorOriginalValue.Id);
        }
    }
}
