using ProjetGED.ExtensionMethods;
using ProjetGED.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
namespace ProjetGED.Controllers
{
    public class FolderController : Controller
    {
        public ActionResult Upload()
        {
            TempData["msgUpFolder"] = (isValid: true, message: "");
            return View();
        }

        [HttpPost]
        [Route("Folder/Upload")]
        public ActionResult Upload(IEnumerable<HttpPostedFileBase> uploadeFolder, string currentFolderPath)
        {
            //crée le dossier s'il n"xiste pas
            //path = userId + currentFolderPath + (extraire nom du dossier)
            //puis crée les document directement sans vérifier car le fichier est nouveau
            if (uploadeFolder.Count() > 0 && uploadeFolder.First() != null)
            {
                try
                {

                    using (var context = new GEDContext())
                    {
                        string folderName = uploadeFolder.First().FileName.Split('/')[0].Trim();
                        int userId = this.UserId();
                        var user = context.OurUsers.Include("Folders").Where(u => u.Id == userId).First();
                        context.Entry(user).Collection(u => u.FolderPrivileges).Load();
                        string newFolderPath = Path.Combine(currentFolderPath, folderName);
                        Folder folderParrent = context.Folders.ToList().FirstOrDefault(f => f.ComparePath(currentFolderPath));

                        if (folderParrent != null)
                        {
                            bool check = false;
                            if (user.Folders.ToList().Exists(f => f.Id == folderParrent.Id))
                                check = true;
                            else if (!user.FolderPrivileges.ToList().Exists(f => f.FolderId == folderParrent.Id))
                            {
                                return new HttpUnauthorizedResult("L'accés à ce Dossier est non autorisé");
                            }
                            else if (!context.FolderPrivileges.Find(user.Id, folderParrent.Id).Write)
                            {
                                TempData["msgUpFolder"] = (isValid: false, message: "Vous n'avez pas le droit de charger des documents dans ce dossier");
                            }
                            else
                            {
                                check = true;
                            }

                            if (check)
                            {
                                var newFolder = new Folder { Name = folderName, Path = newFolderPath, CreatedAt = DateTime.Now };
                                newFolder.Parent = folderParrent;
                                foreach (var document in uploadeFolder)
                                {
                                    string name = Path.GetFileName(document.FileName);

                                    newFolder.Documents.Add(new Document
                                    {
                                        Name = name,
                                        Author = user,
                                        Path = Path.Combine(newFolder.Path, name),
                                        UploadedAt = DateTime.Now,
                                        Version = 0
                                    });
                                }
                                context.Folders.Add(newFolder);

                                if (context.SaveChanges() > 0)
                                {
                                    string physicalFolderPath = Path.Combine(Server.MapPath("~/cloud"), newFolder.Path);
                                    Directory.CreateDirectory(physicalFolderPath);
                                    foreach (var document in uploadeFolder)
                                    {
                                        document.SaveAs(Path.Combine(physicalFolderPath, Path.GetFileName(document.FileName)));
                                    }
                                    TempData["msgUpFolder"] = (isValid: true, message: "le Dossier est sauvegarder avec success");
                                }
                            }
                        }
                        else
                            return HttpNotFound("Dossier inexistant");
                    }
                }
                catch(Exception ex)
                {
                    TempData["msgUpFolder"] = (isValid: false, message: "le Dossier n'a pas était charger");
                }
            }
            else
                TempData["msgUpFolder"] = (isValid: false, message: "Aucun dossier selectionner !");

            return RedirectToAction("Index", new { slug = currentFolderPath });
        }

        // GET: Folder
        [Route("Folder/{*slug}")]
        public ActionResult Index(string slug)
        {

            using (var context = new GEDContext())
            {
                var id = this.UserId();
                var user = context.OurUsers.Include("Folders").First(u => u.Id == id);
                context.Entry(user).Collection(u => u.FolderPrivileges).Load();
                if (string.IsNullOrEmpty(slug))
                    return new HttpNotFoundResult("Ce dossier n'existe pas");
                else
                {
                    Folder folder = context.Folders.ToList().FirstOrDefault(f => f.ComparePath(slug));
                    if (user.Folders.ToList().Exists(f => f.ComparePath(slug)) || user.FolderPrivileges.ToList().Exists(f => f.Folder.ComparePath(slug)))
                    {
                        context.Entry(folder).Collection(r => r.Documents).Load();
                        ViewData["folders"] = user.Folders.ToList().FindAll(f => f.Parent == folder);
                        ViewData["documents"] = folder.Documents;
                    }
                    else if (folder != null && !user.FolderPrivileges.ToList().Exists(f => f.Folder.ComparePath(slug)))
                    {
                        // crée une rdirection customized
                        return Content(System.Net.HttpStatusCode.Unauthorized.ToString(), "Not authorized");
                    }
                    else 
                        return new HttpNotFoundResult("Ce dossier n'existe pas");
                   
                }
                
            }
            return View();
        }

        
    }
    
}