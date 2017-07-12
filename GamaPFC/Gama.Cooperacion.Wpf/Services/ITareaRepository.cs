using Gama.Cooperacion.Business;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public interface ITareaRepository
    {
        ISession Session { get; set; }
        Tarea GetById(int id);
        List<Tarea> Tareas { get; set; }

        List<Tarea> GetAll();

        //List<LookupItem> GetAllForLookup();

        void Create(Tarea entity);

        bool Update(Tarea entity);

        void Delete(Tarea entity);
    }

}
