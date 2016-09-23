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
            Map(x => x.CreatedAt);
            Map(x => x.UpdatedAt);

            HasMany(x => x.ActividadesDeQueEsCoordinador)
                .LazyLoad()
                .Inverse();

            HasManyToMany(x => x.ActividadesEnQueParticipa)
                .Table("CooperanteParticipaEnActividad")
                .LazyLoad()
                .Inverse();
        }
    }
}
