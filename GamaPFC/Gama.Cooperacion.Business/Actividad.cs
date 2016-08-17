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
        // Son propiedades porque tienen accesores (getters y setter)
        public virtual Cooperante Coordinador { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual Estado Estado { get; set; }
        public virtual DateTime FechaDeInicio { get; set; }
        public virtual DateTime FechaDeFin { get; set; }
        public virtual int Id { get; protected set; }
        public virtual string Titulo { get; set; }

        public virtual IList<Cooperante> Cooperantes { get; protected set; }
    }
}
