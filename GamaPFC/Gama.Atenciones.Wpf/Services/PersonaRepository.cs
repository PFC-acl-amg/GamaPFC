using Core.DataAccess;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using Gama.Common.CustomControls;
using Prism.Events;
using Gama.Atenciones.Wpf.Eventos;
using NHibernate;
using MySql.Data.MySqlClient;
using Core.Util;
using System.Configuration;
using Gama.Common.Debug;

namespace Gama.Atenciones.Wpf.Services
{
    public class PersonaRepository : NHibernateOneSessionRepository<Persona, int>, IPersonaRepository
    {
        private List<Persona> _Personas;
        private List<Atencion> _Atenciones;
        private List<Cita> _Citas;
        private ICitaRepository _CitaRepository;

        public PersonaRepository(EventAggregator eventAggregator,
            ICitaRepository citaRepository, ISession session) : base(eventAggregator)
        {
            eventAggregator.GetEvent<CitaCreadaEvent>().Subscribe(OnCitaCreadaEvent);
            eventAggregator.GetEvent<CitaActualizadaEvent>().Subscribe(OnCitaActualizadaEvent);
            _CitaRepository = citaRepository;
        }

        public List<Persona> Personas
        {
            get
            {
                if (_Personas == null)
                    _Personas = base.GetAll();

                return _Personas;
            }
            set
            {
                _Personas = value;
            }
        }

        public static List<string> Nifs { get; set; }

        // Se llama al establecerse la propiedad 'Session'
        public override void Initialize()
        {
            // Generará las personas Y los nifs
            Nifs = Personas.Select(x => x.Nif).ToList();
        }

        private void RaiseActualizarServidor() 
        {
            if (AtencionesResources.ClientService != null && AtencionesResources.ClientService.IsConnected())
                AtencionesResources.ClientService.EnviarMensaje($"Cliente {AtencionesResources.ClientId} ha hecho un broadcast @@{Guid.NewGuid()}%%ATENCIONES");
        }

        public override void UpdateClient()
        {
            Nifs.Clear();
            Nifs.AddRange(_Personas.Select(p => p.Nif));
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
            if (entity.Imagen != null)
                entity.ImagenUpdatedAt = DateTime.Now;

            base.Create(entity);
            Personas.Add(entity);
            AddNif(entity.Nif);
            _EventAggregator.GetEvent<PersonaCreadaEvent>().Publish(entity.Id);
            RaiseActualizarServidor();
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
                RaiseActualizarServidor();

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

        public override void DeleteAll()
        {
            try
            {
                using (MySqlConnection mysqlConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["GamaAtencionesMySql"].ConnectionString))
                {
                    using (MySqlCommand sqlCommand = new MySqlCommand())
                    {
                        sqlCommand.Connection = mysqlConnection;
                        mysqlConnection.Open();

                        sqlCommand.CommandText = "DELETE FROM derivaciones WHERE TRUE";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = "DELETE FROM atenciones WHERE TRUE";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = "DELETE FROM citas WHERE TRUE";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = "DELETE FROM personas WHERE TRUE";
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.CommandText = "DELETE FROM asistentes WHERE TRUE";
                        sqlCommand.ExecuteNonQuery();
                    }
                }


            }
            catch (Exception ex)
            {
                throw;
            }
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

    }
}