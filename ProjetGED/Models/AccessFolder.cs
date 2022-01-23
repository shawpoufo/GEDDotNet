using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace ProjetGED.Models
{

    [Table("AccessFolder")]
    public class AccessFolder
    {
        [Key]
        public int Id { get; set; }
        public int? userId { get; set; }

        [ForeignKey("userId")]
        public virtual User User { get; set; }


        public int? folderId { get; set; }
        [ForeignKey("folderId")]
        public virtual Folder Folder { get; set; }

        public int reader { get; set; }
        public int write { get; set; }
        public int download { get; set; }

        public AccessFolder(int id, int? userId, int? folderId, int reader, int write, int download)
        {
            Id = id;
            this.userId = userId;
            this.folderId = folderId;
            this.reader = reader;
            this.write = write;
            this.download = download;
        }

    }
    
}
