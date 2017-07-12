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
    public class SeguimientoRepository : NHibernateOneSessionRepository<Seguimiento, int>, ISeguimientoRepository
    {
        private List<Seguimiento> _Seguimientos;

        public SeguimientoRepository(EventAggregator eventAggregator) : base(eventAggregator)
        {

        }

        public List<Seguimiento> Seguimientos
        {
            get
            {
                if (_Seguimientos == null)
                    _Seguimientos = base.GetAll();

                return _Seguimientos;
            }
            set
            {
                _Seguimientos = value;
            }
        }

        private void RaiseActualizarServidor()
        {
            if (CooperacionResources.ClientService != null && CooperacionResources.ClientService.IsConnected())
                CooperacionResources.ClientService.EnviarMensaje($"Cliente {CooperacionResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%COOPERACION");
        }

        public override Seguimiento GetById(int id)
        {
            return Seguimientos.Find(x => x.Id == id);
        }

        public override List<Seguimiento> GetAll()
        {
            return Seguimientos;
        }

        /// <summary>
        /// Número de Seguimientos creadas por mes en los últimos meses
        /// </summary>
        /// <param name="numeroDeMeses">Número de meses en total a devolver, incluyendo
        /// el mes actual.</param>
        /// <returns></returns>
        public List<int> GetSeguimientosNuevosPorMes(int numeroDeMeses)
        {
            List<int> resultado;
            try
            {
                resultado = Session.CreateSQLQuery(@"
                SELECT COUNT(Id)
                FROM `seguimientos` 
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
