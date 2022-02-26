using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using ProjetGED.Models;
using ProjetGED.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ProjetGED.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AuthController()
            :this(Startup.UserManagerFactory.Invoke())
        {
            
        }

        public AuthController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
         
        }
        // GET: Auth
        [HttpGet]
        public ActionResult LogIn(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("index", "home");
            var model = new LogInModel { ReturnUrl = returnUrl };
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> LogIn(LogInModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await userManager.FindAsync(model.Email, model.Password);
            
            if (user != null)
            {
                var identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;
                int userId = Startup.DBContext.OurUsers.First(u => u.Email == user.UserName).Id;
                identity.AddClaim(new Claim(type:"UserId", value:userId.ToString()));
                authManager.SignIn(identity);
              
                if (string.IsNullOrEmpty(model.ReturnUrl) || !Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(Url.Action("index", "home"));
                }
                else
                    return Redirect(model.ReturnUrl);
            }

            ModelState.AddModelError("", "Invalid email or password");
            return View();
        }

        public ActionResult LogOut()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("index", "home");
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(SignUpModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                
            };
            //var result = await userManager.CreateAsync(user, model.Password);
            //if (result.Succeeded)
            //{
            //    var identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            //    Request.GetOwinContext().Authentication.SignIn(identity);


            IEnumerable<string> resError = null;
                using (var transaction = Startup.DBContext.Database.BeginTransaction())
                {
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                        var newUser = Startup.DBContext.OurUsers.Add(new User { Email = model.Email, Name = model.Name });

                        try
                        {
                            if (Startup.DBContext.SaveChanges() > 0)
                            {
                                Directory.CreateDirectory(Server.MapPath("~") + "cloud\\" + newUser.Id);
                                newUser.Folders.Add(new Folder { CreatedAt = DateTime.Now, Parent = null, Name = newUser.Id.ToString(), Path = newUser.Id.ToString() });
                                Startup.DBContext.SaveChanges();
                                transaction.Commit();
                            }

                            identity.AddClaim(new Claim(type: "UserId", value: newUser.Id.ToString()));
                            Request.GetOwinContext().Authentication.SignIn(identity);
                            return RedirectToAction("Index", "Home");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            resError = result.Errors;
                        }
                    }

                }
            //}
            if(resError != null)
            {
                foreach (var err in resError)
                {
                    ModelState.AddModelError("", err);
                }
            }
            
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && userManager != null)
            {
              
                userManager.Dispose();

            }
            base.Dispose(disposing);
        }
    }
}