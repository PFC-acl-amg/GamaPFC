using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Wrapper
{
    public class BasicTests
    {
        private Actividad _Actividad;
        
        public BasicTests()
        {
            _Actividad = new Actividad();
            _Actividad.Titulo = "Título de la primera actividad";
        }

        [Fact]
        public void ShouldContainModelInModelProperty()
        {
            var wrapper = new ActividadWrapper(_Actividad);

            Assert.Equal(_Actividad, wrapper.Model);
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionIfModelIsNull()
        {
            ArgumentNullException ex = 
                Assert.Throws<ArgumentNullException>(() => new ActividadWrapper(null));
            Assert.Equal("model", ex.ParamName);
        }

        [Fact]
        public void ShouldThrowArgumentExceptionIfCoordinadorIsNull()
        {
            _Actividad.Coordinador = null;
            ArgumentNullException ex = 
                Assert.Throws<ArgumentNullException>(() => new ActividadWrapper(_Actividad));
            Assert.Equal("Coordinador", ex.ParamName);
        }
       
        [Fact]
        public void ShouldGetValueOfUnderlyingModelProperty()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            Assert.Equal(_Actividad.Titulo, wrapper.Titulo);
        }

        [Fact]
        public void ShouldSetValueOfUnderlyingModelProperty()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            wrapper.Titulo = "My title";
            Assert.Equal(_Actividad.Titulo, wrapper.Titulo);
        }
    }
}

