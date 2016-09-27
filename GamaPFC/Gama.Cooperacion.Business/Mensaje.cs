using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Mensaje
    {
        public virtual int Id { get; set; }
        public virtual string TituloMensaje { get; set; }
        public virtual DateTime FechaPublico { get; set; }

    }
}
