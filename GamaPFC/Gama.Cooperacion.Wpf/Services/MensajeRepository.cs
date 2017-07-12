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

    public class MensajeRepository : NHibernateOneSessionRepository<Mensaje, int>, IMensajeRepository
    {
        private List<Mensaje> _Mensajes;

        public MensajeRepository(EventAggregator eventAggregator) : base(eventAggregator)
        {

        }

        public List<Mensaje> Mensajes
        {
            get
            {
                if (_Mensajes == null)
                    _Mensajes = base.GetAll();

                return _Mensajes;
            }
            set
            {
                _Mensajes = value;
            }
        }

        private void RaiseActualizarServidor()
        {
            if (CooperacionResources.ClientService != null && CooperacionResources.ClientService.IsConnected())
                CooperacionResources.ClientService.EnviarMensaje($"Cliente {CooperacionResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%COOPERACION");
        }

        public override Mensaje GetById(int id)
        {
            return Mensajes.Find(x => x.Id == id);
        }

        public override List<Mensaje> GetAll()
        {
            return Mensajes;
        }
    }
}
