using Core;
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

        public override Asistente GetById(int id)
        {
            try
            {
                var entity = Session.Get<Asistente>((object)id);

                entity.Decrypt();
                foreach (var cita in entity.Citas)
                {
                    cita.Persona.Decrypt();
                }
                
                Session.Clear();

                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override List<Asistente> GetAll()
        {
            try
            {
                var atenciones = Session.CreateCriteria<Asistente>()
                    .SetFetchMode("Citas", NHibernate.FetchMode.Eager)
                    .List<Asistente>().ToList();

                foreach (var atencion in atenciones)
                {
                    atencion.Decrypt();
                    foreach (var cita in atencion.Citas)
                    {
                        cita.Persona.Decrypt();
                    }
                }

                Session.Clear();

                return atenciones;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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
                    //resultado.Add(EncryptionService.Decrypt(nif));
                    resultado.Add(nif);
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
