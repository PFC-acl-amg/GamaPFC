using Gama.Socios.Business;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.Services
{
    public interface IPeriodoDeAltaRepository
    {
        ISession Session { get; set; }

        PeriodoDeAlta GetById(int id);

        List<PeriodoDeAlta> GetAll();

        void Create(PeriodoDeAlta entity);

        bool Update(PeriodoDeAlta entity);

        void Delete(PeriodoDeAlta entity);

        int CountAll();
    }
}
