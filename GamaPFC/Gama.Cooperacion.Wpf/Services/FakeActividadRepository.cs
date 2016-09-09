using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Cooperacion.Business;

namespace Gama.Cooperacion.Wpf.Services
{
    public class FakeActividadRepository : IActividadRepository
    {
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
                    Titulo = Faker.NameFaker.FirstName(),
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
