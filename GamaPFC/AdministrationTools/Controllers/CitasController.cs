using Gama.Atenciones.Wpf.Services;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdministrationTools.Controllers
{
    public class CitasController : Controller
    {
        private ICitaRepository _Citas;

        public CitasController(ICitaRepository citas, ISession session)
        {
            _Citas = citas;
            _Citas.Session = session;
        }

        // GET: Citas
        public ActionResult Index()
        {
            if (Global.IsLogged)
            {
                var citas = _Citas.GetAll();

                return View(citas);
            }
            else
            {
                return View("~/Views/Account/Login.cshtml");
            }
        }
    }
}