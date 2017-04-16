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
        public virtual string Apellidos { get; set; }
        public virtual string Telefono { get; set; } = "";
        public virtual DateTime? FechaDeNacimiento { get; set; }
        public virtual byte[] Imagen { get; set; }

        public virtual string LinkedIn { get; set; } = "";
        public virtual string Twitter { get; set; } = "";
        public virtual string Facebook { get; set; } = "";

        public virtual ComoConocioAGama ComoConocioAGama { get; set; }
        public virtual NivelAcademico NivelAcademico { get; set; }

        public virtual string Ocupacion { get; set; } = "";

        public virtual string Provincia { get; set; }
        public virtual string Municipio { get; set; }
        public virtual string Localidad { get; set; }
        public virtual string CodigoPostal { get; set; }
        public virtual string Calle { get; set; }
        public virtual string Numero { get; set; }
        public virtual string Portal { get; set; }
        public virtual string Piso { get; set; }
        public virtual string Puerta { get; set; }
        public virtual string TelefonoFijo { get; set; }
        public virtual string TelefonoMovil { get; set; }
        public virtual string TelefonoAlternativo { get; set; }
        public virtual string Email { get; set; }
        public virtual string EmailAlternativo { get; set; }
        public virtual string Observaciones { get; set; }

        public virtual IList<Cita> Citas { get; set; }
        
        public Asistente()
        {
            Citas = new List<Cita>();
            EncryptedFields = new List<string>();

            //EncryptedFields.AddRange(new[] {
            //    nameof(Nombre),
            //    nameof(Nif),
            //    nameof(Telefono),
            //    nameof(Email),
            //    nameof(Imagen)
            //});

            IsEncrypted = true;
        }
    }
}
