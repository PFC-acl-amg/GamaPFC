using Gama.Cooperacion.Business;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class FakeTareaRepository
    {
        public ISessionFactory _session { get; set; }
        private List<Tarea> _tarea;

        public void Create(Tarea entity)
        {
            _tarea.Add(entity);
        }
        public void Delete(Tarea entity)
        {
        }

        public List<Tarea> GetAll()
        {
            if (_tarea != null)
                return _tarea;

            _tarea = new List<Tarea>();

            for (int i = 0; i < 5; i++)
            {
                var tarea = new Tarea()
                {
                    Descripcion = Faker.TextFaker.Sentence(),
                    FechaDeFinalizacion = Faker.DateTimeFaker.DateTime(),
                    Responsable = null,
                    HaFinalizado = Faker.BooleanFaker.Boolean(),
                };

                _tarea.Add(tarea);
            }

            return _tarea;
        }
        public Tarea GetById(int id)
        {
            return _tarea.Where(a => a.Id == id).First();
        }

        public bool Update(Evento entity)
        {
            throw new NotImplementedException();
        }



    }
}
