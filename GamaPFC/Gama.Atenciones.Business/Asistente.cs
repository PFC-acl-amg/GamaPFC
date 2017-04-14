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
        public virtual string Email { get; set; }
        public virtual string Telefono { get; set; } = "";
        public virtual DateTime? FechaDeNacimiento { get; set; }
        public virtual byte[] Imagen { get; set; }

        public virtual string LinkedIn { get; set; } = "";
        public virtual string Twitter { get; set; } = "";
        public virtual string Facebook { get; set; } = "";

        public virtual ComoConocioAGama ComoConocioAGama { get; set; }
        public virtual NivelAcademico NivelAcademico { get; set; }

        public virtual IList<Cita> Citas { get; set; }
        
        public Asistente()
        {
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
