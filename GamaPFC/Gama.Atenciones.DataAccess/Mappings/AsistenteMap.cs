using FluentNHibernate.Mapping;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.DataAccess.Mappings
{
    public class AsistenteMap : ClassMap<Asistente>
    {
        public AsistenteMap()
        {
            Table("Asistentes");
            Id(p => p.Id).GeneratedBy.Identity();

            Map(x => x.Nombre);
            Map(x => x.Telefono);
            Map(x => x.Imagen);
            Map(x => x.Nif);

            Map(p => p.CreatedAt);
            Map(p => p.UpdatedAt);

            HasMany(x => x.Citas)
                .Not.LazyLoad()
                .Cascade.None()
                .Inverse();
        }
    }
}
