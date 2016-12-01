using Gama.Cooperacion.Business;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class FakeMensajeRepository
    {
        public ISessionFactory _session { get; set; }
        private List<Mensaje> _mensaje;

        public void Create(Mensaje entity)
        {
            _mensaje.Add(entity);
        }
        public void Delete(Mensaje entity)
        {
        }

        public List<Mensaje> GetAll()
        {
            if (_mensaje != null)
                return _mensaje;

            _mensaje = new List<Mensaje>();

            for (int i = 0; i < 2; i++)
            {
                var mensaje = new Mensaje()
                {
                    Titulo = Faker.TextFaker.Sentence(),
                    FechaDePublicacion = Faker.DateTimeFaker.DateTime(),
                    
                };

                _mensaje.Add(mensaje);
            }

            return _mensaje;
        }
        public Mensaje GetById(int id)
        {
            return _mensaje.Where(a => a.Id == id).First();
        }

        public bool Update(Evento entity)
        {
            throw new NotImplementedException();
        }



    }
}
