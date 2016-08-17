using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataAccess
{
    public interface INHibernateHelper
    {
        ISessionFactory SessionFactory { get; }
        ISession OpenSession();
        void CreateSession();
        void CloseSession();
        ISession GetCurrentSession();
    }
}
