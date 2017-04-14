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
    public class AsistenteWrapper : TimestampedModelWrapper<Asistente>
    {
        public string _SavedNif;

        public AsistenteWrapper(Asistente model) : base(model)
        {

        }

        protected override void InitializeUniqueProperties(Asistente model)
        {
            _SavedNif = model.Nif;
        }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string Nombre
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string NombreOriginalValue => GetOriginalValue<string>(nameof(Nombre));

        public bool NombreIsChanged => GetIsChanged(nameof(Nombre));

        public string Nif
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string NifOriginalValue => GetOriginalValue<string>(nameof(Nif));

        public bool NifIsChanged => GetIsChanged(nameof(Nif));

        public string Email
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string EmailOriginalValue => GetOriginalValue<string>(nameof(Email));

        public bool EmailIsChanged => GetIsChanged(nameof(Email));

        public string Telefono
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string TelefonoOriginalValue => GetOriginalValue<string>(nameof(Telefono));

        public bool TelefonoIsChanged => GetIsChanged(nameof(Telefono));

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

        public NivelAcademico NivelAcademico
        {
            get { return GetValue<NivelAcademico>(); }
            set { SetValue(value); }
        }

        public NivelAcademico NivelAcademicoOriginalValue => GetOriginalValue<NivelAcademico>(nameof(NivelAcademico));

        public bool NivelAcademicoIsChanged => GetIsChanged(nameof(NivelAcademico));


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

        public ComoConocioAGama ComoConocioAGama
        {
            get { return GetValue<ComoConocioAGama>(); }
            set { SetValue(value); }
        }

        public ComoConocioAGama ComoConocioAGamaOriginalValue => GetOriginalValue<ComoConocioAGama>(nameof(ComoConocioAGama));

        public bool ComoConocioAGamaIsChanged => GetIsChanged(nameof(ComoConocioAGama));

        public byte[] Imagen
        {
            get { return GetValue<byte[]>(); }
            set { SetValue(value); }
        }
        public byte[] ImagenOriginalValue => GetOriginalValue<byte[]>(nameof(Imagen));

        public bool ImagenIsChanded => GetIsChanged(nameof(Imagen));

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Nif))
            {
                results.Add(new ValidationResult("El campo de NIF es obligatorio", new[] { nameof(Nif) }));
            }
            else if (Nif != _SavedNif && AtencionesResources.TodosLosNifDeAsistentes.Contains(Nif))
            {
                results.Add(new ValidationResult("El NIF introducido ya existe", new[] { nameof(Nif) }));
            }

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
