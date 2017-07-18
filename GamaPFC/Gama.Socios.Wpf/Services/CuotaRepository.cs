using Core.DataAccess;
using Gama.Socios.Business;
using Gama.Socios.Wpf.Eventos;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.Services
{
    public class CuotaRepository : NHibernateOneSessionRepository<Cuota, int>, ICuotaRepository
    {
        public List<Cuota> _Cuotas;

        public CuotaRepository(EventAggregator eventAggregator) : base(eventAggregator)
        {
            eventAggregator.GetEvent<PeriodoDeAltaActualizadoEvent>().Subscribe(OnActualizarCuotas);
        }

        public List<Cuota> Cuotas
        {
            get
            {
                if (_Cuotas == null)
                    _Cuotas = base.GetAll();

                return _Cuotas;
            }
            set { _Cuotas = value; }
        }
        private void OnActualizarCuotas(int id)
        {
            _Cuotas = base.GetAll();
        }
        private void RaiseActualizarServidor()
        {
            if (SociosResources.ClientService != null && SociosResources.ClientService.IsConnected())
                SociosResources.ClientService.EnviarMensaje($"Cliente {SociosResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%SOCIOS");
        }

        public override void UpdateClient()
        {
            //_Citas = base.GetAll();
        }

        public override Cuota GetById(int id)
        {
            return Cuotas.Find(x => x.Id == id);
        }

        public override List<Cuota> GetAll()
        {
            return Cuotas;
        }

        public override void Create(Cuota entity)
        {
            base.Create(entity);
            Cuotas.Add(entity);
            _EventAggregator.GetEvent<CuotaCreadaEvent>().Publish(entity.Id);
            RaiseActualizarServidor();
        }

        public override bool Update(Cuota entity)
        {
            if (base.Update(entity))
            {
                //entity.Decrypt();
                Cuotas.Remove(Cuotas.Find(x => x.Id == entity.Id));
                Cuotas.Add(entity);
                _EventAggregator.GetEvent<CuotaActualizadaEvent>().Publish(entity.Id);
                RaiseActualizarServidor();
                return true;
            }

            return false;
        }
    }
}
