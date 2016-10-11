using NHibernate;
using NHibernate.Criterion;
using NHibernate.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataAccess
{
    public class NHibernateOneSessionRepository<TEntity, TKey> where TEntity : class
    {
        private ISession _session;

        public NHibernateOneSessionRepository()
        {

        }

        public ISession Session
        {
            get { return _session; }
            set { _session = value; }
        }

        public virtual TEntity GetById(int id)
        {
            try
            {
                //using (var tx = Session.BeginTransaction())
                //{
                    var result = Session.Get<TEntity>((object)id);
                    //tx.Commit();
                    return result;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TEntity> GetAll()
        {
            try
            {
                //using (var tx = Session.BeginTransaction())
                //{
                    var result = Session.CreateCriteria<TEntity>().List<TEntity>().ToList();
                    //tx.Commit();
                    return result;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Create(TEntity entity)
        {
            try
            {
                //using (var tx = _session.BeginTransaction())
                //{
                using (var tx = Session.BeginTransaction())
                {
                    Session.Save(entity);
                    //_session.Insert(entity);
                    //_statelessSession.Insert(entity);

                    tx.Commit();
                    //Session.Flush();
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (message.StartsWith("object references an unsaved transient instance"))
                    return;
            }
        }

        public virtual bool Update(TEntity entity)
        {
            try
            {
                using (var tx = Session.BeginTransaction())
                {
                    Session.Update(entity);
                    tx.Commit();
                }

                return true;
            }
            catch (NHibernate.Exceptions.GenericADOException e)
            {
                var message = e.Message;
                throw e;
            }
        }

        public void Delete(TEntity entity)
        {
            _session.Delete(entity);
            _session.Flush();
        }

        public void Flush()
        {
            try
            {
                Session.Flush();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (message.StartsWith("object references an unsaved transient instance"))
                    return;
            }
        }

        public int CountAll()
        {
            try {
                int result = 0;
                result = _session.QueryOver<TEntity>()
                    .Select(Projections.RowCount())
                    .FutureValue<int>().Value;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
