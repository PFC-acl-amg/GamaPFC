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
                .LazyLoad();

            References(x => x.Responsable)
                .Not.LazyLoad()
                .Fetch.Join();

            HasMany(x => x.Seguimiento)
                .Cascade.All()
                //.Cascade.SaveUpdate()
               .Inverse();

            HasMany(x => x.Incidencias)
                //.Cascade.SaveUpdate()
                .Cascade.All()
               .Inverse();

            //HasMany(x => x.Historial)
            //    .Inverse();

            //HasMany(x => x.Mensajes)
            //    .Inverse();
        }
    }
}
