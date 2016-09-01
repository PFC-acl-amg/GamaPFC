﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Cooperante 
    {
        public virtual IList<Actividad> ActividadesEnQueParticipa { get; protected set; }
        public virtual string Apellido { get; set; }
        public virtual string Dni { get; set; }
        public virtual IList<string> Emails { get; private set; }
        public virtual int Id { get; private set; }
        public virtual string Nombre { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual IList<string> Telefonos { get; private set; }

        public Cooperante()
        {
            ActividadesEnQueParticipa = new List<Actividad>();
            Emails = new List<string>();
            Telefonos = new List<string>();
        }
    }
}
