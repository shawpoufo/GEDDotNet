using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ProjetGED.ExtensionMethods
{
    public static class UserPropertyExtension
    {
        /// <summary>
        /// return Authenticated User Id from the ClaimsIdentity class
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <exception cref="Exception">!</exception>
        public static int UserId(this Controller c)
        {
            var claimsIdentity = c.User.Identity as ClaimsIdentity;
            return Convert.ToInt32(claimsIdentity.FindFirst("UserId").Value);
        }
    }
}