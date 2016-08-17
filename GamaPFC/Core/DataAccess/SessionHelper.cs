using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataAccess
{
    public class SessionHelper : ISessionHelper
    {
        /// <summary>
        /// NHibernate Helper
        /// </summary>
        private INHibernateHelper _nHibernateHelper;

        public SessionHelper(INHibernateHelper nHibernateHelper)
        {
            _nHibernateHelper = nHibernateHelper;
        }

        /// <summary>
        /// Retrieve the current ISession.
        /// </summary>
        public ISession Current
        {
            get
            {
                return _nHibernateHelper.GetCurrentSession();
            }
        }

        /// <summary>
        /// Create an ISession.
        /// </summary>
        public void CreateSession()
        {
            _nHibernateHelper.CreateSession();
        }

        /// <summary>
        /// Clear an ISession.
        /// </summary>
        public void ClearSession()
        {
            Current.Clear();
        }

        /// <summary>
        /// Open an ISession.
        /// </summary>
        public void OpenSession()
        {
            _nHibernateHelper.OpenSession();
        }

        /// <summary>
        /// Close an ISession.
        /// </summary>
        public void CloseSession()
        {
            _nHibernateHelper.CloseSession();
        }
    }
}
