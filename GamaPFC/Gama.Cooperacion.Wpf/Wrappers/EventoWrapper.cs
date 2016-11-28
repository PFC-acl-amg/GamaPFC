using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class EventoWrapper : TimestampedModelWrapper<Evento>
    {
        public EventoWrapper(Evento model) : base(model)
        {
        }
        public string Titulo
        {
            get { return GetValue<string>(); }
            set { } 
        }
        public DateTime FechaDePublicacion
        {
            get { return GetValue<DateTime>(); }
            set { }
        }
        public Ocurrencia Ocurrencia
        {
            get { return GetValue<Ocurrencia>(); }
            set { }
        }
        public ActividadWrapper Actividad { get; private set; }
    }
}
