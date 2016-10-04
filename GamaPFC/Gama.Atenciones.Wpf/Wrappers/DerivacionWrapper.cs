using Core;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Wrappers
{
    public class DerivacionWrapper : ModelWrapper<Derivacion>
    {
        public DerivacionWrapper(Derivacion model) : base(model)
        {

        }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public string Tipo
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string TipoOriginalValue => GetOriginalValue<string>(nameof(Tipo));

        public bool TipoIsChanged => GetIsChanged(nameof(Tipo));

        public bool EsSocial
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsSocialOriginalValue => GetOriginalValue<bool>(nameof(EsSocial));

        public bool EsSocialIsChanged => GetIsChanged(nameof(EsSocial));

        public bool EsJuridica
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsJuridicaOriginalValue => GetOriginalValue<bool>(nameof(EsJuridica));

        public bool EsJuridicaIsChanged => GetIsChanged(nameof(EsJuridica));

        public bool EsPsicologica
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsPsicologicaOriginalValue => GetOriginalValue<bool>(nameof(EsPsicologica));

        public bool EsPsicologicaIsChanged => GetIsChanged(nameof(EsPsicologica));

        public bool EsDeAcodiga
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsDeAcodigaOriginalValue => GetOriginalValue<bool>(nameof(EsDeAcodiga));

        public bool EsDeAcodigaIsChanged => GetIsChanged(nameof(EsDeAcodiga));

        public bool EsDeOrientacionLaboral
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsDeOrientacionLaboralOriginalValue => GetOriginalValue<bool>(nameof(EsDeOrientacionLaboral));

        public bool EsDeOrientacionLaboralIsChanged => GetIsChanged(nameof(EsDeOrientacionLaboral));

        public bool EsExterna
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsExternaOriginalValue => GetOriginalValue<bool>(nameof(EsExterna));

        public bool EsExternaIsChanged => GetIsChanged(nameof(EsExterna));
    }
}
