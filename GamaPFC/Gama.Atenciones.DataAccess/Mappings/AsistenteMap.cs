using FluentNHibernate.Mapping;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.DataAccess.Mappings
{
    public class AsistenteMap : ClassMap<Asistente>
    {
        public AsistenteMap()
        {
            Table("Asistentes");
            Id(p => p.Id).GeneratedBy.Identity();

            Map(x => x.Nif).Not.Nullable().Unique().Length(128);
            Map(p => p.Nombre).Length(60).Not.Nullable().Length(128);
            Map(p => p.Apellidos).Not.Nullable().Default("").Length(120);
            Map(p => p.FechaDeNacimiento);

            Map(p => p.ComoConocioAGama);
            Map(p => p.NivelAcademico);
            Map(p => p.Ocupacion).Not.Nullable().Default("");

            Map(p => p.Provincia).Not.Nullable().Default("").Length(128);
            Map(p => p.Municipio).Not.Nullable().Default("").Length(128);
            Map(p => p.Localidad).Not.Nullable().Default("").Length(128);
            Map(p => p.CodigoPostal).Not.Nullable().Default("").Length(128);
            Map(p => p.Calle).Not.Nullable().Default("").Length(128);
            Map(p => p.Numero).Not.Nullable().Default("").Length(128);
            Map(p => p.Portal).Not.Nullable().Default("").Length(128);
            Map(p => p.Piso).Not.Nullable().Default("").Length(128);
            Map(p => p.Puerta).Not.Nullable().Default("").Length(128);

            Map(p => p.TelefonoFijo).Not.Nullable().Default("").Length(128);
            Map(p => p.TelefonoMovil).Not.Nullable().Default("").Length(128);
            Map(p => p.TelefonoAlternativo).Not.Nullable().Default("").Length(128);
            Map(p => p.Email).Length(128);
            Map(p => p.EmailAlternativo).Length(128);
            Map(p => p.Facebook).Not.Nullable().Default("").Length(128);
            Map(p => p.Twitter).Not.Nullable().Default("").Length(128);
            Map(p => p.LinkedIn).Not.Nullable().Default("").Length(128);
            Map(p => p.Observaciones).Not.Nullable().Default("").CustomSqlType("MEDIUMTEXT");

            Map(p => p.Imagen);

            Map(p => p.CreatedAt);
            Map(p => p.UpdatedAt);

            HasMany(x => x.Citas)
                .Not.LazyLoad()
                .Cascade.None()
                .Inverse();
        }
    }
}
