using Core.DataAccess;
using Core.Util;
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

        public List<string> GetNifs()
        {
            List<string> temp;
            List<string> resultado = new List<string>();

            try
            {
                temp = Session.QueryOver<Asistente>()
                    .Select(x => x.Nif)
                    .List<string>()
                    .ToList();

                foreach (var nif in temp)
                {
                    resultado.Add(EncryptionService.Decrypt(nif));
                }

                Session.Clear();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultado;
        }
    }
}
