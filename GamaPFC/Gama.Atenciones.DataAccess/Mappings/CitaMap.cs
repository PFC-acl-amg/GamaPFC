using FluentNHibernate.Mapping;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.DataAccess.Mappings
{
    public class CitaMap : ClassMap<Cita>
    {
        public CitaMap()
        {
            Table("Citas");
            Id(c => c.Id).GeneratedBy.Identity();

            Map(c => c.Asistente);
            Map(c => c.Fin);
            Map(c => c.HaTenidoLugar);
            Map(c => c.Inicio);
            Map(c => c.Sala);

            Map(p => p.CreatedAt);
            Map(p => p.UpdatedAt);

            References(c => c.Persona);

            HasOne(c => c.Atencion).PropertyRef(x => x.Cita).Cascade.All();
        }
    }
}
