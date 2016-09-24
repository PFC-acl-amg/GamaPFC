using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Cooperacion.Business;
using NHibernate;

namespace Gama.Cooperacion.Wpf.Services
{
    public class FakeCooperanteRepository 
    {
        List<Cooperante> _cooperantes;

        public void Create(Cooperante entity)
        {

        }

        public void Delete(Cooperante entity)
        {

        }

        public List<Cooperante> GetAll()
        {

            if (_cooperantes != null)
                return _cooperantes;
            else
            {
                _cooperantes = new List<Cooperante>();

                for (int i = 0; i < 50; i++)
                {
                    Cooperante c = new Cooperante
                    {
                        Id = i + 1,
                        Apellido = Faker.NameFaker.LastName(),
                        Dni = Faker.StringFaker.AlphaNumeric(9),
                        Nombre = Faker.NameFaker.FirstName(),
                    };

                    _cooperantes.Add(c);
                }

                return _cooperantes;
            }
        }

        public Cooperante GetById(int id)
        {
            return _cooperantes.Where(c => c.Id == id).First();
        }

        public bool Update(Cooperante entity)
        {
            return true;
        }
    }
}
