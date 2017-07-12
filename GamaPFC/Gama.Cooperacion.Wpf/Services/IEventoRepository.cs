using Gama.Cooperacion.Business;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public interface IEventoRepository
    {
        ISession Session { get; set; }
        Evento GetById(int id);
        List<Evento> Eventos { get; set; }

        List<Evento> GetAll();

        //List<LookupItem> GetAllForLookup();

        void Create(Evento entity);

        bool Update(Evento entity);

        void Delete(Evento entity);
    }
}
