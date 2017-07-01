using FluentNHibernate.Mapping;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.DataAccess.Mappings
{
    public class ActividadMap : ClassMap<Actividad>
    {
        public ActividadMap()
        {
            Table("Actividades");
            Id(x => x.Id).GeneratedBy.Identity();

            Map(x => x.Descripcion).CustomSqlType("TEXT");
            Map(x => x.Estado);
            Map(x => x.FechaDeInicio);
            Map(x => x.FechaDeFin);
            Map(x => x.Titulo);
            Map(x => x.CreatedAt);
            Map(x => x.UpdatedAt);

            References(x => x.Coordinador)
                .Not.LazyLoad()
                .Cascade.None();

            HasMany(x => x.Eventos)
                .Not.LazyLoad()
                .Cascade.All()
                .Inverse();

            HasMany(x => x.Foros)
                .Not.LazyLoad()
                .Cascade.All()
                .Inverse();

            HasMany(x => x.Tareas)
                .Not.LazyLoad()
                .Cascade.All();

            HasManyToMany(x => x.Cooperantes)
                .Table("CooperanteParticipaEnActividad")
                .Not.LazyLoad()
                .Cascade.None();
                
        }
    }
}
