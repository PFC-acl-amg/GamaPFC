using Gama.Cooperacion.Business;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class FakeIncidenciaRepository
    {
        public ISessionFactory _session { get; set; }
        private List<Incidencia> _incidencia;

        public void Create(Incidencia entity)
        {
            _incidencia.Add(entity);
        }
        public void Delete(Incidencia entity)
        {
        }

        public List<Incidencia> GetAll()
        {
            if (_incidencia != null)
                return _incidencia;

            _incidencia = new List<Incidencia>();

            for (int i = 0; i < 5; i++)
            {
                var incidencia = new Incidencia()
                {
                    Descripcion = Faker.TextFaker.Sentence(),
                    FechaDePublicacion = Faker.DateTimeFaker.DateTime(),
                    Solucionada = 0,
                };

                _incidencia.Add(incidencia);
            }

            return _incidencia;
        }
        public Incidencia GetById(int id)
        {
            return _incidencia.Where(a => a.Id == id).First();
        }

        public bool Update(Seguimiento entity)
        {
            throw new NotImplementedException();
        }

    }
}
