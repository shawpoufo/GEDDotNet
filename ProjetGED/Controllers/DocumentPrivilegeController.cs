using ProjetGED.ExtensionMethods;
using ProjetGED.Models;
using ProjetGED.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetGED.Controllers
{
    public class DocumentPrivilegeController : Controller
    {
        // GET: DocumentPrivilege
        public ActionResult Index(string path)
        {
            if(!string.IsNullOrWhiteSpace(path))
            {
                using (var context = new GEDContext())
                {
                    int userId = this.UserId();
                    var user = context.OurUsers.Include("Documents.DocumentPrivileges.Author").Where(u => u.Id == userId).First();
                    Document document = null;
                    if ((document = user?.Documents.FirstOrDefault(f => f.ComparePath(path))) != null)
                    {
                        ViewData["path"] = path;
                        return PartialView("DocumentPrivilegeList", document.DocumentPrivileges.OrderBy(fp => fp.UserId));
                    }
                }
            }
            return PartialView("DocumentPrivilegeList", null);
        }
        [HttpPost]
        public ActionResult New (PrivilegeViewModel viewModel)
        {
            using (var context = new GEDContext())
            {
                int userId = this.UserId();
                var user = context.OurUsers.Include("Documents.DocumentPrivileges.Author").Where(u => u.Id == userId).First();
                Document document = null;
                if ((document = user?.Documents.FirstOrDefault(f => f.ComparePath(viewModel.Path))) != null)
                {
                    var newUser = context.OurUsers.Find(Convert.ToInt32(viewModel.NewUserId));
                    if (!document.DocumentPrivileges.ToList().Any(f => f.Author.Id == newUser.Id))
                    {
                        context.DocumentPrivileges.Add(new DocumentPrivilege
                        {
                            Document = document,
                            Author = newUser,
                            Read = viewModel.Read ?? false,
                            Write = viewModel.Write ?? false,
                            DownLoad = viewModel.DownLoad ?? false
                        });
                        context.SaveChanges();
                    }
                    else
                        ModelState.AddModelError("NewUserId", "Cette utilisateur dispose d'un privilege pour ce document");
                    ViewData["path"] = viewModel.Path;
                    return PartialView("DocumentPrivilegeList", document.DocumentPrivileges.OrderBy(fp => fp.UserId));
                }
            }

            return PartialView("DocumentPrivilegeList", null);
        }

        public ActionResult Update(PrivilegeViewModel viewModel)
        {
            using (var context = new GEDContext())
            {
                int userId = this.UserId();
                var user = context.OurUsers.Include("Documents.DocumentPrivileges.Author").Where(u => u.Id == userId).First();
                Document document = null;
                if ((document = user?.Documents.FirstOrDefault(f => f.ComparePath(viewModel.Path))) != null)
                {
                    var newUser = context.OurUsers.Find(Convert.ToInt32(viewModel.NewUserId));
                    if (document.DocumentPrivileges.ToList().Any(f => f.Author.Id == newUser.Id))
                    {
                        var privilege = document.DocumentPrivileges.First(fp => fp.Author.Id == newUser.Id);
                        privilege.Read = viewModel.Read ?? false;
                        privilege.Write = viewModel.Write ?? false;
                        privilege.DownLoad = viewModel.DownLoad ?? false;

                        context.Entry(privilege).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                    else
                        ModelState.AddModelError("NewUserId", "Cette utilisateur ne dispose pas d'un privilege pour ce document");
                    ViewData["path"] = viewModel.Path;
                    return PartialView("DocumentPrivilegeList", document.DocumentPrivileges.OrderBy(fp => fp.UserId));
                }
            }

            return PartialView("DocumentPrivilegeList", null);
        }
        [HttpPost]
        public ActionResult Delete(int delUserId, string path)
        {
            using (var context = new GEDContext())
            {
                int userId = this.UserId();
                var user = context.OurUsers.Include("Documents.DocumentPrivileges.Author").Where(u => u.Id == userId).First();
                Document document = null;
                if ((document = user?.Documents.FirstOrDefault(f => f.ComparePath(path))) != null)
                {
                    var newUser = context.OurUsers.Find(Convert.ToInt32(delUserId));
                    if (document.DocumentPrivileges.ToList().Any(f => f.Author.Id == newUser.Id))
                    {
                        var privilege = document.DocumentPrivileges.First(fp => fp.Author.Id == newUser.Id);

                        document.DocumentPrivileges.Remove(privilege);
                        context.Entry(privilege).State = System.Data.Entity.EntityState.Deleted;
                        context.SaveChanges();
                    }
                    else
                        ModelState.AddModelError("NewUserId", "Cette utilisateur ne dispose pas d'un privilege pour ce document");
                    ViewData["path"] = path;
                    return PartialView("DocumentPrivilegeList", document.DocumentPrivileges.OrderBy(fp => fp.UserId));
                }
            }

            return PartialView("DocumentPrivilegeList", null);
        }
    }
}