using Core;
using Core.Util;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        protected override void InitializeCollectionProperties(Asistente model)
        {
            if (model.Citas == null)
                throw new ArgumentNullException("Citas");

            Citas = new ChangeTrackingCollection<CitaWrapper>(
                model.Citas.Select(c => new CitaWrapper(c)));
            RegisterCollection(this.Citas, model.Citas);

            //this.Citas = new ChangeTrackingCollection<CitaWrapper>
            //    (model.Citas.Select(c => new CitaWrapper(c)));
            //this.RegisterCollection(this.Citas, model.Citas);
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

        public string Apellidos
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ApellidosOriginalValue => GetOriginalValue<string>(nameof(Apellidos));

        public bool ApellidosIsChanged => GetIsChanged(nameof(Apellidos));

        public string Nif
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string NifOriginalValue => GetOriginalValue<string>(nameof(Nif));

        public bool NifIsChanged => GetIsChanged(nameof(Nif));

        public string Ocupacion
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string OcupacionOriginalValue => GetOriginalValue<string>(nameof(Ocupacion));

        public bool OcupacionIsChanged => GetIsChanged(nameof(Ocupacion));

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

        public bool LinkedInIsChanged => GetIsChanged(nameof(LinkedIn));

        public string Twitter
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string TwitterOriginalValue => GetOriginalValue<string>(nameof(Twitter));

        public bool TwitterIsChanged => GetIsChanged(nameof(Twitter));

        public string NivelAcademico
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string NivelAcademicoOriginalValue => GetOriginalValue<string>(nameof(NivelAcademico));

        public bool NivelAcademicoIsChanged => GetIsChanged(nameof(NivelAcademico));

        public DateTime? FechaDeNacimiento
        {
            get { return GetValue<DateTime?>(); }
            set { SetValue(value); }
        }

        public DateTime? FechaDeNacimientoOriginalValue => GetOriginalValue<DateTime?>(nameof(FechaDeNacimiento));

        public bool FechaDeNacimientoIsChanged => GetIsChanged(nameof(FechaDeNacimiento));

        public string ComoConocioAGama
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ComoConocioAGamaOriginalValue => GetOriginalValue<string>(nameof(ComoConocioAGama));

        public bool ComoConocioAGamaIsChanged => GetIsChanged(nameof(ComoConocioAGama));

        public byte[] Imagen
        {
            get { return GetValue<byte[]>(); }
            set { SetValue(value); }
        }
        public byte[] ImagenOriginalValue => GetOriginalValue<byte[]>(nameof(Imagen));

        public bool ImagenIsChanded => GetIsChanged(nameof(Imagen));

        public string Observaciones
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ObservacionesOriginalValue => GetOriginalValue<string>(nameof(Observaciones));

        public bool ObservacionesIsChanged => GetIsChanged(nameof(Observaciones));

        public string TelefonoFijo
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string TelefonoFijoOriginalValue => GetOriginalValue<string>(nameof(TelefonoFijo));

        public bool TelefonoFijoIsChanged => GetIsChanged(nameof(TelefonoFijo));


        public string TelefonoMovil
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string TelefonoMovilOriginalValue => GetOriginalValue<string>(nameof(TelefonoMovil));

        public bool TelefonoMovilIsChanged => GetIsChanged(nameof(TelefonoMovil));


        public string TelefonoAlternativo
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string TelefonoAlternativoOriginalValue => GetOriginalValue<string>(nameof(TelefonoAlternativo));

        public bool TelefonoAlternativoIsChanged => GetIsChanged(nameof(TelefonoAlternativo));


        public string Email
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string EmailOriginalValue => GetOriginalValue<string>(nameof(Email));

        public bool EmailIsChanged => GetIsChanged(nameof(Email));


        public string EmailAlternativo
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string EmailAlternativoOriginalValue => GetOriginalValue<string>(nameof(EmailAlternativo));

        public bool EmailAlternativoIsChanged => GetIsChanged(nameof(EmailAlternativo));


        public string Puerta
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string PuertaOriginalValue => GetOriginalValue<string>(nameof(Puerta));

        public bool PuertaIsChanged => GetIsChanged(nameof(Puerta));


        public string Piso
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string PisoOriginalValue => GetOriginalValue<string>(nameof(Piso));

        public bool PisoIsChanged => GetIsChanged(nameof(Piso));


        public string Portal
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string PortalOriginalValue => GetOriginalValue<string>(nameof(Portal));

        public bool PortalIsChanged => GetIsChanged(nameof(Portal));


        public string Numero
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string NumeroOriginalValue => GetOriginalValue<string>(nameof(Numero));

        public bool NumeroIsChanged => GetIsChanged(nameof(Numero));


        public string Calle
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string CalleOriginalValue => GetOriginalValue<string>(nameof(Calle));

        public bool CalleIsChanged => GetIsChanged(nameof(Calle));


        public string CodigoPostal
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string CodigoPostalOriginalValue => GetOriginalValue<string>(nameof(CodigoPostal));

        public bool CodigoPostalIsChanged => GetIsChanged(nameof(CodigoPostal));


        public string Localidad
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string LocalidadOriginalValue => GetOriginalValue<string>(nameof(Localidad));

        public bool LocalidadIsChanged => GetIsChanged(nameof(Localidad));


        public string Municipio
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string MunicipioOriginalValue => GetOriginalValue<string>(nameof(Municipio));

        public bool MunicipioIsChanged => GetIsChanged(nameof(Municipio));


        public string Provincia
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ProvinciaOriginalValue => GetOriginalValue<string>(nameof(Provincia));

        public bool ProvinciaIsChanged => GetIsChanged(nameof(Provincia));

        public ChangeTrackingCollection<CitaWrapper> Citas { get; private set; }

        public ChangeTrackingCollection<CitaWrapper> CitasPasadas { get;  set; }
        public ChangeTrackingCollection<CitaWrapper> CitasProximas { get;  set; }

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

            if (string.IsNullOrWhiteSpace(TelefonoMovil))
            {
                results.Add(new ValidationResult("El campo de nombre es obligatorio", new[] { nameof(TelefonoMovil) }));
            }

            if (!string.IsNullOrWhiteSpace(Email) && StringUtility.EmailIsNotEmptyAndIsMatch(Email))
            {
                results.Add(new ValidationResult("Email inválido", new[] { nameof(Email) }));
            }

            if (!string.IsNullOrWhiteSpace(EmailAlternativo) && StringUtility.EmailIsNotEmptyAndIsMatch(EmailAlternativo))
            {
                results.Add(new ValidationResult("Email alternativo inválido", new[] { nameof(EmailAlternativo) }));
            }

            return results;
        }
    }
}
