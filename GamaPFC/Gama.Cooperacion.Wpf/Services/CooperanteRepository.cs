using Core.DataAccess;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.Wpf.Eventos;
using Gama.Cooperacion.DataAccess;
using NHibernate;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Util;

namespace Gama.Cooperacion.Wpf.Services
{
    public class CooperanteRepository :
        NHibernateOneSessionRepository<Cooperante, int>, 
        ICooperanteRepository
    {
        private List<Cooperante> _Cooperantes;

        public CooperanteRepository(EventAggregator eventAggregator) : base(eventAggregator)
        {

        }

        public List<Cooperante> Cooperantes
        {
            get
            {
                if (_Cooperantes == null)
                    _Cooperantes = base.GetAll();

                return _Cooperantes;
            }
            set
            {
                _Cooperantes = value;
            }
        }

        private void RaiseActualizarServidor()
        {
            if (CooperacionResources.ClientService != null && CooperacionResources.ClientService.IsConnected())
                CooperacionResources.ClientService.EnviarMensaje($"Cliente {CooperacionResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%COOPERACION");
        }

        public override void UpdateClient()
        {
            CooperacionResources.TodosLosNifDeCooperantes.Clear();
            CooperacionResources.TodosLosNifDeCooperantes.AddRange(_Cooperantes.Select(x => x.Dni));
        }

        public override List<Cooperante> GetAll()
        {
            return Cooperantes;
        }

        public override Cooperante GetById(int id)
        {
            return _Cooperantes.Find(x => x.Id == id);
        }

        public override void Create(Cooperante entity)
        {
            if (entity.Foto != null)
                entity.FotoUpdatedAt = DateTime.Now;

            entity.CreatedAt = DateTime.Now;
            base.Create(entity);
            _Cooperantes.Add(entity);
            CooperacionResources.AddNifCooperante(entity.Dni);
            _EventAggregator.GetEvent<NuevoCooperanteCreadoEvent>().Publish(entity.Id);
            RaiseActualizarServidor();
        }

        public override bool Update(Cooperante entity)
        {
            if (base.Update(entity))
            {
                entity.Decrypt();
                _Cooperantes.Remove(Cooperantes.Find(x => x.Id == entity.Id));
                _Cooperantes.Add(entity);
                _EventAggregator.GetEvent<CooperanteModificadoEvent>().Publish(entity.Id);
                RaiseActualizarServidor();
            }

            return false;
        }

        public List<string> GetNifs()
        {
            return _Cooperantes.Select(X => X.Dni).ToList();
            //List<string> temp;
            //List<string> resultado = new List<string>();

            //try
            //{
            //    temp = Session.QueryOver<Cooperante>()
            //        .Select(x => x.Dni)
            //        .List<string>()
            //        .ToList();

            //    foreach (var nif in temp)
            //    {
            //        resultado.Add(EncryptionService.Decrypt(nif));
            //    }

            //    Session.Clear();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

            //return resultado;
        }
    }
}
