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
        private ISession _Session;

        public NHibernateOneSessionRepository()
        {

        }

        public ISession Session
        {
            get { return _Session; }
            set { _Session = value; }
        }

        public virtual TEntity GetById(int id)
        {
            try
            {
                var entity = Session.Get<TEntity>((object)id);

                var encryptableEntity = entity as IEncryptable;
                if (encryptableEntity != null)
                {
                    //encryptableEntity.IsEncrypted = true;
                    //encryptableEntity.Decrypt();
                    if (encryptableEntity.IsEncrypted)
                    {
                        encryptableEntity.Decrypt();
                    }
                }

                Session.Clear();

                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual List<TEntity> GetAll()
        {
            try
            {
                var entities = Session.CreateCriteria<TEntity>().List<TEntity>().ToList();

                foreach (var entity in entities)
                {
                    var encryptableEntity = entity as IEncryptable;
                    if (encryptableEntity != null)
                    {
                        encryptableEntity.IsEncrypted = true;
                        encryptableEntity.Decrypt();
                    }
                    else
                    {
                        // Si una entidad de la colección no lo es, ninguna lo será, ya que
                        // son todas del mismo tipo (misma clase).
                        break;
                    }
                }

                Session.Clear();

                return entities;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void Create(TEntity entity)
        {
            try
            {
                using (var tx = Session.BeginTransaction())
                {
                    var encryptableEntity = entity as IEncryptable;
                    if (encryptableEntity != null)
                    {
                        encryptableEntity.IsEncrypted = false;
                        encryptableEntity.Encrypt();
                    }
                  
                    Session.Save(entity);
                    tx.Commit();

                    Session.Clear();
                    
                    //if (encryptableEntity != null)
                    //{
                    //    encryptableEntity.Decrypt();
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual bool Update(TEntity entity)
        {
            try
            {
                using (var tx = Session.BeginTransaction())
                {
                    var encryptableEntity = entity as IEncryptable;
                    if (encryptableEntity != null)
                    {
                        encryptableEntity.IsEncrypted = false;
                        encryptableEntity.Encrypt();
                    }


                    Session.Clear();
                    Session.Update(entity);
                    tx.Commit();

                    // Volvemos a desencriptar porque el modelo que nos ha llegado
                    // ha sido por referencia, así que hay que devolverlo adecudamente
                    // a las capas visuales...
                    if (encryptableEntity != null)
                    {
                        encryptableEntity.Decrypt();
                    }

                }

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Delete(TEntity entity)
        {
            _Session.Delete(entity);
            _Session.Flush();
        }

        public void Flush()
        {
            try
            {
                Session.Flush();
            }
            catch (Exception ex)
            {
                throw ex;
                //var message = ex.Message;
                //if (message.StartsWith("object references an unsaved transient instance"))
                //    return;
            }
        }

        public int CountAll()
        {
            try {
                int result = 0;
                result = _Session.QueryOver<TEntity>()
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
