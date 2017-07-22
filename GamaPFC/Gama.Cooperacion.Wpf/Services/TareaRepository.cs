using Core.DataAccess;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.Wpf.Wrappers;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class TareaRepository : NHibernateOneSessionRepository<Tarea, int>, ITareaRepository
    {
        private List<Tarea> _Tareas;

        public TareaRepository(EventAggregator eventAggregator) : base(eventAggregator)
        {
            eventAggregator.GetEvent<NuevaTareaCreadaEvent>().Subscribe(OnActualizarTareasDisponibles);
        }
        private void OnActualizarTareasDisponibles(TareaWrapper wrapper)
        {
            _Tareas = base.GetAll();
        }
        public List<Tarea> Tareas
        {
            get
            {
                if (_Tareas == null)
                    _Tareas = base.GetAll();

                return _Tareas;
            }
            set
            {
                _Tareas = value;
            }
        }

        private void RaiseActualizarServidor()
        {
            if (CooperacionResources.ClientService != null && CooperacionResources.ClientService.IsConnected())
                CooperacionResources.ClientService.EnviarMensaje($"Cliente {CooperacionResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%COOPERACION");
        }

        public override Tarea GetById(int id)
        {
            return Tareas.Find(x => x.Id == id);
        }

        public override List<Tarea> GetAll()
        {
            return Tareas;
        }

        /// <summary>
        /// Número de Tareas creadas por mes en los últimos meses
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
