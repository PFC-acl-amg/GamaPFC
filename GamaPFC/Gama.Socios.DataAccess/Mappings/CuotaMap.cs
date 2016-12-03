using FluentNHibernate.Mapping;
using Gama.Socios.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.DataAccess.Mappings
{
    public class CuotaMap : ClassMap<Cuota>
    {
        public CuotaMap()
        {
            Table("Cuotas");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.CantidadAPagar).Default("0");
            Map(x => x.CantidadPagada).Default("0");
            Map(x => x.Fecha).Not.Nullable();

            References(x => x.Socio);
        }
    }
}
