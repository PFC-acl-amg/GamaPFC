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
            
            References(a => a.Cita).Cascade.Delete();

            HasOne(a => a.DerivacionesPropuestas);
            HasOne(a => a.DerivacionesRealizadas);
        }
    }
}
