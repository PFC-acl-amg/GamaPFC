using Core.DataAccess;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.DataAccess;
using Gama.Cooperacion.Wpf.Eventos;
using MySql.Data.MySqlClient;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class ActividadRepository : 
        NHibernateOneSessionRepository<Actividad, int>, 
        IActividadRepository
    {
        private List<Actividad> _Actividades;
        public ActividadRepository(EventAggregator eventAggregator) : base(eventAggregator) { }
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
            if (ActividadResources.ClientService != null && ActividadResources.ClientService.IsConnected())
                ActividadResources.ClientService.EnviarMensaje($"Cliente {ActividadResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%COOPERACION_ACTIVIDAD");
        }
        public override void UpdateClient()
        {
        }

        public override List<Actividad> GetAll()
        {
            return Actividades;
        }
        public override Actividad GetById(int id)
        {
            return _Actividades.Find(x => x.Id == id);
        }
        public override void Create(Actividad entity)
        {
            //if (entity.Foto != null)
            //    entity.FotoUpdatedAt = DateTime.Now;

            entity.CreatedAt = DateTime.Now;
            base.Create(entity);
            _Actividades.Add(entity);
            _EventAggregator.GetEvent<NuevaActividadEvent>().Publish(entity.Id);
            RaiseActualizarServidor();
        }
    }
}
