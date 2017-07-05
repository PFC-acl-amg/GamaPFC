﻿using Core.DataAccess;
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

        public PeriodoDeAltaRepository(EventAggregator eventAggregator) : base(eventAggregator) { }

        public List<PeriodoDeAlta> PeriodoDeAltas
        {
            get
            {
                if (_PeriodoDeAltas == null)
                    _PeriodoDeAltas = base.GetAll();

                return _PeriodoDeAltas;
            }
            set { _PeriodoDeAltas = value; }
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
            return PeriodoDeAltas.Find(x => x.Id == id);
        }

        public override List<PeriodoDeAlta> GetAll()
        {
            return PeriodoDeAltas;
        }

        public override void Create(PeriodoDeAlta entity)
        {
            base.Create(entity);
            PeriodoDeAltas.Add(entity);
            _EventAggregator.GetEvent<PeriodoDeAltaCreadoEvent>().Publish(entity.Id);
            RaiseActualizarServidor();
        }

        public override bool Update(PeriodoDeAlta entity)
        {
            if (base.Update(entity))
            {
                //entity.Decrypt();
                PeriodoDeAltas.Remove(PeriodoDeAltas.Find(x => x.Id == entity.Id));
                PeriodoDeAltas.Add(entity);
                _EventAggregator.GetEvent<PeriodoDeAltaActualizadoEvent>().Publish(entity.Id);
                RaiseActualizarServidor();
                return true;
            }

            return false;
        }
    }
}