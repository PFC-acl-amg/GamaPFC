using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.ViewModels;
using Gama.Atenciones.Wpf.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gama.Atenciones.WpfTests
{
    public class PersonaViewModelTests
    {
        PersonaWrapper wrapper;
        PersonaViewModel _Vm;

        public PersonaViewModelTests()
        {
            _Vm = new PersonaViewModel();
        }

        [Fact]
        private void ShouldInitializeItsProperties()
        {
            Assert.False(_Vm.EdicionHabilitada);
            Assert.Null(_Vm.Persona.Nombre);
        }

        [Fact]
        private void LoadingANewPersonaShouldChangeViewModelsPersonaProperty()
        {
            wrapper = new PersonaWrapper(new Persona()
            {
                Id = 0,
                Nombre = "nombre"
            });

            _Vm.Load(wrapper);
            Assert.Equal(wrapper, _Vm.Persona);
        }
    }
}
