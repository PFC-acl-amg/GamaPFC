﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Eventos
{
    public class CitaEliminadaEvent : PubSubEvent<int>
    {
        internal void Subscribe(bool v)
        {
            throw new NotImplementedException();
        }
    }
}
