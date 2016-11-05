using FluentNHibernate.Mapping;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.DataAccess.Mappings
{
    public class PersonaMap : ClassMap<Persona>
    {
        public PersonaMap()
        {
            Table("Personas");
            Id(p => p.Id).GeneratedBy.Identity();

            Map(p => p.ComoConocioAGama);
            Map(p => p.DireccionPostal);
            Map(p => p.Email);
            Map(p => p.EstadoCivil);
            Map(p => p.FechaDeNacimiento);
            Map(p => p.Facebook);
            Map(p => p.IdentidadSexual);
            Map(p => p.LinkedIn);
            Map(p => p.Nacionalidad);
            Map(p => p.Nif);
            Map(p => p.NivelAcademico);
            Map(p => p.Nombre).Length(60).Not.Nullable();
            Map(p => p.Ocupacion);
            Map(p => p.OrientacionSexual);
            Map(p => p.Telefono);
            Map(p => p.TieneTrabajo);
            Map(p => p.Twitter);
            Map(p => p.ViaDeAccesoAGama);

            Map(p => p.CreatedAt);
            Map(p => p.UpdatedAt);

            HasMany(x => x.Citas)
                .Cascade.All()
                .Inverse();
        }
    }
}
