using FluentNHibernate;
using FluentNHibernate.Mapping;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.DataAccess.Mappings
{
    public class AtencionMap : ClassMap<Atencion>
    {
        public AtencionMap()
        {
            Table("Atenciones");
            Id(x => x.Id).GeneratedBy.Identity();

            Map(a => a.NumeroDeExpediente);
            Map(a => a.Fecha);
            Map(a => a.Seguimiento);

            Map(a => a.EsDeAcogida);
            Map(a => a.EsDeFormacion);
            Map(a => a.EsDeOrientacionLaboral);
            Map(a => a.EsDeParticipacion);
            Map(a => a.EsDePrevencionParaLaSalud);
            Map(a => a.EsJuridica);
            Map(a => a.EsOtra);
            Map(a => a.EsPsicologica);
            Map(a => a.EsSocial);
            Map(a => a.Otra);

            Map(p => p.CreatedAt);
            Map(p => p.UpdatedAt);

            References(a => a.Cita, "Cita_id").Unique();

            HasOne(a => a.Derivacion).PropertyRef(c => c.Atencion);
        }
    }
}
