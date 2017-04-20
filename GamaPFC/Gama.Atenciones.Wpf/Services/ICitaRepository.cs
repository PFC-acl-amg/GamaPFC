using Gama.Atenciones.Business;
using Gama.Common.CustomControls;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Services
{
    public interface ICitaRepository
    {
        ISession Session { get; set; }

        Cita GetById(int id);

        List<Cita> GetAll();

        List<LookupItem> GetAllForLookup();

        void Create(Cita entity);

        bool Update(Cita entity);

        void Delete(Cita entity);
        int CountAll();
        void DeleteAll();
    }
}
