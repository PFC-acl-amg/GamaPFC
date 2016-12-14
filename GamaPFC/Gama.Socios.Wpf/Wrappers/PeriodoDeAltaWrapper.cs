using Core;
using Gama.Socios.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.Wrappers
{
   public  class PeriodoDeAltaWrapper : ModelWrapper<PeriodoDeAlta>
    {
        public PeriodoDeAltaWrapper(PeriodoDeAlta model) : base(model)
        {

        }

        protected override void InitializeCollectionProperties(PeriodoDeAlta model)
        {
            if (model.Cuotas == null)
            {
                throw new ArgumentException("Cuotas");
            }

            this.Cuotas = new ChangeTrackingCollection<CuotaWrapper>
                (model.Cuotas.Select(c => new CuotaWrapper(c)));
            this.RegisterCollection(this.Cuotas, model.Cuotas);
        }

        public int Id
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public DateTime FechaDeAlta
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }

        public DateTime FechaDeAltaOriginalValue => GetOriginalValue<DateTime>(nameof(FechaDeAlta));

        public bool FechaDeAltaIsChanged => GetIsChanged(nameof(FechaDeAlta));

        public DateTime? FechaDeBaja
        {
            get { return GetValue<DateTime?>(); }
            set { SetValue(value); }
        }

        public DateTime? FechaDeBajaOriginalValue => GetOriginalValue<DateTime?>(nameof(FechaDeBaja));

        public bool FechaDeBajaIsChanged => GetIsChanged(nameof(FechaDeBaja));

        public ChangeTrackingCollection<CuotaWrapper> Cuotas { get; private set; }
    }
}
