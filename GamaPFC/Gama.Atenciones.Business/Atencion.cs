using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Business
{
    public class Atencion : TimestampedModel
    {
        public virtual int Id { get; set; }
        public virtual int NumeroDeExpediente { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string Seguimiento { get; set; }

        public virtual bool EsSocial { get; set; }
        public virtual bool EsJuridica { get; set; }
        public virtual bool EsPsicologica { get; set; }
        public virtual bool EsDeAcogida { get; set; }
        public virtual bool EsDeOrientacionLaboral { get; set; }
        public virtual bool EsDePrevencionParaLaSalud { get; set; }
        public virtual bool EsDeFormacion { get; set; }
        public virtual bool EsDeParticipacion { get; set; }
        public virtual bool EsOtra { get; set; }
        public virtual string Otra { get; set; }

        public virtual Cita Cita { get; set; }

        public virtual Derivacion DerivacionesPropuestas { get; protected set; }
        public virtual Derivacion DerivacionesRealizadas { get; protected set; }

        public Atencion()
        {
            DerivacionesPropuestas = new Derivacion();
            DerivacionesRealizadas = new Derivacion();
        }
    }
}
