using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Cooperante 
    {
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public List<string> Email { get; private set; }
        public int Id { get; private set; }
        public string Nombre { get; set; }
        public string Observaciones { get; set; }
        public List<string> Telefonos { get; private set; }
    }
}
