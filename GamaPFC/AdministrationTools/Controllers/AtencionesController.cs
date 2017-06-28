using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdministrationTools.Controllers
{
    public class AtencionesController : Controller
    {
        // GET: Atenciones
        public ActionResult Index()
        {
            return View();
        }

        // GET: Atenciones/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Atenciones/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Atenciones/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Atenciones/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Atenciones/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Atenciones/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Atenciones/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
