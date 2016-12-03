using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Business
{
    public class Socio : TimestampedModel
    {
        public static int MesesParaSerConsideradoMoroso = 3;

        public virtual string DireccionPostal { get; set; } = "";
        public virtual string Email { get; set; }
        public virtual DateTime? FechaDeNacimiento { get; set; }
        public virtual string Facebook { get; set; } = "";
        public virtual int Id { get; set; }
        public virtual string LinkedIn { get; set; } = "";
        public virtual string Nacionalidad { get; set; } = "";
        public virtual string Nif { get; set; } = "";
        public virtual string Nombre { get; set; }
        public virtual string Telefono { get; set; } = "";
        public virtual string Twitter { get; set; } = "";

        public virtual IList<PeriodoDeAlta> PeriodosDeAlta { get; set; }
        public virtual IList<Cuota> Cuotas { get; set; }

        public Socio()
        {
            Cuotas = new List<Cuota>();
            PeriodosDeAlta = new List<PeriodoDeAlta>();
        }

        public virtual void AddCuota(Cuota cuota)
        {
            cuota.Socio = this;
            Cuotas.Add(cuota);
        }

        public virtual bool IsBirthday()
        {
            return FechaDeNacimiento.HasValue
                && FechaDeNacimiento.Value.Date.Month == DateTime.Now.Month
                && FechaDeNacimiento.Value.Date.Day == DateTime.Now.Day;
        }

        public virtual bool EsMoroso()
        {
            return Cuotas.Count(x => x.CantidadAPagar > 0) > Socio.MesesParaSerConsideradoMoroso;
        }
    }
}
