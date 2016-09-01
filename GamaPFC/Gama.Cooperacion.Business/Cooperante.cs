using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Cooperante 
    {
        public IList<Actividad> ActividadesEnQueParticipa { get; protected set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public IList<string> Emails { get; private set; }
        public int Id { get; private set; }
        public string Nombre { get; set; }
        public string Observaciones { get; set; }
        public IList<Tarea> TareasEnQueParticipa { get; protected set; }
        public IList<string> Telefonos { get; private set; }

        public Cooperante()
        {
            ActividadesEnQueParticipa = new List<Actividad>();
            Emails = new List<string>();
            TareasEnQueParticipa = new List<Tarea>();
            Telefonos = new List<string>();
        }
    }
}
