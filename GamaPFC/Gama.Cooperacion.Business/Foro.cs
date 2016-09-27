using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Foro
    {
        public virtual int Id { get; set; }
        public virtual string TituloForo { get; set; }
        public virtual DateTime FechaForo { get; set; }
        public virtual IList<Mensaje> Mensajes { get; set; } // esto me ogliba a implemntar el constructor 1

        public Foro() // 1=> cuando se crea un foro se iniciciliza la lista Mensaje
        {
            Mensajes = new List<Mensaje>(); // Si en mensaje tubieramos otra contenedor como una lista saltaria al contructor para inicilizar ea lista
        }
    }

}
