using Core.DataAccess;
using Core.Util;
using Gama.Common.CustomControls;
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
    public class SocioRepository : NHibernateOneSessionRepository<Socio, int>, ISocioRepository
    {
        private List<Socio> _Socios;

        public SocioRepository(EventAggregator eventAggregator) : base(eventAggregator) { }

        public List<Socio> Socios
        {
            get
            {
                if (_Socios == null)
                    _Socios = base.GetAll();

                return _Socios;
            }
        }

        public static List<string> Nifs { get; set; }

        // Se llama al establecerse la propiedad 'Session'
        public override void Initialize()
        {
            // Generará las personas Y los nifs
            Nifs = Socios.Select(x => x.Nif).ToList();
        }

        private void RaiseActualizarServidor()
        {
            if (SociosResources.ClientService != null && SociosResources.ClientService.IsConnected())
                SociosResources.ClientService.EnviarMensaje($"Cliente {SociosResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%SOCIOS");
        }

        public override void UpdateClient()
        {
            _Socios = base.GetAll();
            Nifs.Clear();
            Nifs.AddRange(_Socios.Select(p => p.Nif));
        }

        public override Socio GetById(int id)
        {
            return Socios.Find(x => x.Id == id);
        }

        public override List<Socio> GetAll()
        {
            return Socios;
        }

        public List<LookupItem> GetAllForLookup()
        {
            return Socios
                .Select(
                    x => new LookupItem
                    {
                        Id = x.Id,
                        DisplayMember1 = x.Nombre,
                        DisplayMember2 = x.Nif,
                        Imagen = x.Imagen
                    }).ToList();
        }

        public override void Create(Socio entity)
        {
            entity.EstaDadoDeAlta = true;
            base.Create(entity);
            Socios.Add(entity);
            AddNif(entity.Nif);
            _EventAggregator.GetEvent<SocioCreadoEvent>().Publish(entity.Id);
            RaiseActualizarServidor();
        }

        public override bool Update(Socio entity)
        {
            if (base.Update(entity))
            {
                Socios.Remove(Socios.Find(x => x.Id == entity.Id));
                Socios.Add(entity);
                if (entity._SavedNif != entity.Nif)
                {
                    ReplaceNif(entity._SavedNif, entity.Nif);
                    entity._SavedNif = entity.Nif;
                }
                _EventAggregator.GetEvent<SocioActualizadoEvent>().Publish(entity);
                RaiseActualizarServidor();

                return true;
            }

            return false;
        }

        public override void Delete(Socio entity)
        {
            // WARNING: Debe hacer antes la publicación del evento porque se recoge
            // la persona para ver sus citas y atenciones desde otros viewmodels
            _EventAggregator.GetEvent<SocioEliminadoEvent>().Publish(entity.Id);
            base.Delete(entity);
            Socios.Remove(Socios.Find(x => x.Id == entity.Id));
            Nifs.Remove(entity.Nif);
        }

        public IEnumerable<int> GetSociosNuevasPorMes(int numeroDeMeses)
        {
            List<int> resultado;
            try
            {
                resultado = Session.CreateSQLQuery(@"
                SELECT COUNT(Id)
                FROM `personas` 
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

        public void AddNif(string nif)
        {
            if (!Nifs.Contains(nif))
            {
                Nifs.Add(nif);
            }
        }

        public void ReplaceNif(string remove, string add)
        {
            Nifs.Remove(remove);
            Nifs.Add(add);
        }
        public IEnumerable<int> GetSociosNuevosPorMes(int numeroDeMeses)
        {
            List<int> resultado;
            try
            {
                resultado = Session.CreateSQLQuery(@"
                SELECT COUNT(Id)
                FROM `socios` 
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

        public List<string> GetNifs()
        {
            List<string> temp;
            List<string> resultado = new List<string>();

            try
            {
                temp = Session.QueryOver<Socio>()
                    .Select(x => x.Nif)
                    .List<string>()
                    .ToList();

                foreach(var nif in temp)
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
