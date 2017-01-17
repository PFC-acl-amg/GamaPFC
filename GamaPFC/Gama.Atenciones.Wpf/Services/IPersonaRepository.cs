using Gama.Atenciones.Business;
using Gama.Common.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace Gama.Atenciones.Wpf.Services
{
    public interface IPersonaRepository
    {
        ISession Session { get; set; }

        Persona GetById(int id);

        List<Persona> GetAll();

        List<LookupItem> GetAllForLookup();

        void Create(Persona entity);

        bool Update(Persona entity);

        void Delete(Persona entity);

        IEnumerable<int> GetPersonasNuevasPorMes(int dashboardUltimasPersonas);

        int CountAll();

        List<string> GetNifs();
    }
}
