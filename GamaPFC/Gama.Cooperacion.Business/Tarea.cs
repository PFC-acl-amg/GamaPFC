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
        public virtual DateTime? FechaDeFinalizacion { get; set; }
        public virtual Cooperante Responsable { get; set; }
        public virtual IList<Seguimiento> Seguimiento { get; set; }
        public virtual IList<Incidencia> Incidencias { get; set; }

        public Tarea() // cuando se crea una tarea se inicializa el historial de seguimiento y los mensajes de la tarea
        {
            //Historial = new List<Seguimiento>();
            Responsable = new Cooperante();
            Seguimiento = new List<Seguimiento>();
            Incidencias = new List<Incidencia>();
        }
        public virtual void AddIncidencia(Incidencia incidencia) // Para añadir un mensaje a Tarea
        {
            incidencia.Tarea = this;
            Incidencias.Add(incidencia);
        }
        public virtual void AddSeguimiento(Seguimiento historial) // Para añadir un historial de seguimiento Tarea
        {
            historial.Tarea = this;
            Seguimiento.Add(historial);
        }
    }

}
