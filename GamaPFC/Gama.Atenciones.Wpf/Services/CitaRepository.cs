using Core.DataAccess;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Common.CustomControls;
using Core;

namespace Gama.Atenciones.Wpf.Services
{
    public class CitaRepository : NHibernateOneSessionRepository<Cita, int>, ICitaRepository
    {
        public override List<Cita> GetAll()
        {
            try
            {
                var citas = Session.CreateCriteria<Cita>()
                    .SetFetchMode("Persona", NHibernate.FetchMode.Eager).List<Cita>().ToList();

                foreach (var cita in citas)
                {
                    cita.Persona.Decrypt();
                }

                Session.Clear();

                return citas;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LookupItem> GetAllForLookup()
        {
            throw new NotImplementedException();
        }
    }
}
