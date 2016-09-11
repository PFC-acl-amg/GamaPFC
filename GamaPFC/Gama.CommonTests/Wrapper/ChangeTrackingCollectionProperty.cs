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
    public class ChangeTrackingCollectionProperty
    {
        private Actividad _Actividad;
        private List<CooperanteWrapper> _Cooperantes;


        public ChangeTrackingCollectionProperty()
        {
            _Cooperantes = new List<CooperanteWrapper>()
            {
                new CooperanteWrapper(new Cooperante() { Nombre = "Cooperante 1" }),
                new CooperanteWrapper(new Cooperante() { Nombre = "Cooperante 2" }),
            };

            _Actividad = new Actividad();
        }

        [Fact]
        public void ShouldSetIsChangedOfActividadWrapper()
        {
            var wrapper = new ActividadWrapper(_Actividad);
            Assert.False(wrapper.IsChanged);

            wrapper.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
            Assert.True(wrapper.IsChanged);
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

            wrapper.Cooperantes.Add(new CooperanteWrapper(new Cooperante()));
            Assert.True(fired);
        }

        [Fact]
        public void ShouldAcceptChanges()
        {
            var wrapper = new ActividadWrapper(new Actividad()
            {
                Cooperantes = new List<Cooperante>(_Cooperantes.Select(cw => cw.Model)),
            });

            var colaboradorToRemove = wrapper.Cooperantes[0];
            var colaboradorToModify = wrapper.Cooperantes[1];

            var colaboradorToAdd1 = new CooperanteWrapper(new Cooperante());
            var colaboradorToAdd2 = new CooperanteWrapper(new Cooperante());
            wrapper.Cooperantes.Add(colaboradorToAdd1);
            wrapper.Cooperantes.Add(colaboradorToAdd2);
            wrapper.Cooperantes.Remove(colaboradorToRemove);
            colaboradorToModify.Nombre = "Otro Nombre";

            Assert.True(wrapper.IsChanged);
            Assert.Equal(3, wrapper.Cooperantes.Count);
            Assert.Equal(2, wrapper.Cooperantes.AddedItems.Count);
            Assert.Equal(1, wrapper.Cooperantes.RemovedItems.Count);
            Assert.Equal(1, wrapper.Cooperantes.ModifiedItems.Count);

            wrapper.AcceptChanges();

            Assert.False(wrapper.IsChanged);
            Assert.Equal(3, wrapper.Cooperantes.Count);
            Assert.Equal(0, wrapper.Cooperantes.AddedItems.Count);
            Assert.Equal(0, wrapper.Cooperantes.RemovedItems.Count);
            Assert.Equal(0, wrapper.Cooperantes.ModifiedItems.Count);
        }

        [Fact]
        public void ShouldRejectChanges()
        {
            var wrapper = new ActividadWrapper(new Actividad()
            {
                Cooperantes = new List<Cooperante>(_Cooperantes.Select(cw => cw.Model)),
            });

            var colaboradorToRemove = wrapper.Cooperantes[0];
            var colaboradorToModify = wrapper.Cooperantes[1];

            var colaboradorToAdd1 = new CooperanteWrapper(new Cooperante());
            var colaboradorToAdd2 = new CooperanteWrapper(new Cooperante());
            wrapper.Cooperantes.Add(colaboradorToAdd1);
            wrapper.Cooperantes.Add(colaboradorToAdd2);
            wrapper.Cooperantes.Remove(colaboradorToRemove);
            colaboradorToModify.Nombre = "Otro Nombre";

            Assert.True(wrapper.IsChanged);
            Assert.Equal(3, wrapper.Cooperantes.Count);
            Assert.Equal(2, wrapper.Cooperantes.AddedItems.Count);
            Assert.Equal(1, wrapper.Cooperantes.RemovedItems.Count);
            Assert.Equal(1, wrapper.Cooperantes.ModifiedItems.Count);

            wrapper.RejectChanges();

            Assert.False(wrapper.IsChanged);
            Assert.Equal(2, wrapper.Cooperantes.Count);
            Assert.Equal(0, wrapper.Cooperantes.AddedItems.Count);
            Assert.Equal(0, wrapper.Cooperantes.RemovedItems.Count);
            Assert.Equal(0, wrapper.Cooperantes.ModifiedItems.Count);
        }

    }
}
