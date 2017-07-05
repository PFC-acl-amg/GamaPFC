using Core;
using Core.DataAccess;
using Core.Util;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Eventos;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Services
{
    public class AsistenteRepository :
        NHibernateOneSessionRepository<Asistente, int>,
        IAsistenteRepository
    {
        private List<Asistente> _Asistentes;

        public AsistenteRepository(EventAggregator eventAggregator) : base(eventAggregator) { }

        public List<Asistente> Asistentes
        {
            get
            {
                if (_Asistentes == null)
                    _Asistentes = base.GetAll();

                return _Asistentes;
            }
            set
            {
                _Asistentes = value;
            }
        }

        private void RaiseActualizarServidor()
        {
            if (AtencionesResources.ClientService != null && AtencionesResources.ClientService.IsConnected())
                AtencionesResources.ClientService.EnviarMensaje($"Cliente {AtencionesResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%");
        }

        public override void UpdateClient()
        {
            //_Asistentes = base.GetAll();

            AtencionesResources.TodosLosNifDeAsistentes.Clear();
            AtencionesResources.TodosLosNifDeAsistentes.AddRange(_Asistentes.Select(x => x.Nif));
        }

        public override Asistente GetById(int id)
        {
            return _Asistentes.Find(x => x.Id == id);
        }

        public override List<Asistente> GetAll()
        {
            return Asistentes;
        }

        public override void Create(Asistente entity)
        {
            entity.CreatedAt = DateTime.Now;
            base.Create(entity);
            _Asistentes.Add(entity);
            AtencionesResources.AddNifAAsistente(entity.Nif);
            _EventAggregator.GetEvent<AsistenteCreadoEvent>().Publish(entity.Id);
            RaiseActualizarServidor();
        }

        public override bool Update(Asistente entity)
        {
            if (base.Update(entity))
            {
                entity.Decrypt();
                _Asistentes.Remove(Asistentes.Find(x => x.Id == entity.Id));
                _Asistentes.Add(entity);
                _EventAggregator.GetEvent<AsistenteActualizadoEvent>().Publish(entity.Id);
                RaiseActualizarServidor();
            }

            return false;
        }

        public List<string> GetNifs()
        {
            List<string> temp;
            List<string> resultado = new List<string>();

            try
            {
                temp = Session.QueryOver<Asistente>()
                    .Select(x => x.Nif)
                    .List<string>()
                    .ToList();

                foreach (var nif in temp)
                {
                    resultado.Add(EncryptionService.Decrypt(nif));
                }

                Session.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultado;
        }
    }
}
