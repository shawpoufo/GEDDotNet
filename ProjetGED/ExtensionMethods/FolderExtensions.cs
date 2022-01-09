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
        /// <summary>
        /// retourner le  Path sans le fichier root 
        /// cette fonction est utiliser seulement dans l'affichage parceque userId qui est le nom du fichier root
        /// sera ajouter automatiquement par la fonction Html.ActionLink
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        
    }
}