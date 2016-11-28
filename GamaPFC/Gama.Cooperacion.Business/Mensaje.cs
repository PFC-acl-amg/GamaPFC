using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Mensaje : TimestampedModel
    {
        public virtual int Id { get; set; }
        public virtual string Titulo { get; set; }
        public virtual DateTime FechaDePublicacion { get; set; }
        public virtual Foro Foro { get; set; }
    }
}
