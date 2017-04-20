using Core.DataAccess;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Common.CustomControls;
using Core.Util;
using Prism.Events;
using Gama.Atenciones.Wpf.Eventos;

namespace Gama.Atenciones.Wpf.Services
{
    public class PersonaRepository : NHibernateOneSessionRepository<Persona, int>, IPersonaRepository
    {
        private List<Persona> _Personas;
        private IEventAggregator _EventAggregator;

        public List<Persona> Personas
        {
            get
            {
                if (_Personas != null)
                    return _Personas;

                _Personas = base.GetAll();
                return _Personas;
            }
            set
            {
                _Personas = value;
            }
        }

        public PersonaRepository(IEventAggregator eventAggregator)
        {
            _EventAggregator = eventAggregator;
        }

        public override Persona GetById(int id)
        {
            return Personas.Find(x => x.Id == id);
        }

        public override List<Persona> GetAll()
        {
            return Personas;
        }

        public List<LookupItem> GetAllForLookup()
        {
            return Personas
                .Select(
                    x => new LookupItem
                    {
                        Id = x.Id,
                        DisplayMember1 = x.Nombre,
                        DisplayMember2 = x.Nif,
                        Imagen = x.Imagen
                    }).ToList();

            //var personas = Session.CreateCriteria<Persona>().List<Persona>()
            //    .Select(x => {
            //        x.IsEncrypted = true;
            //        x.DecryptFluent();
            //        return x;
            //    })
            //    .Select(
            //        x => new LookupItem
            //        {
            //            Id = x.Id,
            //            DisplayMember1 = x.Nombre,
            //            DisplayMember2 = x.Nif,
            //            Imagen = x.Imagen
            //        }).ToList();

            //Session.Clear();

            //return personas;
        }

        public override void Create(Persona entity)
        {
            entity.CreatedAt = DateTime.Now;
            base.Create(entity);
            Personas.Add(entity);
            entity.Decrypt();
            AtencionesResources.AddNif(entity.Nif);
            _EventAggregator.GetEvent<PersonaCreadaEvent>().Publish(entity.Id);
        }

        public IEnumerable<int> GetPersonasNuevasPorMes(int numeroDeMeses)
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

        public List<string> GetNifs()
        {
            return Personas.Select(x => x.Nif).ToList();

            //List<string> temp;
            //List<string> resultado = new List<string>();

            //try
            //{
            //    temp = Session.QueryOver<Persona>()
            //        .Select(x => x.Nif)
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

        public List<Atencion> GetAtenciones()
        {
            var resultado = new List<Atencion>();

            resultado = Session.QueryOver<Atencion>().List().ToList();

            Session.Clear();

            return resultado;
        }
    }
}
