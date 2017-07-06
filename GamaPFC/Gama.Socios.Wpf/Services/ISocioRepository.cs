using Gama.Common.CustomControls;
using Gama.Socios.Business;
using NHibernate;
using System.Collections.Generic;

namespace Gama.Socios.Wpf.Services
{
    public interface ISocioRepository
    {
        ISession Session { get; set; }

        List<Socio> Socios { get; set; }

        Socio GetById(int id);

        List<Socio> GetAll();

        List<LookupItem> GetAllForLookup();

        void Create(Socio entity);

        bool Update(Socio entity);

        void Delete(Socio entity);

        IEnumerable<int> GetSociosNuevosPorMes(int numeroDeMeses);

        int CountAll();
        void UpdateClient();
    }
}