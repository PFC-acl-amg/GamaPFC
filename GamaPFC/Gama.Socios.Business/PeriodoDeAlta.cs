using System;

namespace Gama.Socios.Business
{
    public class PeriodoDeAlta
    {
        public virtual int Id { get; set; }
        public virtual DateTime? FechaDeAlta { get; set; }
        public virtual DateTime? FechaDeBaja { get; set; }

        public virtual Socio Socio { get; set; }
    }
}