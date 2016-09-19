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

        //public override Actividad GetById(int id)
        //{
        //    Actividad a = base.GetById(id);
        //    a.Coordinador = Session.CreateCriteri
        //}
    }
}
