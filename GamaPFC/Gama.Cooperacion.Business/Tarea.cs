using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Tarea
    {
        public virtual int Id { get; protected set; }
        public virtual string Descripcion { get; set;  }
        public virtual bool HaFinalizado { get; set; }
        public virtual DateTime Plazo { get; set; }
        public virtual Cooperante Responsable { get; set; }
    }
}
