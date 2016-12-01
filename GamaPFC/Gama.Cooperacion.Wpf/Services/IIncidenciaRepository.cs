using Gama.Cooperacion.Business;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public interface IIncidenciaRepository
    {
        ISession Session { get; set; }
        Incidencia GetById(int id);

        List<Incidencia> GetAll();


        void Create(Incidencia entity);

        bool Update(Incidencia entity);

        void Delete(Incidencia entity);
    }
}
