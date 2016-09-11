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
    public class ChangeNotificationSimpleProperty
    {
        private Actividad _Actividad;

        public ChangeNotificationSimpleProperty()
        {
            _Actividad = new Actividad()
            {
                Titulo = "Título del Primer Actividad"
            };
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventOnPropertyChanged()
        {
            var wrapper = new ActividadWrapper(_Actividad);

            var fired = false;
            wrapper.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(wrapper.Titulo))
                {
                    fired = true;
                }
            };

            wrapper.Titulo = "Another title";
            Assert.True(fired);
        }

        [Fact]
        public void ShouldNotRaisePropertyChangedEventIfPropertyIsSetToSameValue()
        {
            var wrapper = new ActividadWrapper(_Actividad);

            var fired = false;
            wrapper.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(wrapper.Titulo))
                {
                    fired = true;
                }
            };

            wrapper.Titulo = "Título del Primer Actividad";
            Assert.False(fired);
        }
    }
}
