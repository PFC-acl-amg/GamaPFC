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
        }

        private void RaiseActualizarServidor()
        {
            AtencionesResources.ClientService.EnviarMensaje($"Cliente {AtencionesResources.ClientId} ha hecho un broadcast");
        }

        public override void UpdateClient()
        {
            _Asistentes = base.GetAll();

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
        //List<Asistente> _Asistentes;

        //public AsistenteRepository(IEventAggregator eventAggregator) : base(eventAggregator) { }

        //public override Asistente GetById(int id)
        //{
        //    try
        //    {
        //        var entity = Session.Get<Asistente>((object)id);

        //        entity.Decrypt();
        //        foreach (var cita in entity.Citas)
        //        {
        //            cita.Persona.Decrypt();
        //        }

        //        Session.Clear();

        //        return entity;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public override List<Asistente> GetAll()
        //{
        //    if (_Asistentes != null)
        //        return _Asistentes;

        //    try
        //    {
        //        var asistentes = Session.CreateCriteria<Asistente>()
        //            .SetFetchMode("Citas", NHibernate.FetchMode.Eager)
        //            .List<Asistente>().ToList();

        //        foreach (var asistente in asistentes)
        //        {
        //            asistente.Decrypt();
        //            foreach (var cita in asistente.Citas)
        //            {
        //                cita.Persona.Decrypt();
        //            }
        //        }

        //        Session.Clear();

        //        _Asistentes = asistentes;
        //        return _Asistentes;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

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
                    //resultado.Add(nif);
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
