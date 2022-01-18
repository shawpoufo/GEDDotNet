using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetGED.Models
{
    [Table("Document")]
    public class Document
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
        public string Path { get; set; }
        //[ForeignKey("Author")]
        public User Author { get; set; }
        //[ForeignKey("Folder")]
        public Folder Folder { get; set; }
        public DateTime UploadedAt { get; set; }
        public ICollection<DocumentPrivilege> DocumentPrivileges { get; set; }
        public Document()
        {
            DocumentPrivileges = new List<DocumentPrivilege>();
        }
    }
}