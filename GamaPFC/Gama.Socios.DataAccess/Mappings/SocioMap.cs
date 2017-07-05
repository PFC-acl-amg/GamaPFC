using FluentNHibernate.Mapping;
using Gama.Socios.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.DataAccess.Mappings
{
    public class SocioMap : ClassMap<Socio>
    {
        public SocioMap()
        {
            Table("Socios");
            Id(x => x.Id).GeneratedBy.Identity();

            Map(x => x.DireccionPostal).Not.Nullable().Default("");
            Map(x => x.Email).Not.Nullable().Default("");
            Map(x => x.Facebook).Not.Nullable().Default("");
            Map(x => x.FechaDeNacimiento).Not.Nullable();
            Map(x => x.LinkedIn).Not.Nullable().Default("");
            Map(x => x.Nacionalidad).Not.Nullable().Default("");
            Map(x => x.Nif).Not.Nullable().Unique().Length(128);
            Map(x => x.Nombre).Not.Nullable();
            Map(x => x.Telefono).Not.Nullable().Default("");
            Map(x => x.Twitter).Not.Nullable().Default("");
            Map(x => x.EstaDadoDeAlta);
            Map(x => x.Imagen);

            Map(x => x.CreatedAt);
            Map(x => x.UpdatedAt);

            HasMany(x => x.PeriodosDeAlta)
                .Cascade.All()
                .Inverse();
        }
    }
}
