using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.DataAccess
{
    public interface INHibernateSessionFactory
    {
        ISessionFactory GetSessionFactory();
        ISession OpenSession();
        IStatelessSession OpenStatelessSession();
        ISession GetCurrentSession();
    }
}
