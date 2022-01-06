using Microsoft.AspNet.Identity;
using ProjetGED.Models;
using ProjetGED.ViewModel;
using System;
using System.Collections.Generic;
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
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                Request.GetOwinContext().Authentication.SignIn(identity);

                await Task.Run(() =>
                    {
                        using (var gedContext = new GEDContext())
                        {

                            gedContext.OurUsers.Add(new User { Email = model.Email, Name = model.Name });
                            gedContext.SaveChanges();
                        }
                    }
                );

                return RedirectToAction("Index", "Home");
            }

            foreach(var err in result.Errors)
            {
                ModelState.AddModelError("", err);
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