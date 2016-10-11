using Gama.Cooperacion.Business;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class FakeEventoRepository
    {
        public ISessionFactory _session { get; set; }
        private List<Evento> _evento;

        public void Create(Evento entity)
        {
            _evento.Add(entity);
        }
        public void Delete(Evento entity)
        {
        }

        public List<Evento> GetAll()
        {
            if (_evento != null)
                return _evento;

            _evento = new List<Evento>();

            for (int i = 0; i < 5; i++)
            {
                var evento = new Evento()
                {
                    Titulo = Faker.TextFaker.Sentence(),
                    FechaDePublicacion = Faker.DateTimeFaker.DateTime(),
                    Ocurrencia = Ocurrencia.Mensaje_Publicado,
                };

                _evento.Add(evento);
            }

            return _evento;
        }
        public Evento GetById(int id)
        {
            return _evento.Where(a => a.Id == id).First();
        }

        public bool Update(Evento entity)
        {
            throw new NotImplementedException();
        }



    }
}
