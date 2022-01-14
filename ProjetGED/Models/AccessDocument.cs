using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetGED.Models
{
    [Table("AccessDocument")]
    public class AccessDocument
    {
        [Key]
        public int Id { get; set; }
        public int? userId { get; set; }

        [ForeignKey("userId")]
        public virtual User User { get; set; }


        public int? documentId { get; set; }
        [ForeignKey("documentId")]
        public virtual Document Document { get; set; }

        public int reader { get; set; }
        public int write { get; set; }
        public int download { get; set; }

        public AccessDocument(/*int id,*/ int? userId, int? documentId, int reader, int write, int download)
        {
            //Id = id;
            this.userId = userId;
            this.documentId = documentId;
            this.reader = reader;
            this.write = write;
            this.download = download;
        }
    }
}