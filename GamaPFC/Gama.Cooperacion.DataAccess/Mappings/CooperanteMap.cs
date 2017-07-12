using FluentNHibernate.Mapping;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.DataAccess.Mappings
{
    public class CooperanteMap : ClassMap<Cooperante>
    {
        public CooperanteMap()
        {
            Table("Cooperantes");
            Id(x => x.Id).GeneratedBy.Identity();
                
            Map(x => x.Apellido);
            Map(x => x.Dni).Unique();
            Map(x => x.Nombre);
            Map(x => x.Observaciones);
            Map(x => x.Telefono);
            Map(x => x.Foto);
            Map(x => x.FotoUpdatedAt);
            Map(x => x.CreatedAt);
            Map(x => x.UpdatedAt);
            // Campos nuevos
            Map(x => x.FechaDeNacimiento);
            Map(x => x.Provincia);
            Map(x => x.Municipio);
            Map(x => x.CP);
            Map(x => x.Localidad);
            Map(x => x.Calle);
            Map(x => x.Numero);
            Map(x => x.Portal);
            Map(x => x.Piso);
            Map(x => x.Puerta);
            Map(x => x.TelefonoMovil);
            Map(x => x.TelefonoAlternativo);
            Map(x => x.Email);
            Map(x => x.EmailAlternativo);
            
            // Fin campos nuevos



            HasMany(x => x.ActividadesDeQueEsCoordinador)
                .Not.LazyLoad()
                .Cascade.None()
                .Inverse();

            HasManyToMany(x => x.ActividadesEnQueParticipa)
                .Table("CooperanteParticipaEnActividad")
                .Not.LazyLoad()
                .Cascade.None()
                .Inverse();
        }
    }
}
