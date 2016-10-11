using Gama.Atenciones.Business;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Wrappers
{
    public class CitaWrapper : TimestampedModelWrapper<Cita>
    {
        public CitaWrapper(Cita model) : base(model)
        {

        }

        public string Asistente
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string AsistenteOriginalValue => GetOriginalValue<string>(nameof(Asistente));

        public bool AsistenteIsChanged => GetIsChanged(nameof(Asistente));

        public DateTime? Fin
        {
            get { return GetValue<DateTime?>(); }
            set { SetValue(value); }
        }

        public DateTime? FinOriginalValue => GetOriginalValue<DateTime?>(nameof(Fin));

        public bool FinIsChanged => GetIsChanged(nameof(Fin));

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public DateTime? Inicio
        {
            get { return GetValue<DateTime?>(); }
            set { SetValue(value); }
        }

        public DateTime? InicioOriginalValue => GetOriginalValue<DateTime?>(nameof(Inicio));

        public bool InicioIsChanged => GetIsChanged(nameof(Inicio));

        public string Sala
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string SalaOriginalValue => GetOriginalValue<string>(nameof(Sala));

        public bool SalaIsChanged => GetIsChanged(nameof(Sala));

        public AtencionWrapper Atencion { get; private set; }
    }
}
