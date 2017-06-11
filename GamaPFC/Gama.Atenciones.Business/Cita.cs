using Core;
using Core.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Business
{
    public class Cita : TimestampedModel
    {
        public virtual string AsistenteEnTexto { get; set; } = "";
        public virtual DateTime? Fin { get; set; }
        public virtual bool HaTenidoLugar { get; set; }
        public virtual int Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string Sala { get; set; } = "";
        public virtual Atencion Atencion { get; set; }
        public virtual Persona Persona { get; set; }
        public virtual Asistente Asistente { get; set; }
        public virtual int Hora { get; set; }
        public virtual int Minutos { get; set; }

        public Cita()
        {
            Fecha = DateTime.Now.Date;
            Hora = 0;
            Minutos = 0;
            EncryptedFields = new List<string>();

            EncryptedFields.AddRange(new[] {
                nameof(Persona),
                nameof(Asistente)
            });

            IsEncrypted = true;
        }

        public virtual void SetAtencion(Atencion atencion)
        {
            atencion.Cita = this;
            this.Atencion = atencion;
        }

        public virtual void CopyValuesFrom(Cita cita)
        {
            Id = cita.Id;
            AsistenteEnTexto = cita.AsistenteEnTexto;
            Fin = cita.Fin;
            HaTenidoLugar = cita.HaTenidoLugar;
            Fecha = new DateTime(cita.Fecha.Year, cita.Fecha.Month, cita.Fecha.Day,
                cita.Fecha.Hour, cita.Fecha.Minute, cita.Fecha.Second);
            Sala = cita.Sala;
            Hora = cita.Hora;
            Minutos = cita.Minutos;
            Asistente = cita.Asistente;
        }
    }
}
