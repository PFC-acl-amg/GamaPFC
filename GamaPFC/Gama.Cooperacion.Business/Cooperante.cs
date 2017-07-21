using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Business
{
    public class Cooperante : TimestampedModel
    {
        public virtual IList<Actividad> ActividadesDeQueEsCoordinador { get; protected set; }
        public virtual IList<Actividad> ActividadesEnQueParticipa { get; protected set; }
        public virtual string Apellido { get; set; } = "";
        public virtual string Dni { get; set; } = "";
        public virtual int Id { get; set; }
        public virtual string Nombre { get; set; } = "";
        public virtual string Telefono { get; set; } = "";
        public virtual byte[] Foto { get; set; }
        public virtual DateTime? FotoUpdatedAt { get; set; }
        public virtual string Observaciones { get; set; } = "";
        // Campos Nuevos añadidos 05.04.2017
        public virtual string Provincia { get; set; } = "";
        public virtual string Municipio { get; set; } = "";
        public virtual string CP { get; set; } = "";
        public virtual string Localidad { get; set; } = "";
        public virtual string Calle { get; set; } = "";
        public virtual string Numero { get; set; } = "";
        public virtual string Portal { get; set; } = "";
        public virtual string Piso { get; set; } = "";
        public virtual string Puerta { get; set; } = "";
        public virtual DateTime? FechaDeNacimiento { get; set; }
        public virtual string TelefonoMovil { get; set; } = "";
        public virtual string TelefonoAlternativo { get; set; } = "";
        public virtual string Email { get; set; } = "";
        public virtual string EmailAlternativo { get; set; } = "";
        // Fin Campos Nuevos

        public Cooperante()
        {
            ActividadesDeQueEsCoordinador = new List<Actividad>();
            ActividadesEnQueParticipa = new List<Actividad>();
            //EncryptedFields = new List<string>();
            //EncryptedFields.AddRange(new[] {
            //    nameof(Dni),
            //    nameof(Nombre),
            //    nameof(Apellido),
            //    nameof(Foto),
            //    nameof(Email),
            //    nameof(EmailAlternativo),
            //    nameof(Provincia),
            //    nameof(Municipio),
            //    nameof(Localidad),
            //    nameof(CP),
            //    nameof(Numero),
            //    nameof(Piso),
            //    nameof(Portal),
            //    nameof(Puerta),
            //    nameof(Telefono),
            //    nameof(TelefonoMovil),
            //    nameof(TelefonoAlternativo),
            //    nameof(Email),
            //    nameof(EmailAlternativo),
            //    nameof(Observaciones)
            //});
            //IsEncrypted = true;
        }
    }
}
