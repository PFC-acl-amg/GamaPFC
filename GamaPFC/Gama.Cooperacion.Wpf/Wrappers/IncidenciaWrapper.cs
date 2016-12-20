using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{

    public class IncidenciaWrapper : ModelWrapper<Incidencia>
    {
        public IncidenciaWrapper(Incidencia model) : base(model)
        {
        }
        public int Id
        {
            get { return GetValue<int>(); }
            set { } // No se pone nada dentro del set porque los mensajes son para ller solo no se modificaran ni se borraran asi tma read only
        }
        public int Solucionada
        {
            get { return GetValue<int>(); }
            set { } 
        }
        public string Descripcion
        {
            get { return GetValue<string>(); }
            set { }
        }
        public DateTime FechaDePublicacion
        {
            get { return GetValue<DateTime>(); }
            set { }
        }
    }
}
