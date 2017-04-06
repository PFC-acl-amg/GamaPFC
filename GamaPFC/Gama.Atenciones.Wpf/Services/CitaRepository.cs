using Core.DataAccess;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Common.CustomControls;
using Core;

namespace Gama.Atenciones.Wpf.Services
{
    public class CitaRepository : NHibernateOneSessionRepository<Cita, int>, ICitaRepository
    {
        public override List<Cita> GetAll()
        {
            try
            {
                var citas = Session.CreateCriteria<Cita>()
                    .SetFetchMode("Persona", NHibernate.FetchMode.Eager)
                    .SetFetchMode("Asistente", NHibernate.FetchMode.Eager)
                    .List<Cita>().ToList();

                foreach (var cita in citas)
                {
                    cita.Decrypt();
                }

                Session.Clear();

                return citas;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override bool Update(Cita entity)
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

                    Session.Update(entity);
                    tx.Commit();
                    Session.Clear();

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

        public List<LookupItem> GetAllForLookup()
        {
            throw new NotImplementedException();
        }
    }
}
