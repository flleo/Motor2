using Motor2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Motor2.Controllers
{
    public class FilesController : Controller
    {
        private motorEntities db = new motorEntities(); 
        //
        // GET: /File/
        public ActionResult Index(int? id)
        {
            var fileToRetrieve = db.Files.Find(id);
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }
    }
}