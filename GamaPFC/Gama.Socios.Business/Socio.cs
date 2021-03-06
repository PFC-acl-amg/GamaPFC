﻿using Core;
using Core.Encryption;
using Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Business
{
    public class Socio : TimestampedModel
    {
        public virtual string _SavedNif { get; set; } = "";
        public virtual string DireccionPostal { get; set; } = "";
        public virtual string Email { get; set; } = "";
        public virtual DateTime? FechaDeNacimiento { get; set; }
        public virtual string Facebook { get; set; } = "";
        public virtual int Id { get; set; }
        public virtual string LinkedIn { get; set; } = "";
        public virtual string Nacionalidad { get; set; } = "";
        public virtual string Nif { get; set; } = "";
        public virtual string Nombre { get; set; } = "";
        public virtual string Telefono { get; set; } = "";
        public virtual string Twitter { get; set; } = "";
        public virtual bool EstaDadoDeAlta { get; set; }
        public virtual byte[] Imagen { get; set; }
        public virtual DateTime? ImagenUpdatedAt { get; set; }


        public virtual IList<PeriodoDeAlta> PeriodosDeAlta { get; set; }

        public Socio()
        {
            PeriodosDeAlta = new List<PeriodoDeAlta>();
            EncryptedFields = new List<string>();

            EncryptedFields.AddRange(new[] {
                nameof(Nombre),
                nameof(Nif),
                nameof(Facebook),
                nameof(Nacionalidad),
                nameof(LinkedIn),
                nameof(Telefono),
                nameof(Twitter),
                nameof(Email),
                nameof(Imagen),
            });

            IsEncrypted = true;
        }

        public virtual bool IsBirthday()
        {
            return FechaDeNacimiento.HasValue
                && FechaDeNacimiento.Value.Date.Month == DateTime.Now.Month
                && FechaDeNacimiento.Value.Date.Day == DateTime.Now.Day;
        }

        public virtual bool EsMoroso(int mesesParaSerConsideradoMoroso = 3)
        {

            int mesesSinPagar = 0;

            foreach (var periodo in PeriodosDeAlta)
            {
                var fechaDeInicio = periodo.FechaDeAlta;
                var fechaDeFin = DateUtility.MinYearMonth(periodo.FechaDeBaja, DateTime.Now);

                while (DateUtility.IsLessOrEqualThanYearMonth(fechaDeInicio, fechaDeFin))
                {
                    var cuota = periodo.Cuotas.Where(x =>
                        DateUtility.IsSameYearMonth(x.Fecha, fechaDeInicio)).FirstOrDefault();

                    if (cuota == null || (!cuota.EstaPagado && !cuota.NoContabilizar))
                        mesesSinPagar++;

                    fechaDeInicio = fechaDeInicio.AddMonths(1);
                }
            }

            return mesesSinPagar >= mesesParaSerConsideradoMoroso;
        }

        public virtual void CopyValuesFrom(Socio socio)
        {
            DireccionPostal = socio.DireccionPostal;
            Email = socio.Email;
            FechaDeNacimiento = socio.FechaDeNacimiento;
            Facebook = socio.Facebook;
            Imagen = socio.Imagen;
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
            catch (Exception)
            {
                throw;
            }
        }

        public virtual void AddPeriodoDeAlta(PeriodoDeAlta periodoDeAlta)
        {
            periodoDeAlta.Socio = this;
            PeriodosDeAlta.Add(periodoDeAlta);
        }

        public virtual string Edad
        {
            get
            {
                try
                {
                    string result;

                    if (FechaDeNacimiento != null)
                    {
                        var difference = new DateTime(DateTime.Now.Ticks - FechaDeNacimiento.Value.Ticks);
                        result = difference.Year.ToString() + " años";
                    }
                    else
                    {
                        result = "Edad no proporcionada";
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    return "0";
                }
            }
        }

        public virtual int? EdadNumerica
        {
            get
            {
                int? result;

                if (FechaDeNacimiento != null)
                {
                    try
                    {
                        var difference = new DateTime(DateTime.Now.Ticks - FechaDeNacimiento.Value.Ticks);

                        // Así prevenimos que se lance una excepción. Nadie debería tener una fecha de
                        // nacimiento mayor a la fecha actual, pero si el usuario lo introduce por error
                        // se controlorá
                        if (FechaDeNacimiento > DateTime.Now)
                            result = null;
                        else
                            result = difference.Year;
                    }
                    catch (Exception)
                    {
                        result = null;
                    }
                }
                else
                {
                    result = null;
                }

                return result;
            }
        }

    }
}
