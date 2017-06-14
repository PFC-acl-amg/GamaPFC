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

        public string AsistenteEnTexto
        {
            get
            {
                return Model.Asistente != null ? Model.Asistente.Nombre : "";
            }
            set { SetValue(value); }
        }

        public string AsistenteEnTextoOriginalValue => GetOriginalValue<string>(nameof(AsistenteEnTexto));

        public bool AsistenteEnTextoIsChanged => GetIsChanged(nameof(AsistenteEnTexto));

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

        //public Asistente Asistente
        //{
        //    get { return GetValue<Asistente>(); }
        //    set { SetValue(value); }
        //}

        public Asistente Asistente
        {
            get { return GetValue<Asistente>(); }
            set { SetValue(value); }
        }

        public Asistente AsistenteOriginalValue => GetOriginalValue<Asistente>(nameof(Asistente));
        public bool AsistenteIsChanged => GetIsChanged(nameof(Asistente));

        public Persona Persona
        {
            get { return Model.Persona; }
            set
            {
                Model.Persona = value;
            }
        }

        public void CopyValuesFrom(Cita cita)
        {
            Id = cita.Id;
            AsistenteEnTexto = cita.AsistenteEnTexto;
            Fin = cita.Fin;
            Fecha = new DateTime(cita.Fecha.Ticks);
            Sala = cita.Sala;
            Hora = cita.Hora;
            Minutos = cita.Minutos;
            Asistente = cita.Asistente;
            //Model.CopyValuesFrom(cita);
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Fecha == null)
            {
                yield return new ValidationResult("El campo de fecha es obligatorio",
                    new[] { nameof(Fecha) });
            }

            if (Model.Asistente == null || string.IsNullOrWhiteSpace(Model.Asistente.Nombre))
            {
                yield return new ValidationResult("El campo de asistente es obligatorio",
                    new[] { nameof(Asistente) });
            }

            if (Model.Asistente != null && Model.Asistente.SolapaConOtrasCitas(this.Model, 45))
            {
                yield return new ValidationResult("La fecha indicada solapa con otra cita",
                       new[] { nameof(Hora), nameof(Minutos) });
            }

            if (string.IsNullOrWhiteSpace(Sala))
            {
                yield return new ValidationResult("El campo de sala es obligatorio",
                    new[] { nameof(Sala) });
            }
        }
    }
}
