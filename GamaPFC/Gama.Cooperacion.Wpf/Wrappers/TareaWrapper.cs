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

        public string DescripcionOriginalValue => GetOriginalValue<string>(nameof(Descripcion));

        public bool DescripcionIsChanged => GetIsChanged(nameof(Descripcion));

        public bool HaFinalizado
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool HaFinalizadoOriginalValue => GetOriginalValue<bool>(nameof(HaFinalizado));

        public bool HaFinalizadoIsChanged => GetIsChanged(nameof(HaFinalizado));

        public DateTime FechaDeFinalizacion
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public DateTime FechaDeFinalizacionOriginalValue => GetOriginalValue<DateTime>(nameof(FechaDeFinalizacion));

        public bool FechaDeFinalizacionIsChanged => GetIsChanged(nameof(FechaDeFinalizacion));

        public string Seguimiento
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string SeguimientoOriginalValue => GetOriginalValue<string>(nameof(Seguimiento));

        public bool SeguimientoIsChanged => GetIsChanged(nameof(Seguimiento));

        public ActividadWrapper Actividad { get; private set; }
        public CooperanteWrapper Responsable { get; private set; }
    }
}
