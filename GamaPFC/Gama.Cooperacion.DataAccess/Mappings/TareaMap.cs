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

            References(x => x.Responsable)
                .Not.LazyLoad()
                .Fetch.Join();

            HasMany(x => x.Historial)
                .Inverse();

            //HasMany(x => x.Mensajes)
            //    .Inverse();
        }
    }

    //public class ForoMap : ClassMap<Foro>
    //{
    //    public ForoMap()
    //    {
    //        Table("Foros");
    //        Id(x => x.Id).GeneratedBy.Identity();

    //        Map(x => x.Titulo).Not.Nullable();
    //        Map(x => x.FechaDePublicacion).Not.Nullable();

    //        HasMany(x => x.Mensajes)
    //           .Inverse();
    //    }
    //}
}
