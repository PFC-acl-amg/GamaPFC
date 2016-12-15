using Gama.Socios.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.Socios.BusinessTests
{
    public class SocioTests
    {
        private Socio _Socio;

        public SocioTests()
        {
            _Socio = new Socio();
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.NotNull(_Socio.PeriodosDeAlta);
        }

    }
}
