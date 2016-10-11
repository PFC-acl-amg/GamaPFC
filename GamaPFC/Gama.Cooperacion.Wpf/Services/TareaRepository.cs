using Core.DataAccess;
using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class TareaRepository : NHibernateOneSessionRepository<Tarea, int>, ITareaRepository
    {
        public TareaRepository()
        {
        }

        /// <summary>
        /// Número de actividades creadas por mes en los últimos meses
        /// </summary>
        /// <param name="numeroDeMeses">Número de meses en total a devolver, incluyendo
        /// el mes actual.</param>
        /// <returns></returns>
        public List<int> GetTareasNuevosPorMes(int numeroDeMeses)
        {
            List<int> resultado;
            try
            {
                resultado = Session.CreateSQLQuery(@"
                SELECT COUNT(Id)
                FROM `tareas` 
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
