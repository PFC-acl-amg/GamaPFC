using Core;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Wrappers
{
    public class PersonaWrapper : TimestampedModelWrapper<Persona>
    {
        public PersonaWrapper(Persona model) : base(model)
        {

        }

        protected override void InitializeCollectionProperties(Persona model)
        {
            if (model.Citas == null)
            {
                throw new ArgumentNullException("Citas");
            }

            this.Citas = new ChangeTrackingCollection<CitaWrapper>
                (model.Citas.Select(c => new CitaWrapper(c)));
            this.RegisterCollection(this.Citas, model.Citas.ToList());
        }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public ComoConocioAGama ComoConocioAGama
        {
            get { return GetValue<ComoConocioAGama>(); }
            set { SetValue(value); }
        }

        public ComoConocioAGama ComoConocioAGamaOriginalValue => GetOriginalValue<ComoConocioAGama>(nameof(ComoConocioAGama));

        public bool ComoConocioAGamaIsChanged => GetIsChanged(nameof(ComoConocioAGama));

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

        public EstadoCivil EstadoCivil
        {
            get { return GetValue<EstadoCivil>(); }
            set { SetValue(value); }
        }

        public EstadoCivil EstadoCivilOriginalValue => GetOriginalValue<EstadoCivil>(nameof(EstadoCivil));

        public bool EstadoCivilIsChanged => GetIsChanged(nameof(EstadoCivil));

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

        public IdentidadSexual IdentidadSexual
        {
            get { return GetValue<IdentidadSexual>(); }
            set { SetValue(value); }
        }

        public IdentidadSexual IdentidadSexualOriginalVaue => GetOriginalValue<IdentidadSexual>(nameof(IdentidadSexual));

        public bool IdentidadSexualIsChanged => GetIsChanged(nameof(IdentidadSexual));

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

        public NivelAcademico NivelAcademico
        {
            get { return GetValue<NivelAcademico>(); }
            set { SetValue(value); }
        }

        public string NivelAcademicoOriginalValue => GetOriginalValue<string>(nameof(NivelAcademico));

        public bool NivelAcademicoIsChanged => GetIsChanged(nameof(NivelAcademico));

        public string Nombre
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string NombreOriginalValue => GetOriginalValue<string>(nameof(Nombre));

        public bool NombreIsChanged => GetIsChanged(nameof(Nombre));

        public string Ocupacion
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string OcupacionOriginalValue => GetOriginalValue<string>(nameof(Ocupacion));

        public bool OcupacionIsChanged => GetIsChanged(nameof(Ocupacion));

        public OrientacionSexual OrientacionSexual
        {
            get { return GetValue<OrientacionSexual>(); }
            set { SetValue(value); }
        }

        public OrientacionSexual OrientacionSexualOriginalValue => GetOriginalValue<OrientacionSexual>(nameof(OrientacionSexual));

        public bool OrientacionSexualIsChanged => GetIsChanged(nameof(OrientacionSexual));

        public string Telefono
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string TelefonoOriginalValue => GetOriginalValue<string>(nameof(Telefono));

        public bool TelefonoIsChanged => GetIsChanged(nameof(Telefono));

        public bool TieneTrabajo
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool TieneTrabajoOriginalValue => GetOriginalValue<bool>(nameof(TieneTrabajo));

        public bool TieneTrabajoIsChanged => GetIsChanged(nameof(TieneTrabajo));

        public string Twitter
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string TwitterOriginalValue => GetOriginalValue<string>(nameof(Twitter));

        public bool TwitterIsChanged => GetIsChanged(nameof(Twitter));

        public ViaDeAccesoAGama ViaDeAccesoAGama
        {
            get { return GetValue<ViaDeAccesoAGama>(); }
            set { SetValue(value); }
        }

        public ViaDeAccesoAGama ViaDeAccesoAGamaOriginalValue => GetOriginalValue<ViaDeAccesoAGama>(nameof(ViaDeAccesoAGama));

        public bool ViaDeAccesoAGamaIsChanged => GetIsChanged(nameof(ViaDeAccesoAGama));

        public ChangeTrackingCollection<CitaWrapper> Citas { get; private set; }

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
