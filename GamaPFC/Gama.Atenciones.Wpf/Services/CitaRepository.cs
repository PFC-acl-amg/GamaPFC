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
        private List<Cita> _Atenciones;

        public CitaRepository(IEventAggregator eventAggregator) : base(eventAggregator) { }

        public List<Cita> Citas
        {
            get
            {
                if (_Atenciones == null)
                    _Atenciones = base.GetAll();

                return _Atenciones;
            }
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
        }

        public override bool Update(Cita entity)
        {
            if (base.Update(entity))
            {
                //entity.Decrypt();
                Citas.Remove(Citas.Find(x => x.Id == entity.Id));
                Citas.Add(entity);
                _EventAggregator.GetEvent<CitaActualizadaEvent>().Publish(entity.Id);
                return true;
            }

            return false;
        }
        //public override List<Cita> GetAll()
        //{
        //    try
        //    {
        //        var citas = Session.CreateCriteria<Cita>()
        //            .SetFetchMode("Persona", NHibernate.FetchMode.Eager)
        //            .SetFetchMode("Asistente", NHibernate.FetchMode.Eager)
        //            .List<Cita>().ToList();

        //        foreach (var cita in citas)
        //        {
        //            cita.Decrypt();
        //        }

        //        Session.Clear();

        //        return citas;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public List<LookupItem> GetAllForLookup()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
