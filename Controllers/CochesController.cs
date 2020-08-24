using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Motor2.Models;

namespace Motor2.Controllers
{
    public class CochesController : Controller
    {
        private motorEntities db = new motorEntities();

        // GET: Coches
        public async Task<ActionResult> Index(int? id)
        {
            //Para mostrar el usuario en la cabecera
            var user = db.Users.Find(id);
            if (user != null)
            {
                ViewBag.User1 = user.user1;
                ViewBag.Id = id;
                if (user.Files.Count > 0) ViewBag.FileId = user.Files.First().Id;
                ViewBag.Href = "Edit";
            }
            else
            {
                ViewBag.User1 = "Login";
                ViewBag.Href = "Login";
            }
            //

            var coches = db.Coches.Include(c => c.User);
            return View(await coches.ToListAsync());
        }

        // GET: Coches/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coch coch = await db.Coches.FindAsync(id);
            if (coch == null)
            {
                return HttpNotFound();
            }
            return View(coch);
        }

        // GET: Coches/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "user1");
            return View();
        }

        // POST: Coches/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,descripcion,fecha,nombreFichero,foto,UserId")] Coch coch)
        {
            if (ModelState.IsValid)
            {
                db.Coches.Add(coch);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "user1", coch.UserId);
            return View(coch);
        }

        // GET: Coches/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coch coch = await db.Coches.FindAsync(id);
            if (coch == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "user1", coch.UserId);
            return View(coch);
        }

        // POST: Coches/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,descripcion,fecha,nombreFichero,foto,UserId")] Coch coch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(coch).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "user1", coch.UserId);
            return View(coch);
        }

        // GET: Coches/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coch coch = await db.Coches.FindAsync(id);
            if (coch == null)
            {
                return HttpNotFound();
            }
            return View(coch);
        }

        // POST: Coches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Coch coch = await db.Coches.FindAsync(id);
            db.Coches.Remove(coch);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
