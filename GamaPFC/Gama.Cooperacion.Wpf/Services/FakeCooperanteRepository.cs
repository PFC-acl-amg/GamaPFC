using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Cooperacion.Business;
using NHibernate;

namespace Gama.Cooperacion.Wpf.Services
{
    public class FakeCooperanteRepository : ICooperanteRepository
    {
        List<Cooperante> _cooperantes;

        public List<Cooperante> Cooperantes
        {
            get
            {
                return _cooperantes;
            }

            set
            {
                _cooperantes = value;
            }
        }

        public ISession Session { get; set; }

        public FakeCooperanteRepository()
        {
            GetAll();
        }

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

        public List<int> GetCooperantesNuevosPorMes(int numeroDeMeses)
        {
            var resultado = new List<int>(numeroDeMeses);

            for (int i = 0; i < numeroDeMeses; i++)
                resultado.Add(i + 2);

            return resultado;
        }

        public bool Update(Cooperante entity)
        {
            return true;
        }

        public void UpdateClient()
        {
            throw new NotImplementedException();
        }
    }
}
