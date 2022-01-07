using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetGED
{
    public class ApplicationUser:IdentityUser
    {
        // add more properties
        public string LeNom { get; set; }
    }
}