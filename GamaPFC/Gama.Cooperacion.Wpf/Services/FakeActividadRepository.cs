using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DataAccess;
using Gama.Cooperacion.Business;
using NHibernate;

namespace Gama.Cooperacion.Wpf.Services
{
    public class FakeActividadRepository : IActividadRepository
    {
        public ISessionFactory _session { get; set; }

        public ISession Session
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        private List<Actividad> _actividades;

        public void Create(Actividad entity)
        {
            _actividades.Add(entity);
        }

        public void Delete(Actividad entity)
        {

        }

        public List<Actividad> GetAll()
        {
            if (_actividades != null)
                return _actividades;

            _actividades = new List<Actividad>();

            for (int i = 0; i < 30; i++)
            {
                var actividad = new Actividad()
                {
                    Titulo = Faker.TextFaker.Sentence(),
                    Descripcion = Faker.TextFaker.Sentences(4),
                };

                _actividades.Add(actividad);
            }

            return _actividades;
        }

        public Actividad GetById(int id)
        {
            return _actividades.Where(a => a.Id == id).First();
        }

        public bool Update(Actividad entity)
        {
            throw new NotImplementedException();
        }
    }
}
