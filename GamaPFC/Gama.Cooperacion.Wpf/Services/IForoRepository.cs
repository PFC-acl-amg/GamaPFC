using Gama.Cooperacion.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public interface IForoRepository
    {
        Foro GetById(string id);

        List<Foro> GetAll();

        void Create(Foro entity);

        bool Update(Foro entity);

        void Delete(Foro entity);
    }
}
