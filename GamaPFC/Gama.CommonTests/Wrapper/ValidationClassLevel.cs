using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Services;
using Gama.Cooperacion.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.CommonTests.Wrapper
{
    public class ValidationClassLevel
    {
        private Actividad _Actividad;

        public ValidationClassLevel()
        {
            _Actividad = new Actividad()
            {
                Titulo = "Algún título",
                Descripcion  = "Alguna descripción"
            };
        }

        [Fact]
        private void ShouldHaveErrorsAndNotBeValidWhenNoCoordinadorExists()
        {
            var expectedError = "La actividad debe tener un coordinador";
            var wrapper = new ActividadWrapper(_Actividad);
            Assert.False(wrapper.IsValid);

            var errors = wrapper.GetErrors(nameof(wrapper.Coordinador)).Cast<string>().ToList();
            Assert.Equal(1, errors.Count);
            Assert.Equal(expectedError, errors.Single());  
        }

        [Fact]
        private void ShouldBeValidAgainWhenACoordinadorIsSet()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            Assert.False(wrapper.IsValid);

            wrapper.Coordinador = new CooperanteWrapper(new FakeCooperanteRepository().GetAll().First());
            Assert.True(wrapper.IsValid);
        }
    }
}
