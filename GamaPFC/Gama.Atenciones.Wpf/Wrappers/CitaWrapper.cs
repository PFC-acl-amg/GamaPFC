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

        public DateTime? Inicio
        {
            get { return GetValue<DateTime?>(); }
            set { SetValue(value); }
        }

        public DateTime? InicioOriginalValue => GetOriginalValue<DateTime?>(nameof(Inicio));

        public bool InicioIsChanged => GetIsChanged(nameof(Inicio));

        public string Sala
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string SalaOriginalValue => GetOriginalValue<string>(nameof(Sala));

        public bool SalaIsChanged => GetIsChanged(nameof(Sala));

        public AtencionWrapper Atencion { get; set; }
        public PersonaWrapper Persona { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Inicio == null)
            {
                yield return new ValidationResult("El campo de inicio es obligatorio",
                    new[] { nameof(Inicio) });
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
