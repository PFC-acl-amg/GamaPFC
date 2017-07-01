using Gama.Atenciones.Business;
using Gama.Common.CustomControls;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Services
{
    public interface IAtencionRepository
    {
        ISession Session { get; set; }
        List<Atencion> Atenciones { get; set; }
        Atencion GetById(int id);
        List<Atencion> GetAll();
        void Create(Atencion entity);
        bool Update(Atencion entity);
        void Delete(Atencion entity);
        IEnumerable<int> GetAtencionesNuevasPorMes(int numeroDeMeses);
        int CountAll();
        void DeleteAll();
    }
}
