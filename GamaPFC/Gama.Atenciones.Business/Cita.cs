using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Business
{
    public class Cita : TimestampedModel
    {
        public virtual string Asistente { get; set; }
        public virtual DateTime? Fin { get; set; }
        public virtual bool HaTenidoLugar { get; set; }
        public virtual int Id { get; set; }
        public virtual DateTime Inicio { get; set; }
        public virtual string Sala { get; set; }
        public virtual Atencion Atencion { get; set; }
        public virtual Persona Persona { get; set; }

        public Cita()
        {
            Inicio = DateTime.Now;
        }
    }
}
