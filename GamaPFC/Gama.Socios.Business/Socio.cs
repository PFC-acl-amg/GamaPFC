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
        public virtual string AvatarPath { get; set; }

        public virtual IList<PeriodoDeAlta> PeriodosDeAlta { get; set; }

        public Socio()
        {
            PeriodosDeAlta = new List<PeriodoDeAlta>();
        }

        public virtual bool IsBirthday()
        {
            return FechaDeNacimiento.HasValue
                && FechaDeNacimiento.Value.Date.Month == DateTime.Now.Month
                && FechaDeNacimiento.Value.Date.Day == DateTime.Now.Day;
        }

        public virtual bool EsMoroso()
        {
            return true;
            //return Cuotas.Count(x => x.CantidadTotal > 0) > Socio.MesesParaSerConsideradoMoroso;
        }

        public virtual void CopyValuesFrom(Socio socio)
        {
            DireccionPostal = socio.DireccionPostal;
            Email = socio.Email;
            FechaDeNacimiento = socio.FechaDeNacimiento;
            Facebook = socio.Facebook;
            LinkedIn = socio.LinkedIn;
            Nacionalidad = socio.Nacionalidad;
            Nif = socio.Nif;
            Nombre = socio.Nombre;
            Telefono = socio.Telefono;
            Twitter = socio.Twitter;

            try
            {

                PeriodosDeAlta = new List<PeriodoDeAlta>(socio.PeriodosDeAlta);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void AddPeriodoDeAlta(PeriodoDeAlta periodoDeAlta)
        {
            periodoDeAlta.Socio = this;
            PeriodosDeAlta.Add(periodoDeAlta);
        }
    }
}
