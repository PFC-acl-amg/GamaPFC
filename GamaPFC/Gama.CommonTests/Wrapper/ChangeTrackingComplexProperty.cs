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
    public class ChangeTrackingComplexProperty
    {
        private Actividad _Actividad;
        
        public ChangeTrackingComplexProperty()
        {
            _Actividad = new Actividad()
            {
                Coordinador = new Cooperante() { Nombre = "Cooperante Name" }
            };
        }

        [Fact]
        public void ShouldSetIsChangedOfActividadWrapper()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            wrapper.Coordinador.Nombre = "Otro nombre";
            Assert.True(wrapper.IsChanged);

            wrapper.Coordinador.Nombre = "Cooperante Name";
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

            wrapper.Coordinador.Nombre = "Otro nombre";
            Assert.True(fired);
        }

        [Fact]
        public void ShouldAcceptChanges()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            wrapper.Coordinador.Nombre = "Otro nombre";
            Assert.Equal("Cooperante Name", wrapper.Coordinador.NombreOriginalValue);

            wrapper.AcceptChanges();

            Assert.False(wrapper.IsChanged);
            Assert.Equal("Otro nombre", wrapper.Coordinador.Nombre);
            Assert.Equal("Otro nombre", wrapper.Coordinador.NombreOriginalValue);
        }

        [Fact]
        public void ShouldRejectChanges()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            wrapper.Coordinador.Nombre = "Otro nombre";
            Assert.Equal("Cooperante Name", wrapper.Coordinador.NombreOriginalValue);

            wrapper.RejectChanges();

            Assert.False(wrapper.IsChanged);
            Assert.Equal("Cooperante Name", wrapper.Coordinador.Nombre);
            Assert.Equal("Cooperante Name", wrapper.Coordinador.NombreOriginalValue);
        }
    }
}
