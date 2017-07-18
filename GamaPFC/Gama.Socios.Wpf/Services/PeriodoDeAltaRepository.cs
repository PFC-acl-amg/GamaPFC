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
    public class PeriodoDeAltaRepository : NHibernateOneSessionRepository<PeriodoDeAlta, int>, IPeriodoDeAltaRepository
    {
        public List<PeriodoDeAlta> _PeriodoDeAltas;

        public PeriodoDeAltaRepository(EventAggregator eventAggregator) : base(eventAggregator)
        {
            eventAggregator.GetEvent<PeriodoDeAltaActualizadoEvent>().Subscribe(OnActualizarPeriodosDeAlta);
        }

        public List<PeriodoDeAlta> PeriodosDeAlta
        {
            get
            {
                if (_PeriodoDeAltas == null)
                    _PeriodoDeAltas = base.GetAll();

                return _PeriodoDeAltas;
            }
            set { _PeriodoDeAltas = value; }
        }
        private void OnActualizarPeriodosDeAlta(int id)
        {
            _PeriodoDeAltas = base.GetAll();
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

        public override PeriodoDeAlta GetById(int id)
        {
            return PeriodosDeAlta.Find(x => x.Id == id);
        }

        public override List<PeriodoDeAlta> GetAll()
        {
            return PeriodosDeAlta;
        }

        public override void Create(PeriodoDeAlta entity)
        {
            base.Create(entity);
            PeriodosDeAlta.Add(entity);
            _EventAggregator.GetEvent<PeriodoDeAltaCreadoEvent>().Publish(entity.Id);
            RaiseActualizarServidor();
        }

        public override bool Update(PeriodoDeAlta entity)
        {
            if (base.Update(entity))
            {
                //entity.Decrypt();
                PeriodosDeAlta.Remove(PeriodosDeAlta.Find(x => x.Id == entity.Id));
                PeriodosDeAlta.Add(entity);
                _EventAggregator.GetEvent<PeriodoDeAltaActualizadoEvent>().Publish(entity.Id);
                RaiseActualizarServidor();
                return true;
            }

            return false;
        }
    }
}
