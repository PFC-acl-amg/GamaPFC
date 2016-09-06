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
        public void Create(Actividad entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Actividad entity)
        {
            throw new NotImplementedException();
        }

        public List<Actividad> GetAll()
        {
            var actividades = new List<Actividad>();

            for (int i = 0; i < 30; i++)
            {
                var actividad = new Actividad()
                {
                    Titulo = Faker.NameFaker.FirstName(),
                    Descripcion = Faker.TextFaker.Sentences(4),
                };

                actividades.Add(actividad);
            }

            return actividades;
        }

        public Actividad GetById(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(Actividad entity)
        {
            throw new NotImplementedException();
        }
    }
}
