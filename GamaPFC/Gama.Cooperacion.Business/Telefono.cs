using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Telefono
    {   
        public virtual int Id { get; protected set; }
        public virtual string Numero { get; set; }
    }
}
