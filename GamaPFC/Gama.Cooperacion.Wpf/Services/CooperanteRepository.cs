using Core.DataAccess;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.DataAccess;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class CooperanteRepository : NHibernateOneSessionRepository<Cooperante, int>, ICooperanteRepository
    {

        public CooperanteRepository()
        {
        }
    }
}
