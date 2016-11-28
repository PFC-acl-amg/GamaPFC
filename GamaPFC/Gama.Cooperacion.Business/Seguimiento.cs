using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Seguimiento: TimestampedModel
    {
        public virtual int Id { get; set; }
        public virtual int Tipo { get; set; } // 0 => Mensaje, 1 => Seguimiento
        public virtual string Descripcion { get; set; }
        public virtual DateTime FechaDePublicacion { get; set; }
        public virtual Tarea Tarea { get; set; }
    }
}
