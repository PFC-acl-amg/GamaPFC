using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Cooperante 
    {
        public virtual IList<Actividad> ActividadesDeQueEsCoordinador { get; protected set; }
        public virtual IList<Actividad> ActividadesEnQueParticipa { get; protected set; }
        public virtual string Apellido { get; set; }
        public virtual string Dni { get; set; }
        public virtual IList<Email> Emails { get; protected set; }
        public virtual int Id { get; protected set; }
        public virtual string Nombre { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual IList<Telefono> Telefonos { get; protected set; }

        public Cooperante()
        {
            ActividadesDeQueEsCoordinador = new List<Actividad>();
            ActividadesEnQueParticipa = new List<Actividad>();
            Emails = new List<Email>();
            Telefonos = new List<Telefono>();
        }

        // For Debug reasons (para los FakeRepositories poder ponerle un Id)
        public void SetId(int value)
        {
            Id = value;
        }
    }
}
