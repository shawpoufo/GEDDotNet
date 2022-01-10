using ProjetGED.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ProjetGED.ExtensionMethods
{
    public static class FolderExtensions
    {
        public static bool ComparePath(this Folder folder, string path)
        {
            return @folder.Path.ToLower() == @path.Replace('/', '\\').ToLower();
        }
       
    }
}