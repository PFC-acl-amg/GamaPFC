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
        private List<Business.Tarea> _tarea;

        public void Create(Business.Tarea entity)
        {
            _tarea.Add(entity);
        }
        public void Delete(Business.Tarea entity)
        {
        }

        public List<Business.Tarea> GetAll()
        {
            if (_tarea != null)
                return _tarea;

            _tarea = new List<Business.Tarea>();

            for (int i = 0; i < 5; i++)
            {
                var tarea = new Business.Tarea()
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
