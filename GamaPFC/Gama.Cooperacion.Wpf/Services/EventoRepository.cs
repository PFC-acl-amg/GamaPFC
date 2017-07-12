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
    //class EventoRepository
    //{
    //}
    public class EventoRepository : NHibernateOneSessionRepository<Evento, int>, IEventoRepository
    {
        private List<Evento> _Eventos;

        public EventoRepository(EventAggregator eventAggregator) : base(eventAggregator)
        {

        }

        public List<Evento> Eventos
        {
            get
            {
                if (_Eventos == null)
                    _Eventos = base.GetAll();

                return _Eventos;
            }
            set
            {
                _Eventos = value;
            }
        }

        private void RaiseActualizarServidor()
        {
            if (CooperacionResources.ClientService != null && CooperacionResources.ClientService.IsConnected())
                CooperacionResources.ClientService.EnviarMensaje($"Cliente {CooperacionResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%COOPERACION");
        }

        public override Evento GetById(int id)
        {
            return Eventos.Find(x => x.Id == id);
        }

        public override List<Evento> GetAll()
        {
            return Eventos;
        }

        /// <summary>
        /// Número de actividades creadas por mes en los últimos meses
        /// </summary>
        /// <param name="numeroDeMeses">Número de meses en total a devolver, incluyendo
        /// el mes actual.</param>
        /// <returns></returns>
        public List<int> GetEventosNuevosPorMes(int numeroDeMeses)
        {
            List<int> resultado;
            try
            {
                resultado = Session.CreateSQLQuery(@"
                SELECT COUNT(Id)
                FROM `eventos` 
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
