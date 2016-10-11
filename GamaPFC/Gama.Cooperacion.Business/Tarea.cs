using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Tarea
    {
        //public virtual Actividad Actividad { get; set; }
        public virtual int Id { get;  set; }
        public virtual string Descripcion { get; set;  }
        public virtual bool HaFinalizado { get; set; }
        public virtual DateTime FechaDeFinalizacion { get; set; }
        public virtual Cooperante Responsable { get; set; }
        public virtual IList<Seguimiento> Historial { get; set; }
        public virtual IList<Mensaje> Mensajes { get; set; }

        public Tarea()
        {
            Historial = new List<Seguimiento>();
            Mensajes = new List<Mensaje>();
        }
    }

}
