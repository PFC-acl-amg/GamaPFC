using Core.DataAccess;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Common.CustomControls;
using Core;
using NHibernate.Criterion;

namespace Gama.Atenciones.Wpf.Services
{
    public class AtencionRepository : NHibernateOneSessionRepository<Atencion, int>, IAtencionRepository
    {
        public List<LookupItem> GetAllForLookup()
        {
            throw new NotImplementedException();
        }

        public override Atencion GetById(int id)
        {
            try
            {
                var entity = Session.Get<Atencion>((object)id);
                
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

                //Session.Clear();

                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<int> GetAtencionesNuevasPorMes(int numeroDeMeses)
        {
            List<int> resultado;
            try
            {
                resultado = Session.CreateSQLQuery(@"
                SELECT COUNT(Id)
                FROM `atenciones` 
                GROUP BY
                    YEAR(CreatedAt), 
                    MONTH(CreatedAt) 
                ORDER BY 
                    YEAR(CreatedAt) DESC, 
                    MONTH(CreatedAt) DESC")
                        .SetMaxResults(numeroDeMeses)
                        .List<object>()
                        .Select(r => int.Parse(r.ToString()))
                        .ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultado;
        }
    }
}
