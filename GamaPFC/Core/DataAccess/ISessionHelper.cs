using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataAccess
{
    public interface ISessionHelper
    {
        NHibernate.ISession Current { get; }
        void CreateSession();
        void ClearSession();
        void OpenSession();
        void CloseSession();
    }
}
