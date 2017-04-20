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
            Map(p => p.DireccionPostal).Not.Nullable().Default("").Length(128);
            Map(p => p.Email).Length(128);
            Map(p => p.EstadoCivil);
            Map(p => p.FechaDeNacimiento);
            Map(p => p.Facebook).Not.Nullable().Default("").Length(128);
            Map(p => p.IdentidadSexual);
            Map(p => p.LinkedIn).Not.Nullable().Default("").Length(128);
            Map(p => p.Nacionalidad).Not.Nullable().Default("").Length(128);
            Map(x => x.Nif).Not.Nullable().Unique().Length(128);
            Map(p => p.NivelAcademico);
            Map(p => p.Nombre).Length(60).Not.Nullable().Length(128);
            Map(p => p.Ocupacion).Not.Nullable().Default("");
            Map(p => p.OrientacionSexual);
            Map(p => p.Telefono).Not.Nullable().Default("").Length(128);
            Map(p => p.TieneTrabajo);
            Map(p => p.Twitter).Not.Nullable().Default("").Length(128);
            Map(p => p.ViaDeAccesoAGama);
            Map(p => p.Imagen);

            Map(p => p.CreatedAt);
            Map(p => p.UpdatedAt);

            HasMany(x => x.Citas)
                .Not.LazyLoad()
                .Cascade.All()
                .Inverse();
        }
    }
}
