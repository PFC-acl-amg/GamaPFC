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
            Map(x => x.EsDeFormacion_Realizada);
            Map(x => x.EsDeOrientacionLaboral_Realizada);
            Map(x => x.EsJuridica_Realizada);
            Map(x => x.EsPsicologica_Realizada);
            Map(x => x.EsSocial_Realizada);
            Map(x => x.EsExterna_Realizada);
            Map(x => x.Externa_Realizada);

            References(x => x.Atencion, "Atencion_id").Unique();
        }
    }
}
