using FluentNHibernate.Mapping;
using Gama.Socios.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.DataAccess.Mappings
{
    public class PeriodoDeAltaMap : ClassMap<PeriodoDeAlta>
    {
        public PeriodoDeAltaMap()
        {
            Table("PeriodosDeAlta");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.FechaDeAlta).Nullable();
            Map(x => x.FechaDeBaja).Nullable();

            HasMany(x => x.Cuotas)
                .Cascade.All()
                .Inverse();

            References(x => x.Socio);
        }
    }
}
