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
    public class ActividadRepository : NHibernateRepository<Actividad, int>, IActividadRepository
    {
        public ActividadRepository(IStatelessSession statelessSession, ISession session,
            INHibernateSessionFactory sessionFactory) 
            : base(statelessSession, session, sessionFactory)
        {
        }

        //public override Actividad GetById(int id)
        //{
        //    Actividad a = base.GetById(id);
        //    a.Coordinador = Session.CreateCriteri
        //}
    }
}
