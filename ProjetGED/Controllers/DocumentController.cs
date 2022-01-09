using ProjetGED.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ProjetGED.Controllers
{
    public class DocumentController : Controller
    {
        // GET: Document
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult Upload()
        {
            TempData["message"] = "";
            return PartialView("_Upload",this.UserId());
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase document , string currentFolderPath)
        {
            try
            {
                if (document.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(document.FileName);
                    int version = 0;
                    // store the document in the DB
                    using (var context = new GEDContext())
                    {
                        int userId = this.UserId();
                        var user = context.OurUsers.Include("Folders").Where(u => u.Id == userId).First();
                        var folder = user.Folders.Where(f => f.ComparePath(currentFolderPath)).First();
                        // load documents folder (current folder)
                        context.Entry(folder).Collection(f => f.Documents).Load();
                        // increment version if file existe
                        if(folder.Documents.ToList().Exists(d => d.Name.ToLower() == fileName.ToLower()))
                        {
                            version = folder.Documents.Where(d => d.Name == fileName).OrderBy(d => d.Version).Select(d => d.Version).Last();
                            version++;
                        }
                        context.Documents.Add(new Models.Document {
                            Author = user,
                            Folder = folder,
                            Name = fileName,
                            UploadedAt = DateTime.Now ,
                            Version = version ,
                            Path = Path.Combine(currentFolderPath, fileName)
                        });
                        context.SaveChanges();
                    }
                    // add version to physical file name if he existe already
                    int dotIndex = fileName.IndexOf('.');
                    if(version > 0)
                    {
                        fileName = fileName.Insert(dotIndex, "(" + version + ")");
                    }
                    // store the document physically
                    string _physicalPath = Path.Combine(Server.MapPath("~/cloud") , currentFolderPath, fileName);
                    document.SaveAs(_physicalPath);
                }
                TempData["message"] = "File Uploaded Successfully!!";
                
                return RedirectToAction("Index", "Folder",new {slug = currentFolderPath });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["message"] = "File upload failed!!";
                return RedirectToAction("Index", "Folder", new {slug = currentFolderPath });
            }
        }
    }
}