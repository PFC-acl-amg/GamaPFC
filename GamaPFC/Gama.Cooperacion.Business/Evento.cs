using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public enum Ocurrencia
    {
        Mensaje_Publicado,
        Tarea_Finalizada,
    }
    public class Evento
    {
        // public virtual Actividad Actividad { get; set; }
        public virtual string Titulo { get; set; }
        public virtual DateTime FechaPublicado { get; set; }
        public virtual Ocurrencia EventoSucedido { get; set; }

    }
}
