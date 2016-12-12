using System;
using System.Collections.Generic;

namespace Gama.Socios.Business
{
    public class PeriodoDeAlta
    {
        public virtual int Id { get; set; }
        public virtual DateTime? FechaDeAlta { get; set; }
        public virtual DateTime? FechaDeBaja { get; set; }

        public virtual IList<Cuota> Cuotas { get; set; }
        public virtual Socio Socio { get; set; }

        public PeriodoDeAlta()
        {
            Cuotas = new List<Cuota>();
        }

        public virtual void AddCuota(Cuota cuota)
        {
            cuota.PeriodoDeAlta = this;
            Cuotas.Add(cuota);
        }

        public virtual List<int> GetMesesAplicables()
        {
            var resultado = new List<int>();



            return resultado;
        }
    }
}