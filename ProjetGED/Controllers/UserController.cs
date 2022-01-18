using ProjetGED.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetGED.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Search(string userName)
        {
            List<User> users = new List<User>();
            if (!string.IsNullOrWhiteSpace(userName))
            {
                using (var context = new GEDContext())
                {
                    users= context.OurUsers.ToList().FindAll(u => u.Email.ToLower().Contains(userName.ToLower()));
                }
            }
            var results = users.Select(u => new { id = u.Id, text = u.Email });
            return Json(new { results},JsonRequestBehavior.AllowGet);
        }

    }
}