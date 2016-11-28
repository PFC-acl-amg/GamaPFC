using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Tarea : TimestampedModel
    {
        public virtual Actividad Actividad { get; set; }
        public virtual int Id { get;  set; }
        public virtual string Descripcion { get; set;  }
        public virtual bool HaFinalizado { get; set; }
        public virtual DateTime FechaDeFinalizacion { get; set; }
        public virtual Cooperante Responsable { get; set; }
        public virtual IList<Seguimiento> Historial { get; set; }
        public virtual IList<Seguimiento> Mensajes { get; set; }

        public Tarea() // cuando se crea una tarea se inicializa el historial de seguimiento y los mensajes de la tarea
        {
            Historial = new List<Seguimiento>();
            Mensajes = new List<Seguimiento>();
        }
        public virtual void AddMensaje(Seguimiento mensaje) // Para añadir un mensaje a Tarea
        {
            mensaje.Tarea = this;
            Mensajes.Add(mensaje);
        }
        public virtual void AddHistorial(Seguimiento historial) // Para añadir un historial de seguimiento Tarea
        {
            historial.Tarea = this;
            Mensajes.Add(historial);
        }
    }

}
