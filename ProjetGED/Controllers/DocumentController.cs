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
            return PartialView("_Upload");
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase document , string currentFolderPath)
        {
            try
            {
                if (document.ContentLength > 0)
                {
                    var claimsIdentity = User.Identity as ClaimsIdentity;
                    var userId = Convert.ToInt32(claimsIdentity.FindFirst("UserId").Value);
                    string fileName = Path.GetFileName(document.FileName);
                    //string dbPath = Path.Combine(userId.ToString(),currentFolderPath, _FileName);
                    int version = 0;
                    // store the document in the DB
                    using (var context = new GEDContext())
                    {
                        var user = context.OurUsers.Include("Folders").Where(u => u.Id == userId).First();
                        var fullCurrentFolderPath = Path.Combine(userId.ToString(), currentFolderPath);
                        var folder = user.Folders.Where(f => f.Path == fullCurrentFolderPath).First();
                        // load documents folder (current folder)
                        context.Entry(folder).Collection(f => f.Documents).Load();
                        // increment version if file existe
                        if(folder.Documents.ToList().Exists(d => d.Name.ToLower() == fileName.ToLower()))
                        {
                            version = folder.Documents.Where(d => d.Name == fileName).Select(d => d.Version).First();
                            version++;
                        }
                        context.Documents.Add(new Models.Document { Author = user, Folder = folder, Name = fileName, UploadedAt = DateTime.Now ,Version = version , Path = Path.Combine(userId.ToString(), currentFolderPath, fileName) });
                        context.SaveChanges();
                    }
                    // add version to physical file name if he existe already
                    int dotIndex = fileName.IndexOf('.');
                    if(version > 0)
                    {
                        fileName = fileName.Insert(dotIndex, "(" + version + ")");
                    }
                    // store the document physically
                    string _physicalPath = Path.Combine(Server.MapPath("~/cloud"), userId.ToString(), currentFolderPath, fileName);
                    document.SaveAs(_physicalPath);
                }
                TempData["message"] = "File Uploaded Successfully!!";
                return RedirectToAction("Index", "Folder",new { slug = currentFolderPath });
            }
            catch
            {
                TempData["message"] = "File upload failed!!";
                return RedirectToAction("Index", "Folder", new { slug = currentFolderPath });
            }
        }
    }
}