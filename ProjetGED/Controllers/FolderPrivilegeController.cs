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
    public class FolderPrivilegeController : Controller
    {
        public PartialViewResult Index(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                using (var context = new GEDContext())
                {
                    int userId = this.UserId();
                    var user = context.OurUsers.Include("Folders.FolderPrivileges.Author").Where(u => u.Id == userId).First();
                    Folder folder = null;
                    if ((folder = user?.Folders.FirstOrDefault(f => f.ComparePath(path))) != null)
                    {

                        ViewData["path"] = path;
                        return PartialView("FolderPrivilegeList", folder.FolderPrivileges.OrderBy(fp => fp.UserId));
                    }
                }
            }
            return PartialView("FolderPrivilegeList", null);
        }
        [HttpPost]
        public ActionResult New(PrivilegeViewModel viewModel)
        {
            using (var context = new GEDContext())
            {
                int userId = this.UserId();
                var user = context.OurUsers.Include("Folders.FolderPrivileges.Author").Where(u => u.Id == userId).First();
                Folder folder = null;
                if ((folder = user?.Folders.FirstOrDefault(f => f.ComparePath(viewModel.Path))) != null)
                {
                    var newUser = context.OurUsers.Find(Convert.ToInt32(viewModel.NewUserId));
                    if (!folder.FolderPrivileges.ToList().Any(f => f.Author.Id == newUser.Id))
                    {

                        if (viewModel.PrivilegeStrategy == "directe")
                        {
                            context.FolderPrivileges.Add(new FolderPrivilege
                            {
                                Folder = folder,
                                Author = newUser,
                                Read = viewModel.Read ?? false,
                                Write = viewModel.Write ?? false,
                                DownLoad = viewModel.DownLoad ?? false
                            });

                            context.Entry(folder).Collection(f => f.Documents).Load();

                            foreach (var document in folder.Documents)
                            {
                                if (context.DocumentPrivileges.Find(newUser.Id, document.Id) == null)
                                {

                                    context.DocumentPrivileges.Add(new DocumentPrivilege
                                    {
                                        Document = document,
                                        Author = newUser,
                                        Read = viewModel.Read ?? false,
                                        Write = viewModel.Write ?? false,
                                        DownLoad = viewModel.DownLoad ?? false
                                    });
                                }

                            }
                        }
                        else
                        {
                            var levels = folder.Tree(context.Folders).ToList();
                            foreach (var level in levels)
                            {
                                foreach (var child in level)
                                {
                                    if(context.FolderPrivileges.Find(newUser.Id,child.Id) == null)
                                    {
                                        context.FolderPrivileges.Add(new FolderPrivilege
                                        {
                                            Folder = child,
                                            Author = newUser,
                                            Read = viewModel.Read ?? false,
                                            Write = viewModel.Write ?? false,
                                            DownLoad = viewModel.DownLoad ?? false
                                        });

                                        context.Entry(child).Collection(f => f.Documents).Load();

                                        foreach (var document in child.Documents)
                                        {
                                            if (context.DocumentPrivileges.Find(newUser.Id, document.Id) == null)
                                            {
                                                context.DocumentPrivileges.Add(new DocumentPrivilege
                                                {
                                                    Document = document,
                                                    Author = newUser,
                                                    Read = viewModel.Read ?? false,
                                                    Write = viewModel.Write ?? false,
                                                    DownLoad = viewModel.DownLoad ?? false
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        context.SaveChanges();
                    }
                    else
                        ModelState.AddModelError("NewUserId", "Cette utilisateur dispose d'un privilege pour ce dossier");
                    ViewData["path"] = viewModel.Path;
                    return PartialView("FolderPrivilegeList", folder.FolderPrivileges.OrderBy(fp => fp.UserId));
                }
            }

            return PartialView("FolderPrivilegeList", null);
        }
        [HttpPost]
        public ActionResult Update(PrivilegeViewModel viewModel)
        {
            using (var context = new GEDContext())
            {
                int userId = this.UserId();
                var user = context.OurUsers.Include("Folders.FolderPrivileges.Author").Where(u => u.Id == userId).First();
                Folder folder = null;
                if ((folder = user?.Folders.FirstOrDefault(f => f.ComparePath(viewModel.Path))) != null)
                {
                    var newUser = context.OurUsers.Find(Convert.ToInt32(viewModel.NewUserId));
                    if (folder.FolderPrivileges.ToList().Any(f => f.Author.Id == newUser.Id))
                    {
                        if (viewModel.PrivilegeStrategy == "directe")
                        {
                            var folderPrivilege = context.FolderPrivileges.Find(newUser.Id, folder.Id);

                            folderPrivilege.Read = viewModel.Read ?? false;
                            folderPrivilege.Write = viewModel.Write ?? false;
                            folderPrivilege.DownLoad = viewModel.DownLoad ?? false;

                            context.Entry(folderPrivilege).State = System.Data.Entity.EntityState.Modified;
                            context.Entry(folder).Collection(f => f.Documents).Load();

                            foreach (var document in folder.Documents)
                            {
                                DocumentPrivilege documentPrivilege = null;

                                if ((documentPrivilege = context.DocumentPrivileges.Find(newUser.Id, document.Id)) != null)
                                {
                                    documentPrivilege.Read = viewModel.Read ?? false;
                                    documentPrivilege.Write = viewModel.Write ?? false;
                                    documentPrivilege.DownLoad = viewModel.DownLoad ?? false;

                                    context.Entry(documentPrivilege).State = System.Data.Entity.EntityState.Modified;
                                }
                            }
                        }
                        else
                        {
                            var levels = folder.Tree(context.Folders).ToList();
                            foreach (var level in levels)
                            {
                                foreach (var child in level)
                                {
                                    FolderPrivilege folderPrivilege = null;

                                    if ((folderPrivilege = context.FolderPrivileges.Find(newUser.Id, child.Id)) != null)
                                    {

                                        folderPrivilege.Read = viewModel.Read ?? false;
                                        folderPrivilege.Write = viewModel.Write ?? false;
                                        folderPrivilege.DownLoad = viewModel.DownLoad ?? false;
                                        context.Entry(folderPrivilege).State = System.Data.Entity.EntityState.Modified;

                                        context.Entry(child).Collection(f => f.Documents).Load();

                                        foreach (var document in child.Documents)
                                        {
                                            DocumentPrivilege documentPrivilege = null;
                                            if ((documentPrivilege = context.DocumentPrivileges.Find(newUser.Id, document.Id)) != null)
                                            {
                                                documentPrivilege.Read = viewModel.Read ?? false;
                                                documentPrivilege.Write = viewModel.Write ?? false;
                                                documentPrivilege.DownLoad = viewModel.DownLoad ?? false;
                                                context.Entry(documentPrivilege).State = System.Data.Entity.EntityState.Modified;

                                            }
                                        }
                                    }
                                }
                            }
                        }

                        context.SaveChanges();
                    }
                    else
                        ModelState.AddModelError("NewUserId", "Cette utilisateur ne dispose pas d'un privilege pour ce dossier");
                    ViewData["path"] = viewModel.Path;
                    return PartialView("FolderPrivilegeList", folder.FolderPrivileges.OrderBy(fp => fp.UserId));
                }
            }

            return PartialView("FolderPrivilegeList", null);
        }
        [HttpPost]
        public ActionResult Delete(int delUserId, string path, string privilegeStrategy)
        {
            using (var context = new GEDContext())
            {
                int userId = this.UserId();
                var user = context.OurUsers.Include("Folders.FolderPrivileges.Author").Where(u => u.Id == userId).First();
                Folder folder = null;
                if ((folder = user?.Folders.FirstOrDefault(f => f.ComparePath(path))) != null)
                {
                    var newUser = context.OurUsers.Find(Convert.ToInt32(delUserId));
                    if (folder.FolderPrivileges.ToList().Any(f => f.Author.Id == newUser.Id))
                    {
                        if (privilegeStrategy == "directe")
                        {
                            var privilege = folder.FolderPrivileges.First(fp => fp.Author.Id == newUser.Id);

                            folder.FolderPrivileges.Remove(privilege);
                            context.Entry(privilege).State = System.Data.Entity.EntityState.Deleted;

                            context.Entry(folder).Collection(f => f.Documents).Load();

                            foreach (var document in folder.Documents)
                            {
                                DocumentPrivilege documentPrivilege = null;
                                if ((documentPrivilege = context.DocumentPrivileges.Find(newUser.Id, document.Id)) != null)
                                {
                                    context.DocumentPrivileges.Remove(documentPrivilege);
                                }

                            }

                        }
                        else
                        {
                            var levels = folder.Tree(context.Folders).ToList();
                            foreach (var level in levels)
                            {
                                foreach (var child in level)
                                {
                                    FolderPrivilege folderPrivilege = null;

                                    if ((folderPrivilege = context.FolderPrivileges.Find(newUser.Id, child.Id)) != null)
                                    {
                                        context.FolderPrivileges.Remove(folderPrivilege);
                                        context.Entry(child).Collection(f => f.Documents).Load();

                                        foreach (var document in child.Documents)
                                        {
                                            DocumentPrivilege documentPrivilege = null;
                                            if ((documentPrivilege = context.DocumentPrivileges.Find(newUser.Id, document.Id)) != null)
                                            {
                                                context.DocumentPrivileges.Remove(documentPrivilege);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        context.SaveChanges();
                    }
                    else
                        ModelState.AddModelError("NewUserId", "Cette utilisateur ne dispose pas d'un privilege pour ce dossier");
                    ViewData["path"] = path;
                    return PartialView("FolderPrivilegeList", folder.FolderPrivileges.OrderBy(fp => fp.UserId));
                }
            }

            return PartialView("FolderPrivilegeList", null);
        }

    }
}