using Motor2.Models;
using System.Linq;
using System.Web.Mvc;

namespace Motor2.Controllers
{
    public class HomeController : Controller
    {
        private motorEntities db = new motorEntities();

        public ActionResult Index(int? id)
        {

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
                ViewBag.Href = "Login";
                ViewBag.User1 = "Login";
            }
            return View();
        }



        public ActionResult About(int? id)
        {
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
                ViewBag.Href = "Login";
                ViewBag.User1 = "Login";
            }
            return View();
        }

        public ActionResult Contact(int? id)
        {
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
                ViewBag.Href = "Login";
                ViewBag.User1 = "Login";
            }
            return View();
        }
    }
}