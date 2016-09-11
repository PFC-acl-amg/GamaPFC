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
    public class ChangeTrackingSimpleProperty
    {
        private Actividad _Actividad;
        
        public ChangeTrackingSimpleProperty()
        {
            _Actividad = new Actividad()
            {
                Titulo = "Título del Primer Actividad"
            };
        }

        [Fact]
        public void ShouldStoreOriginalValue()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            Assert.Equal("Título del Primer Actividad", wrapper.TituloOriginalValue);

            wrapper.Titulo = "Nuevo título";
            Assert.Equal("Título del Primer Actividad", wrapper.TituloOriginalValue);
        }

        [Fact]
        public void ShouldSetIsChanged()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            Assert.False(wrapper.TituloIsChanged);
            Assert.False(wrapper.IsChanged);

            wrapper.Titulo = "Nuevo título";
            Assert.True(wrapper.TituloIsChanged);
            Assert.True(wrapper.IsChanged);

            wrapper.Titulo = "Título del Primer Actividad";
            Assert.False(wrapper.TituloIsChanged);
            Assert.False(wrapper.IsChanged);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForTituloIsChanged()
        {
            var wrapper = new ActividadWrapper(_Actividad);

            var fired = false;
            wrapper.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(wrapper.TituloIsChanged))
                {
                    fired = true;
                }
            };

            wrapper.Titulo = "Another title";
            Assert.True(fired);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForIsChanged()
        {
            var wrapper = new ActividadWrapper(_Actividad);

            var fired = false;
            wrapper.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(wrapper.IsChanged))
                {
                    fired = true;
                }
            };

            wrapper.Titulo = "Another title";
            Assert.True(fired);

            fired = false;
            wrapper.Titulo = "Título del Primer Actividad"; // Valor original
            Assert.True(fired);
        }

        [Fact]
        public void ShouldAcceptChanges()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            Assert.False(wrapper.IsChanged);
            wrapper.Titulo = "Nuevo título";
            Assert.Equal("Nuevo título", wrapper.Titulo);
            Assert.Equal("Título del Primer Actividad", wrapper.TituloOriginalValue);
            Assert.True(wrapper.TituloIsChanged);
            Assert.True(wrapper.IsChanged);

            wrapper.AcceptChanges();

            Assert.Equal("Nuevo título", wrapper.Titulo);
            Assert.Equal("Nuevo título", wrapper.TituloOriginalValue);
            Assert.False(wrapper.TituloIsChanged);
            Assert.False(wrapper.IsChanged);
        }

        [Fact]
        public void ShouldRejectChanges()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            Assert.False(wrapper.IsChanged);
            wrapper.Titulo = "Nuevo título";
            Assert.Equal("Nuevo título", wrapper.Titulo);
            Assert.Equal("Título del Primer Actividad", wrapper.TituloOriginalValue);
            Assert.True(wrapper.TituloIsChanged);
            Assert.True(wrapper.IsChanged);

            wrapper.RejectChanges();

            Assert.Equal("Título del Primer Actividad", wrapper.Titulo);
            Assert.Equal("Título del Primer Actividad", wrapper.TituloOriginalValue);
            Assert.False(wrapper.TituloIsChanged);
            Assert.False(wrapper.IsChanged);
        }
    }
}
