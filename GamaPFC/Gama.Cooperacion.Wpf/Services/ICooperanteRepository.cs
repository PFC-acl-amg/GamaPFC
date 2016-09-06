using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public interface ICooperanteRepository
    {
        Cooperante GetById(int id);

        List<Cooperante> GetAll();

        //List<LookupItem> GetAllForLookup();

        void Create(Cooperante entity);

        bool Update(Cooperante entity);

        void Delete(Cooperante entity);
    }
}
