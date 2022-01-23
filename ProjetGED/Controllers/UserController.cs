using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ProjetGED.ExtensionMethods;
using ProjetGED.Models;


namespace ProjetGED.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserController() : this(Startup.UserManagerFactory.Invoke())
        {
        }

        public UserController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }






        // GET: User
        [Route("User/Settings")]
        public ActionResult Index()
        {
            int userId = this.UserId();
            using (var context = new GEDContext())
            {
                User user = context.OurUsers.Where(u => u.Id == userId).FirstOrDefault();
                ViewBag.user = user;
            }
            return View();
        }


        [HttpPost]
        [Route("User/Update")]
        public async Task<ActionResult> UpdateAsync(string name,string email)
        {
            var userId = this.UserId();

           

            using (var context = new GEDContext())
            {
                User user = context.OurUsers.Where(u => u.Id == userId).FirstOrDefault();
                user.Name = name;
                user.Email = email;
                context.SaveChanges();

            }

            //Update in aspNetUsers
            var aspUser = await userManager.FindByIdAsync(User.Identity.GetUserId());
            aspUser.UserName = email;
            await userManager.UpdateAsync(aspUser);

            return RedirectToAction("Index");

        }


        [Route("User/Password")]
        public ActionResult Password()
        {
            return View();
        }


        [HttpPost]
        [Route("User/UpdatePassword")]
        public async Task<ActionResult> UpdatePasswordAsync(string oldPassword, string newPassword, string newPasswordConfirmed)
        {
            //var user = userManager.FindById(User.Identity.GetUserId());


            var user = await userManager.FindByIdAsync(User.Identity.GetUserId());

            if (userManager.CheckPassword(user,oldPassword))
            {
                if (newPassword == newPasswordConfirmed)
                {
                    
                    var hashPassword = userManager.PasswordHasher.HashPassword(newPassword);
                    user.PasswordHash = hashPassword;
                    await userManager.UpdateAsync(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "The password and confirmation password do not match";
                    return View("Password");
                }
            }
            else
            {
                ViewBag.ErrorOldPassword = "Error on Password";
                return View("Password");
            }
            
            


            //userManager.RemovePassword(User.Identity.GetUserId());

            //userManager.AddPassword(User.Identity.GetUserId(), "aymen");



           
        }

        
    }
}