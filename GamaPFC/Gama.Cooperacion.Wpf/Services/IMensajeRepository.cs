using Gama.Cooperacion.Business;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public interface IMensajeRepository
    {
        ISession Session { get; set; }
        Mensaje GetById(int id);
        List<Mensaje> Mensajes { get; set; }

        List<Mensaje> GetAll();

        //List<LookupItem> GetAllForLookup();

        void Create(Mensaje entity);

        bool Update(Mensaje entity);

        void Delete(Mensaje entity);
    }
}
