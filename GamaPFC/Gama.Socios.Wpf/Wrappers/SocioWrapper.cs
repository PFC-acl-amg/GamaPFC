using Core;
using Gama.Socios.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.Wrappers
{
    public class SocioWrapper : TimestampedModelWrapper<Socio>
    {
        public SocioWrapper(Socio model) : base(model)
        {

        }

        protected override void InitializeCollectionProperties(Socio model)
        {
            if (model.PeriodosDeAlta == null)
            {
                throw new ArgumentNullException("PeriodosDeAlta");
            }

            this.PeriodosDeAlta = new ChangeTrackingCollection<PeriodoDeAltaWrapper>
                (model.PeriodosDeAlta.Select(x => new PeriodoDeAltaWrapper(x)));
            this.RegisterCollection(this.PeriodosDeAlta, model.PeriodosDeAlta);
        }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string DireccionPostal
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string DireccionPostalOriginalValue => GetOriginalValue<string>(nameof(DireccionPostal));

        public bool DireccionPostalIsChanged => GetIsChanged(nameof(DireccionPostal));

        public string Email
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string EmailOriginalValue => GetOriginalValue<string>(nameof(Email));

        public bool EmailIsChanged => GetIsChanged(nameof(Email));

        public DateTime? FechaDeNacimiento
        {
            get { return GetValue<DateTime?>(); }
            set { SetValue(value); }
        }

        public DateTime? FechaDeNacimientoOriginalValue => GetOriginalValue<DateTime?>(nameof(FechaDeNacimiento));

        public bool FechaDeNacimientoIsChanged => GetIsChanged(nameof(FechaDeNacimiento));

        public string Facebook
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string FacebookOriginalValue => GetOriginalValue<string>(nameof(Facebook));

        public bool FacebookIsChanged => GetIsChanged(nameof(Facebook));

        public string LinkedIn
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string LinkedInOriginalValue => GetOriginalValue<string>(nameof(LinkedIn));

        public bool LinkedInIsChanded => GetIsChanged(nameof(LinkedIn));

        public string Nacionalidad
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string NacionalidadOriginalValue => GetOriginalValue<string>(nameof(Nacionalidad));

        public bool NacionalidadIsChanged => GetIsChanged(nameof(Nacionalidad));

        public string Nif
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string NifOriginalValue => GetOriginalValue<string>(nameof(Nif));

        public bool NifIsChanged => GetIsChanged(nameof(Nif));

        public string Nombre
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string NombreOriginalValue => GetOriginalValue<string>(nameof(Nombre));

        public bool NombreIsChanged => GetIsChanged(nameof(Nombre));

        public string Telefono
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string TelefonoOriginalValue => GetOriginalValue<string>(nameof(Telefono));

        public bool TelefonoIsChanged => GetIsChanged(nameof(Telefono));

        public string Twitter
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string TwitterOriginalValue => GetOriginalValue<string>(nameof(Twitter));

        public bool TwitterIsChanged => GetIsChanged(nameof(Twitter));

        public ChangeTrackingCollection<PeriodoDeAltaWrapper> PeriodosDeAlta { get; private set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                results.Add(new ValidationResult("El campo de nombre es obligatorio", new[] { nameof(Nombre) }));
            }

            var pattern =
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

            if (!string.IsNullOrWhiteSpace(Email) &&
                !Regex.IsMatch(Email, pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
            {
                results.Add(new ValidationResult("Email inválido", new[] { nameof(Email) }));
            }

            return results;
        }
    }
}
