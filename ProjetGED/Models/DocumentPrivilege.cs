using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetGED.Models
{
    public class DocumentPrivilege
    {
        [Key, Column(Order = 1)]
        public int UserId { get; set; }
        [Key, Column(Order = 2)]
        public int DocumentId { get; set; }
        public User Author { get; set; }
        public Document Document { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool DownLoad { get; set; }
    }
}