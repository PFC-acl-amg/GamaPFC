﻿using Gama.Atenciones.Business;
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
    public class ValidationSimpleProperty
    {
        private Persona _Persona;

        public ValidationSimpleProperty()
        {
            _Persona = new FakePersonaRepository().GetAll().First();
        }

        [Fact]
        private void ShouldReturnValidationErrorIfNombreIsEmpty()
        {
            var wrapper = new PersonaWrapper(_Persona);
            Assert.False(wrapper.HasErrors);

            wrapper.Nombre = "";
            Assert.True(wrapper.HasErrors);

            var errors = wrapper.GetErrors(nameof(wrapper.Nombre)).Cast<string>();
            Assert.Equal(1, errors.Count());
            Assert.Equal("El campo de nombre es obligatorio", errors.First());

            wrapper.Nombre = "Algún nombre";
            Assert.False(wrapper.HasErrors);
        }

        [Fact]
        private void ShouldRaiseErrorsChangedEventWhenNombreIsSetToEmpty()
        {
            var fired = false;
            var wrapper = new PersonaWrapper(_Persona);

            wrapper.ErrorsChanged += (s, e) => {
                if (e.PropertyName == nameof(wrapper.Nombre))
                {
                    fired = true;
                }
            };

            wrapper.Nombre = "";
            Assert.True(fired);

            fired = false;
            wrapper.Nombre = "Algún nombre";
            Assert.True(fired);
        }

        [Fact]
        private void ShouldSetIsValid()
        {
            var wrapper = new PersonaWrapper(_Persona);
            Assert.True(wrapper.IsValid);

            wrapper.Nombre = "";
            Assert.False(wrapper.IsValid);

            wrapper.Nombre = "Algún nombre";
            Assert.True(wrapper.IsValid);
        }

        [Fact]
        private void ShouldRaisePropertyChangedEventForIsValid()
        {
            var fired = false;
            var wrapper = new PersonaWrapper(_Persona);

            wrapper.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(wrapper.IsValid))
                {
                    fired = true;
                }
            };

            wrapper.Nombre = "";
            Assert.True(fired);

            fired = false;
            wrapper.Nombre = "Algún nombre";
            Assert.True(fired);
        }

        [Fact]
        private void ShouldSetErrosAndIsValidAfterInitialization()
        {
            _Persona.Nombre = "";
            var wrapper = new PersonaWrapper(_Persona);
            Assert.False(wrapper.IsValid);
            Assert.True(wrapper.HasErrors);

            var errors = wrapper.GetErrors(nameof(wrapper.Nombre)).Cast<string>();
            Assert.Equal(1, errors.Count());
            Assert.Equal("El campo de nombre es obligatorio", errors.First());
        }

        [Fact]
        private void ShouldRefreshErrorsAndIsValidWhenRejectingChanges()
        {
            var wrapper = new PersonaWrapper(_Persona);
            Assert.True(wrapper.IsValid);
            Assert.False(wrapper.HasErrors);

            wrapper.Nombre = "";

            Assert.False(wrapper.IsValid);
            Assert.True(wrapper.HasErrors);

            wrapper.RejectChanges();
            Assert.True(wrapper.IsValid);
            Assert.False(wrapper.HasErrors);
        }
    }
}
