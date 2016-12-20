using Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gama.Socios.Business
{
    public class PeriodoDeAlta
    {
        public virtual int Id { get; set; }
        public virtual DateTime FechaDeAlta { get; set; }
        public virtual DateTime? FechaDeBaja { get; set; }

        public virtual IList<Cuota> Cuotas { get; set; }
        public virtual List<Cuota> MesesAplicables { get; set; }
        public virtual Socio Socio { get; set; }

        public PeriodoDeAlta()
        {
            Cuotas = new List<Cuota>();
            MesesAplicables = new List<Cuota>();
        }
        
        public virtual void AddCuota(Cuota cuota)
        {
            cuota.PeriodoDeAlta = this;
            Cuotas.Add(cuota);
        }

        public virtual ICollection<Cuota> GetMesesAplicables()
        {
            var resultado = new List<Cuota>();

            var startDate = FechaDeAlta.AddDays(1 - FechaDeAlta.Day);
            var endDate = FechaDeBaja ?? DateTime.Now.AddDays(1 - DateTime.Now.Day);

            while (startDate.Date <= endDate.Date)
            {
                Cuota nextCuota = ((ICollection<Cuota>)Cuotas).Where(x =>
                        DateUtility.IsSameYearMonth(startDate.Date, x.Fecha.Date)).FirstOrDefault();

                nextCuota = nextCuota ?? new Cuota { Fecha = startDate.Date };
                resultado.Add(nextCuota); 

                startDate = startDate.AddMonths(1);
            }

            return resultado;
        }
    }
}