using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetGED.Controllers
{
    public class FolderController : Controller
    {
        GEDContext ctx = new GEDContext();
        // GET: Folder
        [Route("Folder/Index/{*slug}")]
        public ActionResult Index(string slug)
        {
            ViewData["foldersUrl"] = slug;
            return View();
        }
    }
}