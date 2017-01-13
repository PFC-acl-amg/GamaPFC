using Core.DataAccess;
using Core.Util;
using Gama.Common.CustomControls;
using Gama.Socios.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Socios.Wpf.Services
{
    public class SocioRepository : NHibernateOneSessionRepository<Socio, int>, ISocioRepository
    {
        public IEnumerable<int> GetSociosNuevosPorMes(int numeroDeMeses)
        {
            List<int> resultado;
            try
            {
                resultado = Session.CreateSQLQuery(@"
                SELECT COUNT(Id)
                FROM `socios` 
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
                temp = Session.QueryOver<Socio>()
                    .Select(x => x.Nif)
                    .List<string>()
                    .ToList();

                foreach(var nif in temp)
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

        public List<LookupItem> GetAllForLookup()
        {
            var socios = Session.CreateCriteria<Socio>().List<Socio>()
                .Select(x => x.DecryptFluent())
                .Select(
                x => new LookupItem
                {
                    Id = x.Id,
                    DisplayMember1 = x.Nombre,
                    DisplayMember2 = x.Nif
                }).ToList();

            //var result = new List<LookupItem>(socios);

            return socios;
        }
        //public override List<Socio> GetAll()
        //{
        //    try
        //    {
        //        //using (var tx = Session.BeginTransaction())
        //        //{
        //        var result = Session.CreateCriteria<Socio>().List<Socio>()
        //            .Select(x => x.DecryptFluent()).ToList();
        //        //tx.Commit();
        //        return result;
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public override void Create(Socio entity)
        //{
        //    try
        //    {
        //        using (var tx = Session.BeginTransaction())
        //        {
        //            entity.Encrypt();
        //            Session.Save(entity);

        //            tx.Commit();
        //            //entity.Decrypt();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public override Socio GetById(int id)
        //{
        //    try
        //    {
        //        //using (var tx = Session.BeginTransaction())
        //        //{
        //        var result = Session.Get<Socio>((object)id);
        //        result.Decrypt();
        //        //tx.Commit();
        //        return result;
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public override bool Update(Socio entity)
        //{
        //    try
        //    {
        //        using (var tx = Session.BeginTransaction())
        //        {
        //            entity.Encrypt();
        //            Session.Update(entity);
        //            //Session.Merge(entity);
        //            tx.Commit();
        //            entity.Decrypt();

        //        }

        //        return true;
        //    }
        //    catch (NHibernate.Exceptions.GenericADOException e)
        //    {
        //        var message = e.Message;
        //        throw e;
        //    }
        //}


    }
}
