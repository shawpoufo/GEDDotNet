using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetGED.Models
{   [Table("Folder")]
    public class Folder
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        [ForeignKey("Parent")]
        public Folder Parent { get; set; }
        [ForeignKey("Author")]
        public User Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}