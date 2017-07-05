using Core.DataAccess;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Common.CustomControls;
using Core;
using Prism.Events;
using Gama.Atenciones.Wpf.Eventos;

namespace Gama.Atenciones.Wpf.Services
{
    public class CitaRepository : NHibernateOneSessionRepository<Cita, int>, ICitaRepository
    {
        private List<Cita> _Citas;

        public CitaRepository(EventAggregator eventAggregator) : base(eventAggregator) { }

        public List<Cita> Citas
        {
            get
            {
                if (_Citas == null)
                    _Citas = base.GetAll();

                return _Citas;
            }
            set { _Citas = value; }
        }

        private void RaiseActualizarServidor()
        {
            if (AtencionesResources.ClientService != null && AtencionesResources.ClientService.IsConnected())
                AtencionesResources.ClientService.EnviarMensaje($"Cliente {AtencionesResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%ATENCIONES");
        }

        public override void UpdateClient()
        {
            //_Citas = base.GetAll();
        }

        public override Cita GetById(int id)
        {
            return Citas.Find(x => x.Id == id);
        }

        public override List<Cita> GetAll()
        {
            return Citas;
        }

        public override void Create(Cita entity)
        {
            base.Create(entity);
            Citas.Add(entity);
            _EventAggregator.GetEvent<CitaCreadaEvent>().Publish(entity.Id);
            RaiseActualizarServidor();
        }

        public override bool Update(Cita entity)
        {
            if (base.Update(entity))
            {
                //entity.Decrypt();
                Citas.Remove(Citas.Find(x => x.Id == entity.Id));
                Citas.Add(entity);
                _EventAggregator.GetEvent<CitaActualizadaEvent>().Publish(entity.Id);
                RaiseActualizarServidor();
                return true;
            }

            return false;
        }
    }
}
