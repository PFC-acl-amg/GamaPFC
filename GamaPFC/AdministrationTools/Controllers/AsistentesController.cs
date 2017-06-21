using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdministrationTools.Controllers
{
    public class AsistentesController : Controller
    {
        // GET: Asistentes
        public ActionResult Index()
        {
            return View();
        }

        // GET: Asistentes/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Asistentes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Asistentes/Create
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

        // GET: Asistentes/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Asistentes/Edit/5
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

        // GET: Asistentes/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Asistentes/Delete/5
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
