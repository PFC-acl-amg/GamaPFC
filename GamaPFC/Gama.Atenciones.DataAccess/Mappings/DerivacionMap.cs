using FluentNHibernate.Mapping;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.DataAccess.Mappings
{
    public class DerivacionMap : ClassMap<Derivacion>
    {
        public DerivacionMap()
        {
            Table("Derivaciones");
            Id(x => x.Id).GeneratedBy.Identity();

            Map(x => x.Tipo);
            Map(x => x.EsDeFormacion);
            Map(x => x.EsDeOrientacionLaboral);
            Map(x => x.EsJuridica);
            Map(x => x.EsPsicologica);
            Map(x => x.EsSocial);
            Map(x => x.EsExterna);
            Map(x => x.Externa);

            References(x => x.Atencion);
        }
    }
}
