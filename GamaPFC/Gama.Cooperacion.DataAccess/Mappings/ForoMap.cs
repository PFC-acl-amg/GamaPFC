using FluentNHibernate.Mapping;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.DataAccess.Mappings
{
    public class ForoMap : ClassMap<Foro>
    {
        public ForoMap()
        {
            Table("Foros");
            Id(x => x.Id).GeneratedBy.Identity();

            Map(x => x.Titulo).Not.Nullable();
            Map(x => x.FechaDePublicacion).Not.Nullable();

            Map(x => x.CreatedAt);
            Map(x => x.UpdatedAt);

            References(x => x.Actividad)
                .LazyLoad();

            HasMany(x => x.Mensajes)
                //.Cascade.SaveUpdate()
                .Not.LazyLoad()
                .Cascade.All()
                .Inverse();
        }
    }
}
