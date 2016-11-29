using FluentNHibernate.Mapping;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.DataAccess.Mappings
{
    public class IncidenciaMap : ClassMap<Incidencia>
    {
        public IncidenciaMap()
        {
            Table("IncidenciasTarea");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Descripcion).Not.Nullable();
            Map(x => x.FechaDePublicacion).Not.Nullable();
            Map(x => x.Solucionada);

            Map(x => x.CreatedAt);
            Map(x => x.UpdatedAt);
            //Map(x => x.Tipo).Not.Nullable();

            References(x => x.Tarea)
                .LazyLoad();
        }

    }
}
