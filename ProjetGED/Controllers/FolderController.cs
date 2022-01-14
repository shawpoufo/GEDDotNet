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
            TempData["msgUpFolder"] = (isValid:true,message:"");
            return View();
        }

        [HttpPost]
        [Route("Folder/Upload")]
        public ActionResult Upload(IEnumerable<HttpPostedFileBase> uploadeFolder,string currentFolderPath)
        {
            //crée le dossier s'il n"xiste pas
            //path = userId + currentFolderPath + (extraire nom du dossier)
            //puis crée les document directement sans vérifier car le fichier est nouveau
            if(uploadeFolder.Count() > 0 && uploadeFolder.First() != null)
            {
                try
                {


                    using (var context = new GEDContext())
                    {
                        string folderName = uploadeFolder.First().FileName.Split('/')[0].Trim();
                        int userId = this.UserId();
                        var user = context.OurUsers.Include("Folders").Where(u => u.Id == userId).First();
                        string newFolderPath = Path.Combine(currentFolderPath, folderName);
                        // vérifier que le fichier qu'on veut ajouter n'existe pas 
                        // dans la base de donnée de l'utilisateur afin de le crée
                        // ici on vérifier soulement si l'utilisteur possede déja le fichier mais pas s'il a le privilege
                        // on doit ajouter la vérification du privilege ici (NotImplemented yet)
                        // par ce que l'utilisateur ne doit pas charger un dossier qui a le même chemin d'un dossier 
                        // on doit vérifier que dans le fichier dont on a access ne contient pas un dossier avec le meme nom/chemin
                        if (!user.Folders.ToList().Exists(f => f.ComparePath(newFolderPath)))
                        {
                            
                            var newFolder = new Folder { Name = folderName, Path = newFolderPath, CreatedAt = DateTime.Now };
                            
                            newFolder.Parent = user.Folders.Where(f => f.ComparePath(currentFolderPath)).First();
                           
                            
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
                            user.Folders.Add(newFolder);
                            if(context.SaveChanges() > 0)
                            {
                                string physicalFolderPath = Path.Combine(Server.MapPath("~/cloud"), newFolder.Path);
                                Directory.CreateDirectory(physicalFolderPath);
                                foreach(var document in uploadeFolder)
                                {
                                    document.SaveAs(Path.Combine(physicalFolderPath, Path.GetFileName(document.FileName)));
                                }
                                TempData["msgUpFolder"] = (isValid:true,message:"le Dossier est sauvegarder avec success");
                            }
                        }
                        else
                            TempData["msgUpFolder"] = (isValid:false,message:"le Dossier existe déja");

                    }
                    
                }
                catch 
                {

                    TempData["msgUpFolder"] = (isValid: false, message: "le Dossier n'a pas était charger");
                }
            }
            else
                TempData["msgUpFolder"] = (isValid:false,message:"aucun dossier selectionner !");

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
                Folder folder = null;
                if (string.IsNullOrEmpty(slug))
                    return new HttpNotFoundResult("Ce dossier n'existe pas");
                    //folder = user.Folders.First(f => f.Name == user.Id.ToString());
                else
                {
                    if (user.Folders.ToList().Exists(f => f.ComparePath(slug)))
                        folder = user.Folders.Where(f => f.ComparePath(slug)).First();
                    // ici on doit vérifier s'il existe dans la table privilege
                    else
                        return new HttpNotFoundResult("Ce dossier n'existe pas");

                }
                var accessDocumentId = context.AccessDocuments.Where(u => u.userId == id).Select(u => u.documentId);
                var accessDocument = new List<Document>();
                foreach(var acc in accessDocumentId)
                {
                   accessDocument.Add(context.Documents.Find(acc));
                }

                var accessFolderId = context.AccessFolders.Where(u => u.userId == id).Select(u => u.folderId);
                var accessFolder = new List<Folder>();
                foreach (var acc in accessFolderId)
                {
                    accessFolder.Add(context.Folders.Find(acc));
                }

                context.Entry(folder).Collection(r => r.Documents).Load();
                ViewData["folders"] = user.Folders.ToList().FindAll(f => f.Parent == folder);
                ViewData["documents"] = folder.Documents;
                //
                ViewData["accessDocument"] = accessDocument;
                ViewData["accessFolder"] = accessFolder;
                ViewData["users"] = context.OurUsers.ToList();
                ViewBag.folderId = slug;
            }
            return View();
        }

        [HttpPost]
        [Route("Folder/Access")]
        public ActionResult Access(string userFolder,string folderId,string read,string write,string download)
        {
            //Console.WriteLine("User Id ="+userFolder);
            //Console.WriteLine("Folder Id =" + folderId);


            using (var context = new GEDContext())
            {
                //var accessFolders = context.AccessFolders.FirstOrDefault(a => a.folderId == Int16.Parse(folderId

                //insert in accessFolders


                var v_read = (read=="true") ? 1 : 0;
                var v_write = (write == "true") ? 1 : 0;
                var v_download = (download == "true") ? 1 : 0;



                AccessFolder accessFolder = new AccessFolder(2, Int16.Parse(userFolder), Int16.Parse(folderId), v_read, v_write, v_download);
               

                context.AccessFolders.Add(accessFolder);
                context.SaveChanges();
            }
            

            return RedirectToAction("Index", new { slug = userFolder});
        }


        [HttpPost]
        [Route("Folder/DocumentAccess")]
        public ActionResult DocumentAccess(string userFolder, string documentId, string read, string write, string download)
        {
            //Console.WriteLine("User Id ="+userFolder);
            //Console.WriteLine("Folder Id =" + folderId);


            using (var context = new GEDContext())
            {
                //var accessFolders = context.AccessFolders.FirstOrDefault(a => a.folderId == Int16.Parse(folderId

                //insert in accessFolders


                var v_read = (read == "true") ? 1 : 0;
                var v_write = (write == "true") ? 1 : 0;
                var v_download = (download == "true") ? 1 : 0;



                AccessDocument accessDocument = new AccessDocument(Int16.Parse(userFolder), Int16.Parse(documentId), v_read, v_write, v_download);


                context.AccessDocuments.Add(accessDocument);
                context.SaveChanges();
            }


            return RedirectToAction("Index", new { slug = userFolder });
        }

    }
}