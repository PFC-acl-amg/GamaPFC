using Gama.Atenciones.Business;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Gama.Atenciones.Wpf.Wrappers
{
    public class CitaWrapper : TimestampedModelWrapper<Cita>
    {
        public CitaWrapper(Cita model) : base(model)
        {

        }

        protected override void InitializeComplexProperties(Cita model)
        {
            if (model.Atencion != null)
            {
                Atencion = new AtencionWrapper(model.Atencion);
            }

            if (model.Persona == null)
            {
                throw new ArgumentNullException("Persona");
            }
        }

        public string Asistente
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string AsistenteOriginalValue => GetOriginalValue<string>(nameof(Asistente));

        public bool AsistenteIsChanged => GetIsChanged(nameof(Asistente));

        public DateTime? Fin
        {
            get { return GetValue<DateTime?>(); }
            set { SetValue(value); }
        }

        public DateTime? FinOriginalValue => GetOriginalValue<DateTime?>(nameof(Fin));

        public bool FinIsChanged => GetIsChanged(nameof(Fin));

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public DateTime? Fecha
        {
            get { return GetValue<DateTime?>(); }
            set { SetValue(value); }
        }

        public DateTime? FechaOriginalValue => GetOriginalValue<DateTime?>(nameof(Fecha));

        public bool FechaIsChanged => GetIsChanged(nameof(Fecha));

        public int Hora
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int HoraOriginalValue => GetOriginalValue<int>(nameof(Hora));

        public bool HoraIsChanged => GetIsChanged(nameof(Hora));

        public int Minutos
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int MinutosOriginalValue => GetOriginalValue<int>(nameof(Minutos));

        public bool MinutosIsChanged => GetIsChanged(nameof(Minutos));

        public string Sala
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string SalaOriginalValue => GetOriginalValue<string>(nameof(Sala));

        public bool SalaIsChanged => GetIsChanged(nameof(Sala));

        private AtencionWrapper _Atencion;
        public AtencionWrapper Atencion
        {
            get { return _Atencion; }
            set
            {
                _Atencion = value;
                Model.Atencion = value.Model;
            }
        }

        private PersonaWrapper _Persona;
        public PersonaWrapper Persona
        {
            get { return _Persona; }
            set
            {
                _Persona = value;
                Model.Persona = value.Model;
            }
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Fecha == null)
            {
                yield return new ValidationResult("El campo de fecha es obligatorio",
                    new[] { nameof(Fecha) });
            }
            if (string.IsNullOrWhiteSpace(Asistente))
            {
                yield return new ValidationResult("El campo de asistente es obligatorio",
                    new[] { nameof(Asistente) });
            }
            if (string.IsNullOrWhiteSpace(Sala))
            {
                yield return new ValidationResult("El campo de sala es obligatorio",
                    new[] { nameof(Sala) });
            }
        }
    }
}
