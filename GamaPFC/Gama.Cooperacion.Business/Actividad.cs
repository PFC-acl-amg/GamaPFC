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
        public Cooperante Coordinador { get; set; }
        public string Descripcion { get; set; }
        public Estado Estado { get; set; }
        public DateTime FechaDeInicio { get; set; }
        public DateTime FechaDeFin { get; set; }
        public int Id { get; private set; }
        public string Titulo { get; set; }

        public List<Cooperante> Cooperantes { get; private set; }
    }
}
