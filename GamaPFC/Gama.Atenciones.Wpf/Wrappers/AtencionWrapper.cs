using Core;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Wrappers
{
    public class AtencionWrapper : TimestampedModelWrapper<Atencion>
    {
        public AtencionWrapper(Atencion model) : base(model)
        {
            InitializeComplexProperties(model);
        }

        protected override void InitializeComplexProperties(Atencion model)
        {
            if (model.DerivacionesPropuestas == null)
                throw new ArgumentNullException("DerivacionesPropuestas");

            if (model.DerivacionesRealizadas == null)
                throw new ArgumentNullException("DerivacionesRealizadas");

            DerivacionesPropuestas = new DerivacionWrapper(model.DerivacionesPropuestas);
            RegisterComplex(DerivacionesPropuestas);

            DerivacionesRealizadas = new DerivacionWrapper(model.DerivacionesRealizadas);
            RegisterComplex(DerivacionesRealizadas);
        }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int NumeroDeExpediente
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public DateTime Fecha
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public DateTime FechaOriginalValue => GetOriginalValue<DateTime>(nameof(Fecha));

        public bool FechaIsChanged => GetIsChanged(nameof(Fecha));

        public string Seguimiento
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string SeguimientoOriginalValue => GetOriginalValue<string>(nameof(Seguimiento));

        public bool SeguimientoIsChanged => GetIsChanged(nameof(Seguimiento));

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

        public bool EsDePrevencionParaLaSalur
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsDePrevencionParaLaSalurOriginalValue => GetOriginalValue<bool>(nameof(EsDePrevencionParaLaSalur));

        public bool EsDePrevencionParaLaSalurIsChanged => GetIsChanged(nameof(EsDePrevencionParaLaSalur));

        public bool EsDeFormacion
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsDeFormacionOriginalValue => GetOriginalValue<bool>(nameof(EsDeFormacion));

        public bool EsDeFormacionIsChanged => GetIsChanged(nameof(EsDeFormacion));

        public bool EsDeParticipacion
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsDeParticipacionOriginalValue => GetOriginalValue<bool>(nameof(EsDeParticipacion));

        public bool EsDeParticipacionIsChanged => GetIsChanged(nameof(EsDeParticipacion));

        public bool EsOtra
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EsOtraOriginalValue => GetOriginalValue<bool>(nameof(EsOtra));

        public bool EsOtraIsChanged => GetIsChanged(nameof(EsOtra));

        public string Otra
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string OtraOriginalValue => GetOriginalValue<string>(nameof(Otra));

        public bool OtraIsChanged => GetIsChanged(nameof(Otra));

        public DerivacionWrapper DerivacionesPropuestas { get; private set; }
        public DerivacionWrapper DerivacionesRealizadas { get;  private set;}
    }
}
