using System;

namespace Gama.Socios.Business
{
    public class Cuota
    {
        public virtual int Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual double CantidadTotal { get; set; } = 0;
        public virtual double CantidadPagada { get; set; } = 0;
        public virtual bool EstaPagado { get; set; }
        public virtual bool NoContabilizar { get; set; }
        public virtual string Comentarios { get; set; }

        public virtual double CantidadPendienteDePago
        {
            get { return CantidadTotal - CantidadPagada; }
        }

        public virtual PeriodoDeAlta PeriodoDeAlta { get; set; }
    }
}