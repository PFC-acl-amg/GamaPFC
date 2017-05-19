using Core;
using Core.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Business
{
    public class Asistente : TimestampedModel, IEncryptable
    {
        public virtual int Id { get; set; }
        public virtual string Nif { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Apellidos { get; set; } = "";
        public virtual DateTime? FechaDeNacimiento { get; set; }
        public virtual byte[] Imagen { get; set; }

        public virtual string ComoConocioAGama { get; set; }
        public virtual string NivelAcademico { get; set; }
        public virtual string Ocupacion { get; set; } = "";

        // Dirección postal
        public virtual string Provincia { get; set; } = "";
        public virtual string Municipio { get; set; } = "";
        public virtual string Localidad { get; set; } = "";
        public virtual string CodigoPostal { get; set; } = "";
        public virtual string Calle { get; set; } = "";
        public virtual string Numero { get; set; } = "";
        public virtual string Portal { get; set; } = "";
        public virtual string Piso { get; set; } = "";
        public virtual string Puerta { get; set; } = "";

        // Datos de contacto
        public virtual string TelefonoFijo { get; set; } = "";
        public virtual string TelefonoMovil { get; set; } = "";
        public virtual string TelefonoAlternativo { get; set; } = "";
        public virtual string Email { get; set; }
        public virtual string EmailAlternativo { get; set; }
        public virtual string LinkedIn { get; set; } = "";
        public virtual string Twitter { get; set; } = "";
        public virtual string Facebook { get; set; } = "";
        public virtual string Observaciones { get; set; } = "";


        public virtual IList<Cita> Citas { get; set; }
        
        public Asistente()
        {
            Citas = new List<Cita>();
            EncryptedFields = new List<string>();
            EncryptedFields.AddRange(new[] {
                nameof(Nif),
                nameof(Nombre),
                nameof(Apellidos),
                nameof(Imagen),
                nameof(LinkedIn),
                nameof(Twitter),
                nameof(Facebook),
                nameof(ComoConocioAGama),
                nameof(NivelAcademico),
                nameof(Ocupacion),
                nameof(Provincia),
                nameof(Municipio),
                nameof(Localidad),
                nameof(CodigoPostal),
                nameof(Numero),
                nameof(Piso),
                nameof(Portal),
                nameof(Puerta),
                nameof(TelefonoFijo),
                nameof(TelefonoMovil),
                nameof(TelefonoAlternativo),
                nameof(Email),
                nameof(EmailAlternativo),
                nameof(Observaciones),
                nameof(Citas)
            });

            IsEncrypted = true;
        }

        public virtual void CopyValuesFrom(Asistente other)
        {
            Nif = other.Nif;
            Nombre = other.Nombre;
            Apellidos = other.Apellidos;
            FechaDeNacimiento = other.FechaDeNacimiento;
            Imagen = other.Imagen;

            ComoConocioAGama = other.ComoConocioAGama;
            NivelAcademico = other.NivelAcademico;
            Ocupacion = other.Ocupacion;

            Provincia = other.Provincia;
            Municipio = other.Municipio;
            Localidad = other.Localidad;
            CodigoPostal = other.CodigoPostal;
            Calle = other.Calle;
            Numero = other.Numero;
            Portal = other.Portal;
            Piso = other.Piso;
            Puerta = other.Puerta;

            TelefonoFijo = other.TelefonoFijo;
            TelefonoMovil = other.TelefonoMovil;
            TelefonoAlternativo = other.TelefonoAlternativo;
            Email = other.Email;
            EmailAlternativo = other.EmailAlternativo;
            LinkedIn = other.LinkedIn;
            Twitter = other.Twitter;
            Facebook = other.Facebook;
            Observaciones = other.Observaciones;
        }
    }
}
