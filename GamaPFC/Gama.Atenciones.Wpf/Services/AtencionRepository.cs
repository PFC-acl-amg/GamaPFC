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

        public AtencionRepository(IEventAggregator eventAggregator) : base(eventAggregator) { }

        public List<Atencion> Atenciones
        {
            get
            {
                if (_Atenciones == null)
                    _Atenciones = base.GetAll();

                return _Atenciones;
            }
        }

        public override Atencion GetById(int id)
        {
            return Atenciones.Find(x => x.Id == id);
        }

        public override List<Atencion> GetAll()
        {
            return Atenciones;
            //try
            //{
            //    var atenciones = Session.CreateCriteria<Atencion>()
            //        .SetFetchMode("Cita", NHibernate.FetchMode.Eager).List<Atencion>().ToList();

            //    Session.Clear();

            //    return atenciones;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        public override void Create(Atencion entity)
        {
            base.Create(entity);
            Atenciones.Add(entity);
            _EventAggregator.GetEvent<AtencionCreadaEvent>().Publish(entity.Id);
        }

        public override bool Update(Atencion entity)
        {
            if (base.Update(entity))
            {
                entity.Decrypt();
                Atenciones.Remove(Atenciones.Find(x => x.Id == entity.Id));
                Atenciones.Add(entity);
                _EventAggregator.GetEvent<AtencionActualizadaEvent>().Publish(entity.Id);
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

        //public override Atencion GetById(int id)
        //{
        //    try
        //    {
        //        var entity = Session.Get<Atencion>((object)id);

        //        var encryptableEntity = entity as IEncryptable;
        //        if (encryptableEntity != null)
        //        {
        //            //encryptableEntity.IsEncrypted = true;
        //            //encryptableEntity.Decrypt();
        //            if (encryptableEntity.IsEncrypted)
        //            {
        //                encryptableEntity.Decrypt();
        //            }
        //        }

        //        //Session.Clear();

        //        return entity;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
