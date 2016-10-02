using Core.DataAccess;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Common.CustomControls;

namespace Gama.Atenciones.Wpf.Services
{
    public class AtencionRepository : NHibernateOneSessionRepository<Atencion, int>, IAtencionRepository
    {
        public List<LookupItem> GetAllForLookup()
        {
            throw new NotImplementedException();
        }
    }
}
