using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Business
{
    public class Asistente
    {
        public virtual int Id { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Email { get; set; }
        public virtual string Telefono { get; set; } = "";
        public virtual byte[] Imagen { get; set; }

        public virtual IList<Cita> Citas { get; set; }
    }
}
