using Core.DataAccess;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Atenciones.Wpf.Services
{
    public class AsistenteRepository :
        NHibernateOneSessionRepository<Asistente, int>,
        IAsistenteRepository
    {
    }
}
