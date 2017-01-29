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
                var entities = Session.CreateCriteria<Cita>()
                    .SetFetchMode("Persona", NHibernate.FetchMode.Eager).List<Cita>().ToList();

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

        public List<LookupItem> GetAllForLookup()
        {
            throw new NotImplementedException();
        }
    }
}
