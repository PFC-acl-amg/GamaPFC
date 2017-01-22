using Core.DataAccess;
using Gama.Atenciones.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gama.Common.CustomControls;
using Core.Util;

namespace Gama.Atenciones.Wpf.Services
{
    public class PersonaRepository : NHibernateOneSessionRepository<Persona, int>, IPersonaRepository
    {

        public List<LookupItem> GetAllForLookup()
        {
            var personas = Session.CreateCriteria<Persona>().List<Persona>()
                .Select(x => x.DecryptFluent())
                .Select(
                x => new LookupItem
                {
                    Id = x.Id,
                    DisplayMember1 = x.Nombre,
                    DisplayMember2 = x.Nif
                }).ToList();
            
            return personas;
        }

        public IEnumerable<int> GetPersonasNuevasPorMes(int numeroDeMeses)
        {
            List<int> resultado;
            try
            {
                resultado = Session.CreateSQLQuery(@"
                SELECT COUNT(Id)
                FROM `personas` 
                GROUP BY
                    YEAR(CreatedAt), 
                    MONTH(CreatedAt) 
                ORDER BY 
                    YEAR(CreatedAt) DESC, 
                    MONTH(CreatedAt) DESC")
                        .SetMaxResults(numeroDeMeses)
                        .List<object>()
                        .Select(r => int.Parse(r.ToString()))
                        .ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultado;
        }

        public List<string> GetNifs()
        {
            List<string> temp;
            List<string> resultado = new List<string>();

            try
            {
                temp = Session.QueryOver<Persona>()
                    .Select(x => x.Nif)
                    .List<string>()
                    .ToList();

                foreach (var nif in temp)
                {
                    resultado.Add(EncryptionService.Decrypt(nif));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultado;
        }

        public List<Atencion> GetAtenciones()
        {
            var resultado = new List<Atencion>();

            resultado = Session.QueryOver<Atencion>().List().ToList();

            return resultado;
        }
    }
}
