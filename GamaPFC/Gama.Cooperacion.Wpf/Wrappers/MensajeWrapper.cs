using Core;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Wrappers
{
    public class MensajeWrapper : ModelWrapper<Mensaje>
    {
        public MensajeWrapper(Mensaje model) : base(model)
        {
        }
        public int Id
        {
            get { return GetValue<int>(); }
            set { } // No se pone nada dentro del set porque los mensajes son para ller solo no se modificaran ni se borraran asi tma read only
        }

        public string Titulo
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string TituloOriginalValue => GetOriginalValue<string>(nameof(Titulo));

        public bool TituloIsChanged => GetIsChanged(nameof(Titulo));

        public DateTime FechaDePublicacion
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public DateTime FechaDePublicacionOriginalValue => GetOriginalValue<DateTime>(nameof(FechaDePublicacion));

        public ForoWrapper Foro { get; private set; }
    }
}
