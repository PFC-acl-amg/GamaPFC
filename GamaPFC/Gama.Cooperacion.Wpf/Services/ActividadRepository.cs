using Core.DataAccess;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.DataAccess;
using Gama.Cooperacion.Wpf.Eventos;
using MySql.Data.MySqlClient;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class ActividadRepository : NHibernateOneSessionRepository<Actividad, int>, IActividadRepository
    {
        private List<Actividad> _Actividades;

        public ActividadRepository(EventAggregator eventAggregator) : base(eventAggregator)
        {

        }

        public List<Actividad> Actividades
        {
            get
            {
                if (_Actividades == null)
                    _Actividades = base.GetAll();

                return _Actividades;
            }
            set
            {
                _Actividades = value;
            }
        }

        private void RaiseActualizarServidor()
        {
            if (CooperacionResources.ClientService != null && CooperacionResources.ClientService.IsConnected())
                CooperacionResources.ClientService.EnviarMensaje($"Cliente {CooperacionResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%COOPERACION");
        }

        public override Actividad GetById(int id)
        {
            return Actividades.Find(x => x.Id == id);
        }

        public override List<Actividad> GetAll()
        {
            return Actividades;
        }

        public override void Create(Actividad entity)
        {
            foreach (var cooperante in entity.Cooperantes)
                cooperante.Encrypt();

            entity.Coordinador.Encrypt();

            base.Create(entity);
            Actividades.Add(entity);
            _EventAggregator.GetEvent<ActividadCreadaEvent>().Publish(entity.Id);
            RaiseActualizarServidor();

            foreach (var cooperante in entity.Cooperantes)
                cooperante.Decrypt();

            entity.Coordinador.Decrypt();
        }

        public override bool Update(Actividad entity)
        {
            foreach (var cooperante in entity.Cooperantes)
                cooperante.Encrypt();

            entity.Coordinador.Encrypt();

            if (base.Update(entity))
            {
                Actividades.Remove(Actividades.Find(x => x.Id == entity.Id));
                Actividades.Add(entity);
                _EventAggregator.GetEvent<ActividadActualizadaEvent>().Publish(entity.Id);
                RaiseActualizarServidor();

                foreach (var cooperante in entity.Cooperantes)
                    cooperante.Decrypt();

                entity.Coordinador.Decrypt();

                return true;
            }

            return false;
        }

        public override void Delete(Actividad entity)
        {
            // WARNING: Debe hacer antes la publicación del evento porque se recoge
            // la Actividad para ver sus citas y Actividades desde otros viewmodels
            _EventAggregator.GetEvent<ActividadEliminadaEvent>().Publish(entity);
            base.Delete(entity);
            Actividades.Remove(Actividades.Find(x => x.Id == entity.Id));
        }

        public override void DeleteAll()
        {
            try
            {
                using (MySqlConnection mysqlConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["GamaCooperacionMySql"].ConnectionString))
                {
                    using (MySqlCommand sqlCommand = new MySqlCommand())
                    {
                        sqlCommand.Connection = mysqlConnection;
                        mysqlConnection.Open();

                        sqlCommand.CommandText = "DELETE FROM seguimientos WHERE 1";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = "DELETE FROM incidenciastarea WHERE 1";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = "DELETE FROM mensajes WHERE 1";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = "DELETE FROM foros WHERE 1";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = "DELETE FROM tareas WHERE 1";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = "DELETE FROM eventos WHERE 1";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = "DELETE FROM cooperanteparticipaenactividad WHERE 1";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = "DELETE FROM actividades WHERE 1";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = "DELETE FROM cooperantes WHERE 1";
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Número de actividades creadas por mes en los últimos meses
        /// </summary>
        /// <param name="numeroDeMeses">Número de meses en total a devolver, incluyendo
        /// el mes actual.</param>
        /// <returns></returns>
        public List<int> GetActividadesNuevasPorMes(int numeroDeMeses)
        {
            List<int> resultado;
            try
            {
                resultado = Session.CreateSQLQuery(@"
                SELECT COUNT(Id)
                FROM `actividades` 
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
