﻿using MySql.Data.MySqlClient;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Exceptions;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Core.DataAccess
{
    public class NHibernateOneSessionRepository<TEntity, TKey> where TEntity : class
    {
        private ISession _Session;
        protected IEventAggregator _EventAggregator;

        public NHibernateOneSessionRepository(IEventAggregator eventAggregator = null)
        {
            _EventAggregator = eventAggregator;
        }
   
        public ISession Session
        {
            get { return _Session; }
            set { _Session = value; Initialize(); }
        }

        public virtual void Initialize()
        {

        }

        public virtual void UpdateClient()
        {

        }

        public virtual TEntity GetById(int id)
        {
            try
            {
                var entity = Session.Get<TEntity>((object)id);

                var encryptableEntity = entity as IEncryptable;
                if (encryptableEntity != null)
                {
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

                    var timeStampedEntity = entity as TimestampedModel;
                    if (timeStampedEntity != null)
                    {
                        timeStampedEntity.CreatedAt = DateTime.Now;
                    }
                  
                    Session.Save(entity);
                    tx.Commit();

                    if (encryptableEntity != null)
                    {
                        encryptableEntity.Decrypt();
                    }

                    Session.Clear();
                }
            }
            catch (Exception e)
            {
                string ErrorCapturado = e.InnerException.ToString();
                string MensajeError1 = "Duplicate entry";
                string ClaveError1 = "for key 'Dni'";
                if (ErrorCapturado.Contains(MensajeError1))
                    if (ErrorCapturado.Contains(ClaveError1))
                    {
                        MessageBox.Show("El DNI ya está en la Base de Datos");
                       
                        //throw;
                    }
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

                    var timeStampedEntity = entity as TimestampedModel;
                    if (timeStampedEntity != null)
                    {
                        timeStampedEntity.UpdatedAt = DateTime.Now;
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

                    Session.Clear();
                }

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public virtual void Delete(TEntity entity)
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

        public virtual void  DeleteAll()
        {
            try
            {
                var entities = Session.CreateCriteria<TEntity>().List<TEntity>().ToList();
                using (var tx = Session.BeginTransaction())
                {
                    foreach (var entity in entities)
                    {
                        Session.Delete(entity);
                    }

                    tx.Commit();
                    Session.Clear();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
