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
    public class ForoRepository : NHibernateOneSessionRepository<Foro, int>, IForoRepository
    {
        private List<Foro> _Foros;

        public ForoRepository(EventAggregator eventAggregator) : base(eventAggregator)
        {

        }

        public List<Foro> Foros
        {
            get
            {
                if (_Foros == null)
                    _Foros = base.GetAll();

                return _Foros;
            }
            set
            {
                _Foros = value;
            }
        }

        private void RaiseActualizarServidor()
        {
            if (CooperacionResources.ClientService != null && CooperacionResources.ClientService.IsConnected())
                CooperacionResources.ClientService.EnviarMensaje($"Cliente {CooperacionResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%COOPERACION");
        }

        public override Foro GetById(int id)
        {
            return Foros.Find(x => x.Id == id);
        }

        public override List<Foro> GetAll()
        {
            return Foros;
        }
        /// <summary>
        /// Número de Foros creadas por mes en los últimos meses
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
                FROM `foros` 
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
