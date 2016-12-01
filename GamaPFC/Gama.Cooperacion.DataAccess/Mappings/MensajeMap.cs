using FluentNHibernate.Mapping;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.DataAccess.Mappings
{
    public class MensajeMap : ClassMap<Mensaje>
    {
        public MensajeMap()
        {
            Table("Mensajes");
            Id(x => x.Id).GeneratedBy.Identity();

            Map(x => x.Titulo).Not.Nullable();
            Map(x => x.FechaDePublicacion).Not.Nullable();

            //References(x => x.Foro)
            //    .LazyLoad();
        }

    }
}
