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

            if (model.MesesAplicables == null)
            {
                throw new ArgumentException("MesesAplicables");
            }

            this.MesesAplicables = new ChangeTrackingCollection<CuotaWrapper>
                (model.GetMesesAplicables().Select(x => new CuotaWrapper(x)));
            this.RegisterCollection(this.MesesAplicables, model.MesesAplicables);
        }

        public override void AcceptChanges()
        {
            MesesAplicables.Clear();
            foreach (var mesAplicable in Model.GetMesesAplicables())
            {
                MesesAplicables.Add(new CuotaWrapper(mesAplicable));
            }

            base.AcceptChanges();

            //this.MesesAplicables = new ChangeTrackingCollection<CuotaWrapper>
            //    (Model.GetMesesAplicables().Select(x => new CuotaWrapper(x)));
            //this.RegisterCollection(this.MesesAplicables, Model.MesesAplicables);
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

        public ChangeTrackingCollection<CuotaWrapper> MesesAplicables { get; private set; }
    }
}
