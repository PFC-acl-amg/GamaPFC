using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Email 
    {
        public virtual int Id { get; protected set; }
        public virtual string Direccion { get; set; }
    }
}
