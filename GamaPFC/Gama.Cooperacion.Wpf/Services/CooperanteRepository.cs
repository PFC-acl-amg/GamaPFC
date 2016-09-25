using Core.DataAccess;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.DataAccess;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class CooperanteRepository : NHibernateOneSessionRepository<Cooperante, int>, ICooperanteRepository
    {
        public CooperanteRepository()
        {
        }

        public List<int> GetCooperantesNuevosPorMes(int numeroDeMeses)
        {
            List<int> resultado;
            try
            {
                resultado = Session.CreateSQLQuery(@"
                SELECT COUNT(Id)
                FROM `cooperantes` 
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
