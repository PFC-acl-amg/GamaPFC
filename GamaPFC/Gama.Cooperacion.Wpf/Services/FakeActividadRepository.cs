using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DataAccess;
using Gama.Cooperacion.Business;
using NHibernate;

namespace Gama.Cooperacion.Wpf.Services
{
    public class FakeActividadRepository : IActividadRepository
    {
        private List<Actividad> _actividades;

        public ISession Session { get; set; }

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

            int createdAt = 0;
            for (int i = 0; i < 50; i++)
            {
                var actividad = new Actividad()
                {
                    Id = i + 1,
                    Titulo = Faker.TextFaker.Sentence(),
                    Descripcion = Faker.TextFaker.Sentences(4),
                    CreatedAt = DateTime.Now.AddMonths(createdAt),
                    Coordinador = null,
                    Cooperantes = null,
                };

                _actividades.Add(actividad);

                if (i % 5 == 0)
                {
                    createdAt--;
                }
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

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public List<int> GetActividadesNuevasPorMes(int numeroDeMeses)
        {
            var resultado = new List<int>(numeroDeMeses);

            for (int i = 0; i < numeroDeMeses; i++)
                resultado.Add(i + 2);

            return resultado;
        }
    }
}
