using System;

namespace Gama.Socios.Business
{
    public class Cuota
    {
        public virtual int Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual double CantidadAPagar { get; set; } = 0;
        public virtual double CantidadPagada { get; set; } = 0;

        public virtual double CantidadPendienteDePago
        {
            get { return CantidadAPagar - CantidadPagada; }
        }

        public virtual Socio Socio { get; set; }
    }
}