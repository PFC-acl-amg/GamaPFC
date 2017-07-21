using FluentNHibernate.Mapping;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.DataAccess.Mappings
{
    public class TareaMap : ClassMap<Tarea>
    {
        public TareaMap()
        {
            Table("Tareas");
            Id(x => x.Id).GeneratedBy.Identity();

            Map(x => x.Descripcion);
            Map(x => x.FechaDeFinalizacion);
            Map(x => x.HaFinalizado);

            Map(x => x.CreatedAt);
            Map(x => x.UpdatedAt);

            References(x => x.Actividad)
                .Cascade.None()
                .LazyLoad();

            References(x => x.Responsable)
                .Not.LazyLoad()
                .Cascade.None();

            HasMany(x => x.Seguimiento)
                .Not.LazyLoad()
                .Cascade.All()
                .Inverse();

            HasMany(x => x.Incidencias)
                .Not.LazyLoad()
                .Cascade.All()
                .Inverse();

        }
    }
}
