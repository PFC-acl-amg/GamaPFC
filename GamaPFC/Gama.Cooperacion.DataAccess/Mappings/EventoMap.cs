using FluentNHibernate.Mapping;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.DataAccess.Mappings
{
    public class EventoMap : ClassMap<Evento>
    {
        public EventoMap()
        {
            Table("Eventos");
            Id(x => x.Id).GeneratedBy.Identity();

            Map(x => x.Titulo);
            Map(x => x.FechaDePublicacion);
            Map(x => x.Ocurrencia);
        }
    }
}
