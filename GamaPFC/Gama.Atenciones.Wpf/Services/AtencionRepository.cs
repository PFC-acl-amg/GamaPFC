using Core.DataAccess;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Common.CustomControls;
using Core;
using NHibernate.Criterion;
using Prism.Events;
using Gama.Atenciones.Wpf.Eventos;

namespace Gama.Atenciones.Wpf.Services
{
    public class AtencionRepository : NHibernateOneSessionRepository<Atencion, int>, IAtencionRepository
    {
        private List<Atencion> _Atenciones;

        public AtencionRepository(EventAggregator eventAggregator) : base(eventAggregator) { }

        public List<Atencion> Atenciones
        {
            get
            {
                if (_Atenciones == null)
                    _Atenciones = base.GetAll();

                return _Atenciones;
            }
            set
            {
                _Atenciones = value;
            }
        }

        private void RaiseActualizarServidor()
        {
            if (AtencionesResources.ClientService != null && AtencionesResources.ClientService.IsConnected())
                AtencionesResources.ClientService.EnviarMensaje($"Cliente {AtencionesResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%");
        }

        public override void UpdateClient()
        {
            _Atenciones = base.GetAll();
        }

        public override Atencion GetById(int id)
        {
            return Atenciones.Find(x => x.Id == id);
        }

        public override List<Atencion> GetAll()
        {
            return Atenciones;
        }

        public override void Create(Atencion entity)
        {
            base.Create(entity);
            Atenciones.Add(entity);
            _EventAggregator.GetEvent<AtencionCreadaEvent>().Publish(entity.Id);
            RaiseActualizarServidor();
        }

        public override bool Update(Atencion entity)
        {
            if (base.Update(entity))
            {
                entity.Decrypt();
                Atenciones.Remove(Atenciones.Find(x => x.Id == entity.Id));
                Atenciones.Add(entity);
                _EventAggregator.GetEvent<AtencionActualizadaEvent>().Publish(entity.Id);
                RaiseActualizarServidor();
                return true;
            }

            return false;
        }

        public IEnumerable<int> GetAtencionesNuevasPorMes(int numeroDeMeses)
        {
            List<int> resultado;
            try
            {
                resultado = Session.CreateSQLQuery(@"
                SELECT COUNT(Id)
                FROM `atenciones` 
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
