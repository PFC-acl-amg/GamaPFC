﻿using Core;
using Core.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Business
{
    public class Socio : TimestampedModel, IEncryptable
    {
        public virtual string DireccionPostal { get; set; } = "";
        public virtual string Email { get; set; } = "";
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

        public virtual List<string> EncryptedFields { get; set; }

        public virtual bool IsEncrypted { get; set; }

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
                mesesSinPagar += periodo.Cuotas.Count(x => x.EstaPagado == false);
            }

            return mesesSinPagar >= mesesParaSerConsideradoMoroso;
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

        public virtual void Encrypt()
        {
            if (IsEncrypted)
                return;

            foreach (var propertyName in EncryptedFields)
            {
                var propertyInfo = this.GetType().GetProperty(propertyName);
                var propertyValue = propertyInfo.GetValue(this, null);

                if (propertyValue != null)
                {
                    propertyInfo.SetValue(this, StringCipher.Encrypt(propertyValue.ToString()));
                }
            }

            IsEncrypted = true;
        }

        public virtual Socio DecryptFluent()
        {
            Decrypt();
            return this;
        }

        public virtual void Decrypt()
        {
            try
            {
                if (!IsEncrypted)
                    return;

                foreach (var propertyName in EncryptedFields)
                {
                    var propertyInfo = this.GetType().GetProperty(propertyName);
                    var propertyValue = propertyInfo.GetValue(this, null);

                    if (propertyValue != null)
                    {
                        propertyInfo.SetValue(this, StringCipher.Decrypt(propertyValue.ToString()));
                    }
                }

                IsEncrypted = false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
