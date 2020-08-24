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
using Motor2.Filters;
using System.IO;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;

namespace Motor2.Controllers
{
    public class UsersController : Controller
    {
        private motorEntities db = new motorEntities();


        // GET: Users
        [UserAuthenticationFilter()]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index(int? id)
        {
            ViewData["role"] = "Admin";

            var user = db.Users.Find(id);
            if (user != null)
            {
                ViewBag.User1 = user.user1;
                ViewBag.Id = user.Id;
                if (user.Files.Count > 0) ViewBag.FileId = user.Files.First().Id;
                ViewBag.Href = "Edit";
            }
            else
            {
                ViewBag.User1 = "Login";
                ViewBag.Href = "Login";
            }


            return View(await db.Users.ToListAsync());
        }



        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.User1 = "Login";
            ViewBag.Href = "Login";
            ViewBag.UserId = new SelectList(db.Users, "Id", "user1");
            return View();
        }

        // POST: Users/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,user1,email,password,confirmPassword,nombreFichero,foto")] User user, HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Para resolver más fácil valorDuplicado(e), lo almacenamos sin espacios.
                    user.user1 = user.user1.Trim();
                    user.email = user.email.Trim();
                    user.password = user.password.Trim();
                    //
                    if (upload != null && upload.ContentLength > 0)
                    {
                        Debug.WriteLine(upload.FileName);
                        if (user.nombreFichero == null)
                            user.nombreFichero = upload.FileName;
                        var imagen = new Models.File
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = (int?)FileType.Image,
                            ContentType = upload.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            imagen.Content = reader.ReadBytes(upload.ContentLength);
                            user.foto = imagen.Content;
                        }
                        user.Files = new List<Models.File> { imagen };
                        user = db.Users.Add(user);
                    }
                    else
                    {
                        Debug.WriteLine("No ha upload");
                        user = db.Users.Add(user);
                    }

                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Home", new { id = 0 });
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("No ha sido posible añadir el usuario", " Inténtalo de nuevo, si el problema persisite contacte con el administrador.");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException e)
            {
                ViewBag.ErrorMessage = "Ya existe " + valorDuplicado(e);
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "user1", user.Id);
            return View(user);
        }



        public String valorDuplicado(System.Data.Entity.Infrastructure.DbUpdateException e)
        {
            string error = e.InnerException.ToString();
            int pos = 0;
            while (pos == 0)
            {
                pos = error.IndexOf("(", 0);
            }
            int pos1 = error.IndexOf(")", pos);
            return error.Substring(pos, pos1 - pos + 1);
        }


        // GET: Users/Edit/5
        [UserAuthenticationFilter]
        public async Task<ActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.Include(s => s.Files).SingleOrDefaultAsync(s => s.Id == id);

            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.User1 = user.user1;
                ViewBag.Id = user.Id;
                if (user.Files.Count > 0) ViewBag.FileId = user.Files.First().Id;
                ViewBag.Href = "Edit";
            }
            return View(user);
        }

        [UserAuthenticationFilter]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, HttpPostedFileBase upload)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            if (TryUpdateModel(user, "", new string[] { "user1", "email", "password", "confirmPassword", "nombreFichero" }))
            {
                try
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        if (user.Files.Any(fi => fi.FileType == (int?)FileType.Image))
                        {
                            db.Files.Remove(user.Files.First(fi => fi.FileType == (int?)FileType.Image));
                        }

                        if (user.nombreFichero.Equals(""))
                            user.nombreFichero = upload.FileName;
                        var imagen = new Models.File
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = (int?)FileType.Image,
                            ContentType = upload.ContentType
                        };
                        using (var reader = new BinaryReader(upload.InputStream))
                        {
                            imagen.Content = reader.ReadBytes(upload.ContentLength);
                            user.foto = imagen.Content;
                        }
                        user.Files = new List<Models.File> { imagen };
                    }
                    db.Entry(user).State = EntityState.Modified;
                    Debug.WriteLine(user.ToString());
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { id = user.Id });

                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("No ha sido posible añadir el usuario", " Inténtalo de nuevo, si el problema persisite contacte con el administrador.");
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException e)
                {
                    ViewBag.ErrorMessage = "Ya existe " + valorDuplicado(e);
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException e)
                {
                    Debug.WriteLine(e.EntityValidationErrors + "***");
                }
            }



            ViewBag.UserId = new SelectList(db.Users, "Id", "user1", id);
            return View(user);
        }


        // GET: Users
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([Bind(Include = "email,password")] User user)
        {
            var user2 = await db.Users.FirstOrDefaultAsync(f => f.email == user.email && f.password == user.password);
            try
            {
                if (user2 != null)
                {
                    Session["UserID"] = Guid.NewGuid();
                    return RedirectToAction("Index", "Home", new { id = user2.Id });
                }
                else
                {
                    ModelState.AddModelError("", "No se reconocen las credenciales");
                    return View(user);
                }
            }
            catch (System.InvalidOperationException)
            {
                ModelState.AddModelError("", "Inténtelo de nuevo. Si el problema persiste, consulte a su administrador");
                return View(user);
            }
            catch (IOException) { return View(user); }
        }


        // GET: Users/Details/5
        [UserAuthenticationFilter]
        public async Task<ActionResult> Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.Include(s => s.Files).SingleOrDefaultAsync(s => s.Id == id);
            //Debug.WriteLine(user.Files.First().FileType);
            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.User1 = user.user1;
                ViewBag.Id = user.Id;
                if (user.Files.Count > 0) ViewBag.FileId = user.Files.First().Id;
                ViewBag.Href = "Edit";
            }
            return View(user);
        }

        // GET: Users/Delete/5
        [UserAuthenticationFilter]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.User1 = user.user1;
                ViewBag.Id = user.Id;
                if (user.Files.Count > 0) ViewBag.FileId = user.Files.First().Id;
                ViewBag.Href = "Edit";
            }
            return View(user);
        }


        // POST: Users/Delete/5
        [UserAuthenticationFilter]
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {

            User user = await db.Users.FindAsync(id);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { id });
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
