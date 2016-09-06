using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Cooperacion.Business;

namespace Gama.Cooperacion.Wpf.Services
{
    public class FakeCooperanteRepository : ICooperanteRepository
    {
        public void Create(Cooperante entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Cooperante entity)
        {
            throw new NotImplementedException();
        }

        public List<Cooperante> GetAll()
        {
            var cooperantes = new List<Cooperante>();

            for (int i = 0; i < 10; i++)
            {
                Cooperante c = new Cooperante
                {
                    Apellido = Faker.NameFaker.LastName(),
                    Dni = Faker.StringFaker.AlphaNumeric(9),
                    Nombre = Faker.NameFaker.FirstName(),
                };

                cooperantes.Add(c);
            }

            return cooperantes;
        }

        public Cooperante GetById(int id)
        {
            throw new NotImplementedException();
        }

        public bool Update(Cooperante entity)
        {
            throw new NotImplementedException();
        }
    }
}
