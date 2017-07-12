using Gama.Cooperacion.Business;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public interface ISeguimientoRepository
    {
        ISession Session { get; set; }
        Seguimiento GetById(int id);
        List<Seguimiento> Seguimientos { get; set; }

        List<Seguimiento> GetAll();


        void Create(Seguimiento entity);

        bool Update(Seguimiento entity);

        void Delete(Seguimiento entity);
    }
}
