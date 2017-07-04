using Core;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public bool EsDeFormacion
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsDeFormacionOriginalValue => GetOriginalValue<bool>(nameof(EsDeFormacion));

        public bool EsDeFormacionIsChanged => GetIsChanged(nameof(EsDeFormacion));

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

        public string Externa
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string ExternaOriginalValue => GetOriginalValue<string>(nameof(Externa));

        public bool ExternaIsChanged => GetIsChanged(nameof(Externa));

        public bool EsSocial_Realizada
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsSocial_RealizadaOriginalValue => GetOriginalValue<bool>(nameof(EsSocial_Realizada));

        public bool EsSocial_RealizadaIsChanged => GetIsChanged(nameof(EsSocial_Realizada));

        public bool EsJuridica_Realizada
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsJuridica_RealizadaOriginalValue => GetOriginalValue<bool>(nameof(EsJuridica_Realizada));

        public bool EsJuridica_RealizadaIsChanged => GetIsChanged(nameof(EsJuridica_Realizada));

        public bool EsPsicologica_Realizada
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsPsicologica_RealizadaOriginalValue => GetOriginalValue<bool>(nameof(EsPsicologica_Realizada));

        public bool EsPsicologica_RealizadaIsChanged => GetIsChanged(nameof(EsPsicologica_Realizada));

        public bool EsDeFormacion_Realizada
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsDeFormacion_RealizadaOriginalValue => GetOriginalValue<bool>(nameof(EsDeFormacion_Realizada));

        public bool EsDeFormacion_RealizadaIsChanged => GetIsChanged(nameof(EsDeFormacion_Realizada));

        public bool EsDeOrientacionLaboral_Realizada
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsDeOrientacionLaboral_RealizadaOriginalValue => GetOriginalValue<bool>(nameof(EsDeOrientacionLaboral_Realizada));

        public bool EsDeOrientacionLaboral_RealizadaIsChanged => GetIsChanged(nameof(EsDeOrientacionLaboral_Realizada));

        public bool EsExterna_Realizada
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsExterna_RealizadaOriginalValue => GetOriginalValue<bool>(nameof(EsExterna_Realizada));

        public bool EsExterna_RealizadaIsChanged => GetIsChanged(nameof(EsExterna_Realizada));

        public string Externa_Realizada
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Externa_RealizadaOriginalValue => GetOriginalValue<string>(nameof(Externa_Realizada));

        public bool Externa_RealizadaIsChanged => GetIsChanged(nameof(Externa_Realizada));

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new ValidationResult[0];
        }
    }
}
