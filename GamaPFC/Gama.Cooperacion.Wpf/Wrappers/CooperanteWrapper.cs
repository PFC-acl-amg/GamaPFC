using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class CooperanteWrapper : TimestampedModelWrapper<Cooperante>
    {
        public string _SavedNif;
        public CooperanteWrapper(Cooperante model) : base (model)
        {
        }

        protected override void InitializeCollectionProperties(Cooperante model)
        {
        }
        public override bool IsChanged => base.IsChanged;

        public override void AcceptChanges()
        {
            base.AcceptChanges();
        }

        public override void RejectChanges()
        {
            base.RejectChanges();
        }
        // Nuevos Campos añadidos a la clase Cooperante
        public string Nif
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string Provincia
        {
            get { return GetValue<string>(); }
            set { SetValue(value);}
        }
        public string ProvinciaOriginalValue => GetOriginalValue<string>(nameof(Provincia));
        public bool ProvinciaIsChanged => GetIsChanged(nameof(Provincia));

        public string Municipio
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string MunicipioOriginalValue => GetOriginalValue<string>(nameof(Municipio));
        public bool MunicipioIsChanged => GetIsChanged(nameof(Municipio));

        public string CP
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string CPOriginalValue => GetOriginalValue<string>(nameof(CP));
        public bool CPIsChanged => GetIsChanged(nameof(CP));

        public string Localidad
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string LocalidadOriginalValue => GetOriginalValue<string>(nameof(Localidad));
        public bool LocalidadIsChanged => GetIsChanged(nameof(Localidad));

        public string Calle
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string CalleOriginalValue => GetOriginalValue<string>(nameof(Calle));
        public bool CalleIsChanged => GetIsChanged(nameof(Calle));

        public string Numero
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string NumeroOriginalValue => GetOriginalValue<string>(nameof(Numero));
        public bool NumeroIsChanged => GetIsChanged(nameof(Numero));

        public string Portal
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string PortalOriginalValue => GetOriginalValue<string>(nameof(Portal));
        public bool PortalIsChanged => GetIsChanged(nameof(Portal));

        public string Piso
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string PisoOriginalValue => GetOriginalValue<string>(nameof(Piso));
        public bool PisoIsChanged => GetIsChanged(nameof(Piso));

        public string Puerta
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string PuertaOriginalValue => GetOriginalValue<string>(nameof(Puerta));
        public bool PuertaIsChanged => GetIsChanged(nameof(Puerta));

        public DateTime? FechaDeNacimiento
        {
            get { return GetValue<DateTime?>(); }
            set { SetValue(value); }
        }
        public DateTime? FechaDeNacimientoOriginalValue => GetOriginalValue<DateTime?>(nameof(FechaDeNacimiento));
        public bool FechaDeNacimientoIsChanged => GetIsChanged(nameof(FechaDeNacimiento));

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
        // Fin nuevos campos
        public string Apellido
        {
            get { return GetValue<string>(); }
            set { SetValue(value); OnPropertyChanged("NombreCompleto"); }
        }

        public string ApellidoOriginalValue => GetOriginalValue<string>(nameof(Apellido));

        public bool ApellidoIsChanged => GetIsChanged(nameof(Apellido));

        public string Dni
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string DniOriginalValue => GetOriginalValue<string>(nameof(Dni));

        public bool DniIsChanged => GetIsChanged(nameof(Dni));

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string Nombre
        {
            get { return GetValue<string>(); }
            set
            {
                SetValue(value);
                OnPropertyChanged("NombreCompleto");
            }
        }

        public string NombreOriginalValue => GetOriginalValue<string>(nameof(Nombre));

        public bool NombreIsChanged => GetIsChanged(nameof(Nombre));

        public string Observaciones
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        // Añadiendo Campos Faltantes en la clase Cooperante
        public string Telefono
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string TelefonoOriginalValue => GetOriginalValue<string>(nameof(Telefono));
        public bool TelefonoIsChanged => GetIsChanged(nameof(Telefono));
        public byte[] Foto
        {
            get { return GetValue<byte[]>(); }
            set { SetValue(value); }
        }
        public byte[] ImagenSocioOriginalValue => GetOriginalValue<byte[]>(nameof(Foto));

        public bool ImagenSocioIsChanded => GetIsChanged(nameof(Foto));
        public DateTime? FotoUpdatedAt
        {
            get { return GetValue<DateTime?>(); }
            set { SetValue(value); }
        }

        public DateTime? ImagenUpdatedAtOriginalValue => GetOriginalValue<DateTime?>(nameof(FotoUpdatedAt));

        public bool ImagenUpdatedAtIsChanged => GetIsChanged(nameof(FotoUpdatedAt));

        public string ObservacionesOriginalValue => GetOriginalValue<string>(nameof(Observaciones));

        public bool ObservacionesIsChanged => GetIsChanged(nameof(Observaciones));

        // Propiedad del Wrapper únicamente
        public string NombreCompleto => string.Format("{0} {1}", Nombre, Apellido);

        public override string ToString()
        {
            return NombreCompleto;
        }
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                yield return new ValidationResult("El Nombre es obligatorio",
                    new[] { nameof(Nombre) });
            }

            if (string.IsNullOrWhiteSpace(Apellido))
            {
                yield return new ValidationResult("El Apellido es obligatorio",
                    new[] { nameof(Apellido) });
            }
            if (string.IsNullOrWhiteSpace(Dni))
            {
                yield return new ValidationResult("El DNI es obligatorio",
                    new[] { nameof(Dni) });
            }
            //if (Coordinador == null || Coordinador.Id == 0)
            //{
            //    yield return new ValidationResult("La actividad debe tener un coordinador",
            //        new[] { nameof(Coordinador) });
            //}
        }
    }
}
