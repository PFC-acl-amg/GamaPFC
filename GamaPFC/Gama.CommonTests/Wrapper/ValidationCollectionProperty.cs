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
    public class ValidationCollectionProperty
    {
        private Cooperante _Cooperante;

        public ValidationCollectionProperty()
        {
            _Cooperante = new Cooperante
            {
                Nombre = "Algún nombre",
                //Emails = new List<Email>
                //{
                //    new Email { Direccion = "direccion1@dominio.com" },
                //    new Email { Direccion = "direccion2@otrodominio.com" }
                //}
            };
        }

        [Fact]
        private void ShouldSetIsValidOfRoot()
        {
            var wrapper = new CooperanteWrapper(_Cooperante);
            Assert.True(wrapper.IsValid);

            //wrapper.Emails.First().Direccion = "";
            //Assert.False(wrapper.IsValid);

            //wrapper.Emails.First().Direccion = "direccion1@dominio.com";
            //Assert.True(wrapper.IsValid);
        }

        [Fact]
        private void ShouldSetIsValidOfRootWhenInitializing()
        {
            //_Cooperante.Emails.First().Direccion = "";
            //var wrapper = new CooperanteWrapper(_Cooperante);
            //Assert.False(wrapper.IsValid);
            //Assert.False(wrapper.HasErrors);
            //Assert.True(wrapper.Emails.First().HasErrors);
        }

        [Fact]
        private void ShouldSetIsValidOfRootWhenRemovingInvalidItem()
        {
            var wrapper = new CooperanteWrapper(_Cooperante);
            Assert.True(wrapper.IsValid);

            //wrapper.Emails.First().Direccion = "";
            //Assert.False(wrapper.IsValid);

            //wrapper.Emails.RemoveAt(0);
            Assert.True(wrapper.IsValid);
        }

        [Fact]
        private void ShouldSetIsValidOfRootWhenAddingInvalidItem()
        {
            //var emailToAdd = new EmailWrapper(new Email());
            //var wrapper = new CooperanteWrapper(_Cooperante);
            //Assert.True(wrapper.IsValid);

            //wrapper.Emails.Add(emailToAdd);
            //Assert.False(wrapper.IsValid);

            //emailToAdd.Direccion = "valida@ok.com";
            //Assert.True(wrapper.IsValid);
        }

        [Fact]
        private void ShouldRaisePropertyChangeEventForIsValidOfRoot()
        {
            var fired = false;
            var wrapper = new CooperanteWrapper(_Cooperante);

            wrapper.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(wrapper.IsValid))
                {
                    fired = true;
                }
            };

            //wrapper.Emails.First().Direccion = "";
            //Assert.True(fired);

            //fired = false;
            //wrapper.Emails.First().Direccion = "valida@ok.com";
            //Assert.True(fired);
        }

        [Fact]
        private void ShouldRaisePropertyChangedEventForIsValidOfRootWhenRemovingInvalidItem()
        {
            var fired = false;
            var wrapper = new CooperanteWrapper(_Cooperante);
            Assert.True(wrapper.IsValid);

            wrapper.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(wrapper.IsValid))
                {
                    fired = true;
                }
            };

            //wrapper.Emails.First().Direccion = "";
            //Assert.True(fired);

            //fired = false;
            //wrapper.Emails.Remove(wrapper.Emails.First());
            //Assert.True(fired);
        }

        [Fact]
        private void ShoudlRaisePropertyChangedEventForIsValidOfRootWhenAddingInvalidItem()
        {
            //var emailToAdd = new EmailWrapper(new Email());
            //var wrapper = new CooperanteWrapper(_Cooperante);
            //Assert.True(wrapper.IsValid);

            var fired = false;

            //wrapper.PropertyChanged += (s, e) =>
            //{
            //    if (e.PropertyName == nameof(wrapper.IsValid))
            //    {
            //        fired = true;
            //    }
            //};

            //wrapper.Emails.Add(emailToAdd);
            //Assert.False(wrapper.IsValid);
            //Assert.True(fired);
        }
    }
}
