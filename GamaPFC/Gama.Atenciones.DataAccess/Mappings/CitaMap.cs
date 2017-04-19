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

            Map(c => c.AsistenteEnTexto).Not.Nullable().Default("");
            Map(c => c.Fin);
            Map(c => c.HaTenidoLugar).Not.Nullable().Default("0");
            Map(c => c.Fecha);
            Map(c => c.Sala).Not.Nullable().Default("");
            Map(x => x.Hora).Not.Nullable().Default("0");
            Map(x => x.Minutos).Not.Nullable().Default("0");

            Map(p => p.CreatedAt);
            Map(p => p.UpdatedAt);

            References(c => c.Persona)
                .Not.LazyLoad();

            References(x => x.Asistente)
                .Cascade.None()
                .Not.LazyLoad();

            HasOne(c => c.Atencion).PropertyRef(x => x.Cita).Cascade.All();
        }
    }
}
