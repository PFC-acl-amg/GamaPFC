using System;

namespace Gama.Socios.Business
{
    public class Cuota
    {
        public virtual int Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual double CantidadTotal { get; set; } = 0;
        public virtual double CantidadPagada { get; set; } = 0;
        public virtual bool EstaPagado { get; set; } = false;
        public virtual bool NoContabilizar { get; set; } = false;
        public virtual string Comentarios { get; set; } = "";

        public virtual double CantidadPendienteDePago
        {
            get { return CantidadTotal - CantidadPagada; }
        }

        public virtual PeriodoDeAlta PeriodoDeAlta { get; set; }

        public virtual void CopyValuesFrom(Cuota nuevaCuota)
        {
            Fecha = nuevaCuota.Fecha;
            CantidadTotal = nuevaCuota.CantidadTotal;
            CantidadPagada = nuevaCuota.CantidadPagada;
            EstaPagado = nuevaCuota.EstaPagado;
            NoContabilizar = nuevaCuota.NoContabilizar;
            Comentarios = nuevaCuota.Comentarios;
        }
    }
}