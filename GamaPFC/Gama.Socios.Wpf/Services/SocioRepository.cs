using Core.DataAccess;
using Gama.Common.CustomControls;
using Gama.Socios.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.Services
{
    public class SocioRepository : NHibernateOneSessionRepository<Socio, int>, ISocioRepository
    {
        public override bool Update(Socio entity)
        {
            try
            {
                using (var tx = Session.BeginTransaction())
                {
                    //entity.Encrypt();
                    Session.Update(entity);
                    //Session.Merge(entity);
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


        public IEnumerable<int> GetSociosNuevosPorMes(int numeroDeMeses)
        {
            List<int> resultado;
            try
            {
                resultado = Session.CreateSQLQuery(@"
                SELECT COUNT(Id)
                FROM `socios` 
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

        public List<LookupItem> GetAllForLookup()
        {
            var socios = Session.CreateCriteria<Socio>().List<Socio>().Select(
                p => new LookupItem
                {
                    Id = p.Id,
                    DisplayMember1 = p.Nombre,
                    DisplayMember2 = p.Nif
                }).ToList();

            //var result = new List<LookupItem>(socios);

            return socios;
        }
    }
}
