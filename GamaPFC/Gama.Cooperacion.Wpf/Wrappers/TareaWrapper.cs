using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class TareaWrapper : ModelWrapper<Tarea>
    {
        public TareaWrapper(Tarea model) : base(model)
        {

        }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string Descripcion
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public bool HaFinalizado
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public DateTime FechaDeFinalizacion
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public string Seguimiento
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public ActividadWrapper Actividad { get; private set; }
        public CooperanteWrapper Responsable { get; private set; }
    }
}
