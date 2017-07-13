using Gama.Socios.Wpf.Services;
using Microsoft.Practices.Unity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdministrationTools.Controllers
{
    public class SociosController : Controller
    {
        private ISocioRepository _Socios;

        public SociosController(ISocioRepository socios, Gama.Socios.DataAccess.NHibernateSessionFactoryWeb sessionFactory)
        {
            _Socios = socios;
            _Socios.Session = sessionFactory.OpenSession();
        }

        // GET: Citas
        public ActionResult Index()
        {
            var socios = _Socios.GetAll();

            return View(socios);
        }
    }
}