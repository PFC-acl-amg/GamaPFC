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
    public class ChangeNotificationComplexProperty
    {
        private Actividad _Actividad;

        public ChangeNotificationComplexProperty()
        {
            _Actividad = new Actividad()
            {
                Titulo = "Título del Primer Actividad"
            };
        }

        [Fact]
        public void ShouldInitializeCoordinadorProperty()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            Assert.NotNull(wrapper.Coordinador);
            Assert.Equal(_Actividad.Coordinador, wrapper.Coordinador.Model);
        }
    }
}
