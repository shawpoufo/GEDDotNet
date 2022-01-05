using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetGED.Controllers
{
    public class WelcomController : Controller
    {
        GEDContext ctx = new GEDContext();

        // GET: Welcom
        public ActionResult Index()
        {
            
                ctx.Users.Add(new Models.User { Email="y",Name="mitah",Password="12315" });
                ctx.SaveChanges();
                var x = ctx.Users.ToList().Last();
            //ctx.Entry(x).State = System.Data.Entity.EntityState.Modified;
            return View(x);
        }
    }
}