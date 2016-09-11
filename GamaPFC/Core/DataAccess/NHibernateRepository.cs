using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataAccess
{
    public class NHibernateRepository<TEntity, TKey> where TEntity : class
    {
        ISession _session;

        public NHibernateRepository(ISessionHelper sessionHelper)
        {
            _session = sessionHelper.Current;
        }

        protected ISession Session { get { return _session; } }

        public virtual TEntity GetById(int id)
        {
            return _session.Get<TEntity>((object)id);
        }

        public List<TEntity> GetAll()
        {
            return _session.CreateCriteria<TEntity>().List<TEntity>().ToList();
        }

        public void Create(TEntity entity)
        {
            try
            {
                using (var tx = _session.BeginTransaction())
                {
                    _session.SaveOrUpdate(entity);
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public bool Update(TEntity entity)
        {
            try
            {
                using (var tx = _session.BeginTransaction())
                {
                    _session.SaveOrUpdate(entity);
                    tx.Commit();
                }

                return true;
            }
            catch (NHibernate.Exceptions.GenericADOException e)
            {
                var message = e.Message;
                return false;
            }
        }

        public void Delete(TEntity entity)
        {
            _session.Delete(entity);
            _session.Flush();
        }
    }
}
