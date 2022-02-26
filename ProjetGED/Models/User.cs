﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetGED.Models
{   [Table("user")]
    public class User
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        //[Index(IsUnique = true)]
        public string Email { get; set; }
        public ICollection<Folder> Folders { get; set; }
        public ICollection<Document> Documents { get; set; }
        //public ICollection<AccessFolder> AccessFolders { get; set; }
        public User()
        {
            Folders = new List<Folder>();
            Documents = new List<Document>();
            //AccessFolders = new List<AccessFolder>();
        }
    }
}