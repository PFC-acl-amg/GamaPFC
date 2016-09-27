using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Seguimiento
    {
        public virtual int Id { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual DateTime FechaDePublicacion { get; set; }
    }
}
