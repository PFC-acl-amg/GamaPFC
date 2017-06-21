using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gama.Atenciones.Business;
using Gama.Atenciones.Wpf.Services;
using NHibernate;

namespace AdministrationTools.Controllers
{
    public class PersonasController : Controller
    {
        private IPersonaRepository _Personas;

        public PersonasController(IPersonaRepository personaRepository, ISession session)
        {
            _Personas = personaRepository;
            _Personas.Session = session;
        }
        // GET: Personas
        public ActionResult Index()
        {
            var personas = _Personas.GetAll().OrderBy(p => p.Nombre);

            return View(personas);
        }

        // GET: Personas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Persona persona = _Personas.GetById(id.Value);

            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // GET: Personas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Personas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AvatarPath,ComoConocioAGama,DireccionPostal,Email,EstadoCivil,FechaDeNacimiento,Facebook,IdentidadSexual,LinkedIn,Nacionalidad,Nif,NivelAcademico,Nombre,Ocupacion,OrientacionSexual,Telefono,TieneTrabajo,Twitter,Imagen,ViaDeAccesoAGama,CreatedAt,UpdatedAt")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                _Personas.Create(persona);
                return RedirectToAction("Index");
            }

            return View(persona);
        }

        // GET: Personas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = _Personas.GetById(id.Value);
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // POST: Personas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AvatarPath,ComoConocioAGama,DireccionPostal,Email,EstadoCivil,FechaDeNacimiento,Facebook,IdentidadSexual,LinkedIn,Nacionalidad,Nif,NivelAcademico,Nombre,Ocupacion,OrientacionSexual,Telefono,TieneTrabajo,Twitter,Imagen,ViaDeAccesoAGama,CreatedAt,UpdatedAt")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(persona).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(persona);
        }

        // GET: Personas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = _Personas.GetById(id.Value);
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Persona persona = _Personas.GetById(id);
            //db.Personas.Remove(persona);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
