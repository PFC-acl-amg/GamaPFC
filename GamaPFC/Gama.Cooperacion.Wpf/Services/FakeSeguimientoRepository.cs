using Gama.Cooperacion.Business;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class FakeSeguimientoRepository
    {
        public ISessionFactory _session { get; set; }
        private List<Seguimiento> _seguimiento;

        public void Create(Seguimiento entity)
        {
            _seguimiento.Add(entity);
        }
        public void Delete(Seguimiento entity)
        {
        }

        public List<Seguimiento> GetAll()
        {
            if (_seguimiento != null)
                return _seguimiento;

            _seguimiento = new List<Seguimiento>();

            for (int i = 0; i < 5; i++)
            {
                var seguimiento = new Seguimiento()
                {
                    Descripcion = Faker.TextFaker.Sentence(),
                    FechaDePublicacion = Faker.DateTimeFaker.DateTime(),
                };

                _seguimiento.Add(seguimiento);
            }

            return _seguimiento;
        }
        public Seguimiento GetById(int id)
        {
            return _seguimiento.Where(a => a.Id == id).First();
        }

        public bool Update(Seguimiento entity)
        {
            throw new NotImplementedException();
        }

    }
}
