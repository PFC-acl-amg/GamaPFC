using Gama.Atenciones.Business;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Services
{
    public interface IAsistenteRepository
    {
        ISession Session { get; set; }

        Asistente GetById(int id);

        List<Asistente> GetAll();

        void Create(Asistente entity);

        bool Update(Asistente entity);

        void Delete(Asistente entity);

        int CountAll();
    }
}
