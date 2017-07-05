using Gama.Socios.Business;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.Services
{
    public interface ICuotaRepository
    {
        ISession Session { get; set; }
        List<Cuota> Cuotas { get; set; }

        Cuota GetById(int id);

        List<Cuota> GetAll();

        void Create(Cuota entity);

        bool Update(Cuota entity);

        void Delete(Cuota entity);

        int CountAll();
    }
}
