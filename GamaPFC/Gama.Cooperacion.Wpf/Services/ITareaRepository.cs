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
        List<Business.Tarea> Tareas { get; set; }

        List<Business.Tarea> GetAll();

        //List<LookupItem> GetAllForLookup();

        void Create(Business.Tarea entity);

        bool Update(Business.Tarea entity);

        void Delete(Business.Tarea entity);
    }

}
