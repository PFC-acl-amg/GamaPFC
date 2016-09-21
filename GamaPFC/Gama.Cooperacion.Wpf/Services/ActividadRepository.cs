using Core.DataAccess;
using Gama.Cooperacion.Business;
using Gama.Cooperacion.DataAccess;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gama.Cooperacion.Wpf.Services
{
    public class ActividadRepository : NHibernateOneSessionRepository<Actividad, int>, IActividadRepository
    {
        public ActividadRepository()
        {
        }

        //public override bool Update(Actividad entity)
        //{
        //    string query = "UPDATE actividades a SET a.Titulo = :titulo, "
        //        + "a.Descripcion = :descripcion, "
        //        + "a.Coordinador_Id = :coordinador_id " 
        //        + "WHERE a.Id = :id";
        //    try
        //    {
        //        using (var tx = Session.BeginTransaction())
        //        {
        //            //Session.Update(entity);
        //            Session.CreateSQLQuery(query)
        //                .SetString("titulo", entity.Titulo)
        //                .SetString("descripcion", entity.Descripcion)
        //                .SetInt32("coordinador_id", entity.Coordinador.Id)
        //                .SetInt32("id", entity.Id)
        //                .ExecuteUpdate();
        //            tx.Commit();
        //        }

        //        return true;
        //    }
        //    catch (NHibernate.Exceptions.GenericADOException e)
        //    {
        //        var message = e.Message;
        //        return false;
        //    }
        //}

        //public override Actividad GetById(int id)
        //{
        //    Actividad a = base.GetById(id);
        //    a.Coordinador = Session.CreateCriteri
        //}
    }
}
