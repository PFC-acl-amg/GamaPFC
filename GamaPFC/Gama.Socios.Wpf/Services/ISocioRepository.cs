using Gama.Socios.Business;
using NHibernate;
using System.Collections.Generic;

namespace Gama.Socios.Wpf.Services
{
    public interface ISocioRepository
    {
        ISession Session { get; set; }

        Socio GetById(int id);

        List<Socio> GetAll();
        
        void Create(Socio entity);

        bool Update(Socio entity);

        void Delete(Socio entity);

        IEnumerable<int> GetSociosNuevosPorMes(int numeroDeMeses);

        int CountAll();
    }
}