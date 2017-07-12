using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Foro : TimestampedModel
    {
        public virtual int Id { get; set; }
        public virtual string Titulo { get; set; }
        public virtual DateTime FechaDePublicacion { get; set; }
        public virtual IList<Mensaje> Mensajes { get; set; } // esto me obliga a implementar el constructor 1
        public virtual Actividad Actividad { get; set; }

        public Foro() // 1=> cuando se crea un foro se iniciciliza la lista Mensaje
        {
            Mensajes = new List<Mensaje>(); // Si en mensaje tubieramos otra contenedor como una lista saltaria al contructor para inicilizar ea lista
        }
        public virtual void AddMensaje(Mensaje mensaje)
        {
            mensaje.Foro = this;
            Mensajes.Add(mensaje);
        }

        public virtual void AddMensajes(List<Mensaje> mensajes)
        {
            foreach (var mensaje in mensajes)
                AddMensaje(mensaje);
        }
    }

}
