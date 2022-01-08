using ProjetGED.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ProjetGED.Controllers
{
    public class FolderController : Controller
    {
        // GET: Folder
        [Route("Folder/{*slug}")]
        public ActionResult Index(string slug)
        {
            ViewData["foldersUrl"] = slug;
            using (var context = new GEDContext())
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var id = Convert.ToInt32(claimsIdentity.FindFirst("UserId").Value);
                var user = context.OurUsers.Include("Folders").First(u => u.Id == id);
                Folder folder = null;
                //if (string.IsNullOrEmpty(slug))
                    folder = user.Folders.First(f => f.Name == user.Id.ToString());
                //else
                //{
                //    if (user.Folders.ToList().Exists(f => f.Path == slug))
                //        folder = user.Folders.Where(f => f.Path == slug).First();
                //    else
                //        return new HttpNotFoundResult("Ce dossier n'existe pas");

                //}
                context.Entry(folder).Collection(r => r.Documents).Load();
                ViewData["folders"] = user.Folders.ToList().FindAll(f => f.Parent == folder);
                ViewData["documents"] = folder.Documents;
            }
            return View();
        }
    }
}