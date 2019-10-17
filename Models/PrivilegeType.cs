using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dhipaya.Models
{
    public class PrivilegeType
    {
        [Key]
        public int PrivilegeTypeID { get; set; }
        public string PrivilegeTypeName { get; set; }
        public string PrivilegeTypeDesc { get; set; }
        public StatusType Status { get; set; }

        public virtual ICollection<Privilege> Privileges { get; set; }
    }
}
