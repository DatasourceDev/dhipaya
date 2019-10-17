using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
    public class PrivilegeImage
   {
        [Key]
        public int PrivilegeImageID { get; set; }
        public int PrivilegeID { get; set; }

        public string Url { get; set; }

        public virtual Privilege Privilege { get; set; }
    }
}
