using FluentNHibernate.Mapping;
using Gama.Common.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Common.Data
{
    public class UsuarioMap : ClassMap<Usuario>
    {
        public UsuarioMap()
        {
            Table("Usuarios");
            Map(x => x.Nombre).Unique().Not.Nullable();
            Map(x => x.Password).Not.Nullable();
        }
    }
}
