using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class SeguimientoWrapper : ModelWrapper<Seguimiento>
    {
        public SeguimientoWrapper(Seguimiento model) : base(model)
        {
        }
        public int Id
        {
            get { return GetValue<int>(); }
            set { } // No se pone nada dentro del set porque los mensajes son para ller solo no se modificaran ni se borraran asi tma read only
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
        public TareaWrapper Tarea { get; private set; }
    }
}
