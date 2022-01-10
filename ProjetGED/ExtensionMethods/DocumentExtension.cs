using ProjetGED.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetGED.ExtensionMethods
{
    public static class DocumentExtension
    {
        public static bool ComparePath(this Document doc, string path)
        {
            return doc.Path.ToLower() == @path.Replace('/', '\\').ToLower();
        }
    }
}