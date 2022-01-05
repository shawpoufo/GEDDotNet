using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetGED.Models
{   [Table("Folder")]
    public class Folder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
            
        public Folder Parent { get; set; }
        public User Author { get; set; }
    }
}