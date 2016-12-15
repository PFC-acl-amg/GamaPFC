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
        public virtual Socio Socio { get; set; }

        public PeriodoDeAlta()
        {
            Cuotas = new List<Cuota>();
        }

        public virtual List<CuotaMonth> MesesAplicables => GetMesesAplicables();

        public virtual void AddCuota(Cuota cuota)
        {
            cuota.PeriodoDeAlta = this;
            Cuotas.Add(cuota);
        }

        public virtual List<CuotaMonth> GetMesesAplicables()
        {
            var resultado = new List<CuotaMonth>();

            var startDate = FechaDeAlta.AddDays(1 - FechaDeAlta.Day);
            var endDate = FechaDeBaja ?? DateTime.Now.AddDays(1 - DateTime.Now.Day);

            while (startDate.Date <= endDate.Date)
            {
                var cuotaMonth = new CuotaMonth
                {
                    Cuota = ((List<Cuota>)Cuotas).Where(x => 
                        DateUtility.IsSameYearMonth(startDate.Date, x.Fecha.Date)).FirstOrDefault()
                };

                resultado.Add(cuotaMonth); 
                startDate.AddMonths(1);
            }

            return resultado;
        }
    }

    public class CuotaMonth
    {
        public Cuota Cuota { get; set; }
    }
}