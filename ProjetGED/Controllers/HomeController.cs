using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ProjetGED.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            // extraire les path des Folders de la BD
            // extraire les path des document qui s touve dans le root folder , le nom du folder root est le id du user

            using(var context = new GEDContext())
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var id = Convert.ToInt32(claimsIdentity.FindFirst("UserId").Value);
                var user = context.OurUsers.Include("Folders").First(u => u.Id == id);
                var rootFolder = user.Folders.First(f => f.Name == user.Id.ToString());
                context.Entry(rootFolder).Collection(r => r.Documents).Load();
                ViewData["folders"] = user.Folders.ToList().FindAll(f => f.Parent == rootFolder);
                ViewData["files"] = rootFolder.Documents;
            }
            return View();
        }
        [Authorize(Roles = "admin")]
        public ActionResult OnlyAdmin()
        {
            return Json(new { data="hi admin"},JsonRequestBehavior.AllowGet);
        }
    }
}