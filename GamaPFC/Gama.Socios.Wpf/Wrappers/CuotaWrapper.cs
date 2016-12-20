using Core;
using Gama.Socios.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.Wrappers
{
    public class CuotaWrapper : ModelWrapper<Cuota>
    {
        public CuotaWrapper(Cuota model) : base(model)
        {

        }

        public int Id
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

        public double CantidadTotal
        {
            get { return GetValue<double>(); }
            set { SetValue(value); }
        }

        public double CantidadTotalOriginalValue => GetOriginalValue<double>(nameof(CantidadTotal));

        public bool CantidadTotalIsChanged => GetIsChanged(nameof(CantidadTotal));

        public double CantidadPagada
        {
            get { return GetValue<double>(); }
            set { SetValue(value); }
        }

        public double CantidadPagadaOriginalValue => GetOriginalValue<double>(nameof(CantidadPagada));

        public bool CantidadPagadaIsChanged => GetIsChanged(nameof(CantidadPagada));

        public double CantidadPendienteDePago
        {
            get { return GetValue<double>(); }
        }

        public bool EstaPagado
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool EstaPagadoOriginalValue => GetOriginalValue<bool>(nameof(EstaPagado));

        public bool EstaPagadoIsChanged => GetIsChanged(nameof(EstaPagado));

        public bool NoContabilizar
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public bool NoContabilizarOriginalValue => GetOriginalValue<bool>(nameof(NoContabilizar));

        public bool NoContabilizarIsChanged => GetIsChanged(nameof(NoContabilizar));
    }
}
