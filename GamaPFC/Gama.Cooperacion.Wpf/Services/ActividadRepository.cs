using Core.DataAccess;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class ActividadRepository : NHibernateRepository<Actividad, int>
    {
        public ActividadRepository(ISessionHelper sessionHelper) : base(sessionHelper)
        {
        }
    }
}
