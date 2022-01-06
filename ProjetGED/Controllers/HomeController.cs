using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetGED.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            
            return View();
        }
        [Authorize(Roles = "admin")]
        public ActionResult OnlyAdmin()
        {
            return Json(new { data="hi admin"},JsonRequestBehavior.AllowGet);
        }
    }
}