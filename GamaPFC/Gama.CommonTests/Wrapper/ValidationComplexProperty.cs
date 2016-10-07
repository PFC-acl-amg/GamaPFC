using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.FakeServices;
using Gama.Atenciones.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.CommonTests.Wrapper
{
    public class ValidationComplexProperty
    {
        private Atencion _Atencion;

        public ValidationComplexProperty()
        {
            _Atencion = new Atencion();
        }

        [Fact]
        private void ShouldSetIsValidOfRoot()
        {
            var wrapper = new AtencionWrapper(_Atencion);
            Assert.True(wrapper.IsValid);

            wrapper.DerivacionesPropuestas.Tipo = null;
            Assert.False(wrapper.IsValid);

            wrapper.DerivacionesPropuestas.Tipo = "algún tipo";
            Assert.True(wrapper.IsValid);
        }

        [Fact]
        private void ShouldSetIsValidOfRootAfterInitialization()
        {
            _Atencion.DerivacionesPropuestas.Tipo = null;
            var wrapper = new AtencionWrapper(_Atencion);
            Assert.False(wrapper.IsValid);

            wrapper.DerivacionesPropuestas.Tipo = "algún tipo";
            Assert.True(wrapper.IsValid);
        }

        [Fact]
        private void ShouldRaisePropertyChangedEventForIsValidOfRoot()
        {
            var fired = false;
            var wrapper = new AtencionWrapper(_Atencion);
            wrapper.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(wrapper.IsValid))
                {
                    fired = true;
                }
            };

            wrapper.DerivacionesPropuestas.Tipo = null;
            Assert.True(fired);

            fired = false;
            wrapper.DerivacionesPropuestas.Tipo = "algún tipo";
            Assert.True(fired);
        }
    }
}
