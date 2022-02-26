using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ProjetGED
{
    public class Startup
    {
        public static Func<UserManager<ApplicationUser>> UserManagerFactory { get; private set; }
        public static GEDContext DBContext { get; set; }
        static Startup()
        {
            DBContext = new GEDContext();
        }
        public void Configuration(IAppBuilder app)
        {
 
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/auth/login")
                

            });
            // configure the user manager
            UserManagerFactory = () =>
            {
                var usermanager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(DBContext));

                // allow alphanumeric characters in username
                usermanager.UserValidator = new UserValidator<ApplicationUser>(usermanager)
                {
                    AllowOnlyAlphanumericUserNames = false
                };

                usermanager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 6

                };
                

                return usermanager;
            };
        }

  
    }
}