using FluentNHibernate.Mapping;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Mappings
{
    public class ActividadMap : ClassMap<Actividad>
    {
        public ActividadMap()
        {
            Table("Actividades");
            Id(x => x.Id).GeneratedBy.Identity();

            Map(x => x.Descripcion);
            Map(x => x.Titulo);
        }
    }
}
