using Core.DataAccess;
using Gama.Cooperacion.Business;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class IncidenciaRepository : NHibernateOneSessionRepository<Incidencia, int>, IIncidenciaRepository
    {
        private List<Incidencia> _Incidencias;

        public IncidenciaRepository(EventAggregator eventAggregator) : base(eventAggregator)
        {

        }

        public List<Incidencia> Incidencias
        {
            get
            {
                if (_Incidencias == null)
                    _Incidencias = base.GetAll();

                return _Incidencias;
            }
            set
            {
                _Incidencias = value;
            }
        }

        private void RaiseActualizarServidor()
        {
            if (CooperacionResources.ClientService != null && CooperacionResources.ClientService.IsConnected())
                CooperacionResources.ClientService.EnviarMensaje($"Cliente {CooperacionResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%COOPERACION");
        }

        public override Incidencia GetById(int id)
        {
            return Incidencias.Find(x => x.Id == id);
        }

        public override List<Incidencia> GetAll()
        {
            return Incidencias;
        }

        /// <summary>
        /// Número de Incidencias creadas por mes en los últimos meses
        /// </summary>
        /// <param name="numeroDeMeses">Número de meses en total a devolver, incluyendo
        /// el mes actual.</param>
        /// <returns></returns>
        public List<int> GetIncidenciasNuevosPorMes(int numeroDeMeses)
        {
            List<int> resultado;
            try
            {
                resultado = Session.CreateSQLQuery(@"
                SELECT COUNT(Id)
                FROM `incidenciastareas` 
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
