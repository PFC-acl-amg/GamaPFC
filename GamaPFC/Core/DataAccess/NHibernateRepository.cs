using NHibernate;
using NHibernate.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.DataAccess
{
    public class NHibernateRepository<TEntity, TKey> where TEntity : class
    {
        public ISession _session;
        public IStatelessSession _statelessSession;
        private INHibernateSessionFactory _sessionFactory;

        public NHibernateRepository(INHibernateSessionFactory sessionFactory)
        {
            //_session = session;
            //_statelessSession = statelessSession;
            _sessionFactory = sessionFactory;
            _session = _sessionFactory.OpenSession();
        }

        protected ISession Session
        {
            get
            {
                if (!_session.IsOpen)
                {
                    _session = _sessionFactory.OpenSession();
                }
                return _session;
            }
        }

        public virtual TEntity GetById(int id)
        {
            try
            {
                using (var tx = Session.BeginTransaction())
                {
                    var result = Session.Get<TEntity>((object)id);
                    //_session.Close();
                    tx.Commit();
                    Session.Close();
                    return result;
                }
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
                using (var tx = Session.BeginTransaction())
                {
                    var result = Session.CreateCriteria<TEntity>().List<TEntity>().ToList();
                    Session.Close();
                    //tx.Commit();
                    return result;
                }
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
                    Session.SaveOrUpdate(entity);
                    //_session.Insert(entity);
                    //_statelessSession.Insert(entity);

                    tx.Commit();
                }
                Session.Close();
            }
            catch (Exception ex)
            {
                throw new GenericADOException(ex.Message, ex);
            }
        }

        public bool Update(TEntity entity)
        {
            try
            {
                using (var tx = Session.BeginTransaction())
                {
                    Session.SaveOrUpdate(entity);
                    tx.Commit();
                }
                Session.Close();

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
            //_session.Flush();
        }
    }
}
