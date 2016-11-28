using FluentNHibernate.Mapping;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.DataAccess.Mappings
{
    public class SeguimientoMap : ClassMap<Seguimiento>
    {
        public SeguimientoMap()
        {
            Table("Seguimientos");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Descripcion).Not.Nullable();
            Map(x => x.FechaDePublicacion).Not.Nullable();
            Map(x => x.Tipo).Not.Nullable();
        }
        
    }
}
