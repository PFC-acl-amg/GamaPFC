using Core;
using Core.Util;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Services;
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
        public string _SavedNif
        {
            get { return Model._SavedNif; }
            set { Model._SavedNif = value; }
        }

        public string Nif
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string NifOriginalValue => GetOriginalValue<string>(nameof(Nif));

        public bool NifIsChanged => GetIsChanged(nameof(Nif));

        public PersonaWrapper(Persona model) : base(model)
        {
            Validate();
        }

        protected override void InitializeUniqueProperties(Persona model)
        {
            model._SavedNif = model.Nif;
        }

        protected override void InitializeCollectionProperties(Persona model)
        {
            if (model.Citas == null)
            {
                throw new ArgumentNullException("Citas");
            }

            this.Citas = new ChangeTrackingCollection<CitaWrapper>
                (model.Citas.Select(c => new CitaWrapper(c)));
            this.RegisterCollection(this.Citas, model.Citas);
        }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string ComoConocioAGama
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ComoConocioAGamaOriginalValue => GetOriginalValue<string>(nameof(ComoConocioAGama));

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

        public string EstadoCivil
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string EstadoCivilOriginalValue => GetOriginalValue<string>(nameof(EstadoCivil));

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

        public string IdentidadSexual
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string IdentidadSexualOriginalValue => GetOriginalValue<string>(nameof(IdentidadSexual));

        public bool IdentidadSexualIsChanged => GetIsChanged(nameof(IdentidadSexual));

        public byte[] Imagen
        {
            get { return GetValue<byte[]>(); }
            set { SetValue(value); }
        }
        public byte[] ImagenOriginalValue => GetOriginalValue<byte[]>(nameof(Imagen));

        public bool ImagenIsChanded => GetIsChanged(nameof(Imagen));

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

        public void AddCita(Cita cita)
        {
            Model.AddCita(cita);
            Citas.Add(new CitaWrapper(cita));
        }

        public bool NacionalidadIsChanged => GetIsChanged(nameof(Nacionalidad));

        public string NivelAcademico
        {
            get { return GetValue<string>(); }
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

        public string OrientacionSexual
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string OrientacionSexualOriginalValue => GetOriginalValue<string>(nameof(OrientacionSexual));

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

        public string ViaDeAccesoAGama
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ViaDeAccesoAGamaOriginalValue => GetOriginalValue<string>(nameof(ViaDeAccesoAGama));

        public bool ViaDeAccesoAGamaIsChanged => GetIsChanged(nameof(ViaDeAccesoAGama));

        public ChangeTrackingCollection<CitaWrapper> Citas { get; private set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Nif))
            {
                results.Add(new ValidationResult("El campo de NIF es obligatorio", new[] { nameof(Nif) }));
            }
            else if (Nif != Model._SavedNif && PersonaRepository.Nifs.Contains(Nif))
            {
                results.Add(new ValidationResult("El NIF introducido ya existe", new[] { nameof(Nif) }));
            }

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                results.Add(new ValidationResult("El campo de nombre es obligatorio", new[] { nameof(Nombre) }));
            }

            if (StringUtility.EmailIsNotEmptyAndIsMatch(Email))
            {
                results.Add(new ValidationResult("Email inválido", new[] { nameof(Email) }));
            }

            return results;
        }
    }
}
