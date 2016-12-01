using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Incidencia : TimestampedModel
    {
        public virtual int Id { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual DateTime FechaDePublicacion { get; set; }
        public virtual int Solucionada { get; set; } // Resuelta = 0 || Sin resolver = 1
        public virtual Tarea Tarea { get; set; }
    }
}
