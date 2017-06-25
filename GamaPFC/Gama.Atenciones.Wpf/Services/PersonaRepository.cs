using Core.DataAccess;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using Gama.Common.CustomControls;
using Prism.Events;
using Gama.Atenciones.Wpf.Eventos;
using NHibernate;

namespace Gama.Atenciones.Wpf.Services
{
    public class PersonaRepository : NHibernateOneSessionRepository<Persona, int>, IPersonaRepository
    {
        private List<Persona> _Personas;
        private ICitaRepository _CitaRepository;

        public PersonaRepository(EventAggregator eventAggregator,
            ICitaRepository citaRepository, ISession session) : base(eventAggregator)
        {
            eventAggregator.GetEvent<CitaCreadaEvent>().Subscribe(OnCitaCreadaEvent);
            eventAggregator.GetEvent<CitaActualizadaEvent>().Subscribe(OnCitaActualizadaEvent);
            _CitaRepository = citaRepository;
        }

        private void OnCitaCreadaEvent(int citaId)
        {
            Cita cita = _CitaRepository.GetById(citaId);
            Persona persona = _Personas.Find(x => x.Id == cita.Persona.Id);
            persona.Citas.Add(cita);
        }

        private void OnCitaActualizadaEvent(int citaId)
        {
            Cita cita = _CitaRepository.GetById(citaId);
            List<Cita> citas = _Personas
                .Select(p => p.Citas)
                .First(x => x.Any(c => c.Id == citaId))
                .ToList();

            Cita citaDesactualizada = citas.First(c => c.Id == citaId);
            citaDesactualizada.CopyValuesFrom(cita);
        }

        public List<Persona> Personas
        {
            get
            {
                if (_Personas == null)
                    _Personas = base.GetAll();

                return _Personas;
            }
        }

        public static List<string> Nifs { get; set; }

        // Se llama al establecerse la propiedad 'Session'
        public override void Initialize()
        {
            // Generará las personas Y los nifs
            Nifs = Personas.Select(x => x.Nif).ToList();
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
        }

        public override void Create(Persona entity)
        {
            base.Create(entity);
            Personas.Add(entity);
            AddNif(entity.Nif);
            _EventAggregator.GetEvent<PersonaCreadaEvent>().Publish(entity.Id);
        }

        public override bool Update(Persona entity)
        {
            if (base.Update(entity))
            {
                Personas.Remove(Personas.Find(x => x.Id == entity.Id));
                Personas.Add(entity);
                if (entity._SavedNif != entity.Nif)
                {
                    ReplaceNif(entity._SavedNif, entity.Nif);
                    entity._SavedNif = entity.Nif;
                }
                _EventAggregator.GetEvent<PersonaActualizadaEvent>().Publish(entity.Id);

                return true;
            }

            return false;
        }

        public override void Delete(Persona entity)
        {
            // WARNING: Debe hacer antes la publicación del evento porque se recoge
            // la persona para ver sus citas y atenciones desde otros viewmodels
            _EventAggregator.GetEvent<PersonaEliminadaEvent>().Publish(entity.Id);
            base.Delete(entity);
            Personas.Remove(Personas.Find(x => x.Id == entity.Id));
            Nifs.Remove(entity.Nif);
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

        public List<Atencion> GetAtenciones()
        {
            var resultado = new List<Atencion>();

            resultado = Session.QueryOver<Atencion>().List().ToList();

            Session.Clear();

            return resultado;
        }
    }
}



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