using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetGED.ViewModel
{
    public class PrivilegeViewModel
    {
        [Required]
        public string NewUserId { get; set; }
        public bool? Read { get; set; }
        public bool? Write { get; set; }
        public bool? DownLoad { get; set; }
        public string Path { get; set; }

        public string PrivilegeStrategy{ get; set; }

    }
    //public class FolderPrivilegeMetaData
    //{
    //    [Remote()]
    //    public bool? DownLoad { get; set; }
    //}
}