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
        /// Retourner tous les sous fichies du fichier courant
        /// </summary>
        /// <param name="folder">fichier courant</param>
        /// <param name="allFolders">la liste de tous les fichiers</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<Folder>> Tree(this Folder folder,IEnumerable<Folder> allFolders)
        {
            Stack<IEnumerable<Folder>> children = new Stack<IEnumerable<Folder>>();      
            bool check = true;
            children.Push(new[] { folder });

            while (check)
            {
                check = false;
                var oldLevel = children.Peek();
                var query = from parents in oldLevel
                            join child in allFolders on parents equals child.Parent
                            select child;

                if (query.Count() > 0)
                {
                    children.Push(query);
                    check = true;
                }
            }
            
            return children;
        }


    }
}