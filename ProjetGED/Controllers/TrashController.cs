using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetGED.Controllers
{
    public class TrashController : Controller
    {
        // GET: Trash
        public ActionResult Index()
        {
            return View();
        }
    }
}