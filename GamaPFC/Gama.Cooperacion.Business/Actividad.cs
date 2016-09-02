using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public enum Estado
    {
        NoComenzado,
        Comenzado,
        Finalizado,
    }
    
    public class Actividad
    {
        public virtual IList<Cooperante> Cooperantes { get; protected set; }
        public virtual Cooperante Coordinador { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual Estado Estado { get; set; }
        public virtual DateTime FechaDeInicio { get; set; }
        public virtual DateTime FechaDeFin { get; set; }
        public virtual int Id { get; protected set; }
        public virtual string Titulo { get; set; }
        public virtual IList<Tarea> Tareas { get; protected set; }

        public Actividad()
        {
            Coordinador = new Cooperante();
            Cooperantes = new List<Cooperante>();
            Tareas = new List<Tarea>();
        }

        public virtual void AddCooperante(Cooperante cooperante)
        {
            Cooperantes.Add(cooperante);
            cooperante.ActividadesEnQueParticipa.Add(this);
        }

        public virtual void AddTarea(Tarea tarea)
        {
            Tareas.Add(tarea);
            tarea.Actividad = this;
        }

        public virtual void EstablecerCoordinador(Cooperante coordinador)
        {
            Coordinador = coordinador;
            coordinador.ActividadesDeQueEsCoordinador.Add(this);
            coordinador.ActividadesEnQueParticipa.Add(this);
        }
    }
}
