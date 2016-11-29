using Gama.Cooperacion.Business;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class FakeForoRepository : IForoRepository
    {
        public ISessionFactory _session { get; set; }
        private List<Foro> _foro;

        public void Create(Foro entity)
        {
            _foro.Add(entity);
        }
        public void Delete(Foro entity)
        {
        }

        public List<Foro> GetAll()
        {
            if (_foro != null)
                return _foro;

            _foro = new List<Foro>();

            for (int i = 0; i < 2; i++)
            {
                var foro = new Foro()
                {
                    Titulo = Faker.TextFaker.Sentence(),
                    FechaDePublicacion = Faker.DateTimeFaker.DateTime(),
                };

                _foro.Add(foro);
            }

            return _foro;
        }
        public Foro GetById(string titulo)
        {
            return _foro.Where(a => a.Titulo == titulo).First();
        }

        public bool Update(Foro entity)
        {
            throw new NotImplementedException();
        }



    }
}
